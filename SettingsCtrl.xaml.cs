using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;

namespace TexturedRender
{
    /// <summary>
    /// Interaction logic for SettingsCtrl.xaml
    /// </summary>
    public partial class SettingsCtrl : UserControl
    {
        List<Pack> resourcePacks = new List<Pack>();
        Settings settings;

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        bool inited = false;
        public SettingsCtrl(Settings settings)
        {
            this.settings = settings;
            InitializeComponent();
            inited = true;
            paletteList.SetPath("Plugins\\Assets\\Textured\\Palettes", 1f);
            ReloadPacks();
            SetValues();
        }

        void WriteDefaultPack()
        {
            string dir = "Plugins\\Assets\\Textured\\Resources\\Default";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            Properties.Resources.keyBlack.Save(dir + "\\blackKey.png");
            Properties.Resources.keyBlackPressed.Save(dir + "\\blackKeyPressed.png");
            Properties.Resources.keyWhite.Save(dir + "\\whiteKey.png");
            Properties.Resources.keyWhitePressed.Save(dir + "\\whiteKeyPressed.png");
            Properties.Resources.note.Save(dir + "\\note.png");
            File.WriteAllBytes(dir + "\\pack.json", Properties.Resources.pack);
        }

        void ReloadPacks()
        {
            int lastSelected = pluginList.SelectedIndex;
            string lastSelectedName;
            if (lastSelected == -1)
            {
                lastSelectedName = "Default";
                lastSelected = 0;
            }
            else
            {
                lastSelectedName = (string)((ListBoxItem)pluginList.SelectedItem).Content;
            }

            WriteDefaultPack();
            Bitmap GetBitmap(string path)
            {
                FileStream s;
                try
                {
                    s = File.OpenRead(path);
                }
                catch { throw new Exception("Could not open " + path); }
                Bitmap b;
                try
                {
                    b = new Bitmap(s);
                }
                catch { throw new Exception("Corrupt image: " + path); }
                s.Close();
                return b;
            }
            string dir = "Plugins\\Assets\\Textured\\Resources";
            string[] packs = Directory.GetDirectories(dir);
            resourcePacks.Clear();
            foreach(var r in resourcePacks)
            {
                r.whiteKeyTex.Dispose();
                r.blackKeyTex.Dispose();
                r.whiteKeyPressedTex.Dispose();
                r.blackKeyPressedTex.Dispose();
                r.preview.Dispose();
                foreach(var n in r.NoteTextures)
                {
                    n.noteMiddleTex.Dispose();
                    n.noteBottomTex.Dispose();
                    n.noteTopTex.Dispose();
                }
            }
            pluginList.Items.Clear();
            foreach (var p in packs)
            {
                var pack = new Pack() { name = p.Split('\\').Last() };
                try
                {
                    string json;
                    try
                    {
                        json = File.ReadAllText(Path.Combine(p, "pack.json"));
                    }
                    catch { throw new Exception("Could not find pack.json file"); }
                    dynamic data;
                    try
                    {
                        data = (dynamic)JsonConvert.DeserializeObject(json);
                    }
                    catch { throw new Exception("Corrupt json in pack.json"); }
                    try
                    {
                        pack.description = data.description;
                    }
                    catch { pack.description = "[no description]"; }

                    string pname;
                    try
                    {
                        pname = data.previewImage;
                        pack.preview = GetBitmap(Path.Combine(p, pname));
                    }
                    catch { }

                    try
                    {
                        pack.keyboardHeight = data.keyboardHeight;
                    }
                    catch { }
                    try
                    {
                        pack.sameWidthNotes = data.sameWidthNotes;
                    }
                    catch { }

                    #region Get Keys
                    try
                    {
                        pname = data.blackKey;
                    }
                    catch { throw new Exception("Missing property \"blackKey\""); }
                    pack.blackKeyTex = GetBitmap(Path.Combine(p, pname));
                    try
                    {
                        pname = data.blackKeyPressed;
                    }
                    catch { throw new Exception("Missing property \"blackKeyPressed\""); }
                    pack.blackKeyPressedTex = GetBitmap(Path.Combine(p, pname));
                    try
                    {
                        pname = data.whiteKey;
                    }
                    catch { throw new Exception("Missing property \"whiteKey\""); }
                    pack.whiteKeyTex = GetBitmap(Path.Combine(p, pname));
                    try
                    {
                        pname = data.whiteKeyPressed;
                    }
                    catch { throw new Exception("Missing property \"whiteKeyPressed\""); }
                    pack.whiteKeyPressedTex = GetBitmap(Path.Combine(p, pname));
                    #endregion

                    JArray noteSizes;
                    try
                    {
                        noteSizes = data.notes;
                    }
                    catch { throw new Exception("Missing Array Property \"notes\""); }
                    if (noteSizes.Count == 0) throw new Exception("Note textures array can't be 0");
                    if (noteSizes.Count > 4) throw new Exception("Only up to 4 note textures are supported");

                    List<NoteTexture> noteTex = new List<NoteTexture>();
                    foreach (dynamic s in noteSizes)
                    {
                        NoteTexture tex = new NoteTexture();
                        try
                        {
                            tex.useCaps = (bool)s.useEndCaps;
                        }
                        catch { throw new Exception("Missing property \"useEndCaps\" in note size textures"); }
                        try
                        {
                            tex.stretch = (bool)s.alwaysStretch;
                        }
                        catch { throw new Exception("Missing property \"alwaysStretch\" in note size textures"); }
                        try
                        {
                            tex.maxSize = (double)s.maxSize;
                        }
                        catch { throw new Exception("Missing property \"maxSize\" in note size textures"); }

                        try
                        {
                            pname = s.middleTexture;
                        }
                        catch { throw new Exception("Missing property \"middleTexture\""); }
                        tex.noteMiddleTex = GetBitmap(Path.Combine(p, pname));
                        tex.noteMiddleAspect = (double)tex.noteMiddleTex.Height / tex.noteMiddleTex.Width;

                        if (tex.useCaps)
                        {
                            try
                            {
                                pname = s.topTexture;
                            }
                            catch { throw new Exception("Missing property \"topTexture\""); }
                            tex.noteTopTex = GetBitmap(Path.Combine(p, pname));
                            try
                            {
                                pname = s.bottomTexture;
                            }
                            catch { throw new Exception("Missing property \"bottomTexture\""); }
                            tex.noteBottomTex = GetBitmap(Path.Combine(p, pname));
                            tex.noteTopAspect = (double)tex.noteTopTex.Height / tex.noteTopTex.Width;
                            tex.noteBottomAspect = (double)tex.noteBottomTex.Height / tex.noteBottomTex.Width;

                            try
                            {
                                tex.noteTopOversize = (double)s.topOversize;
                            }
                            catch { throw new Exception("Missing property \"topOversize\" in note size textures"); }
                            try
                            {
                                tex.noteBottomOversize = (double)s.bottomOversize;
                            }
                            catch { throw new Exception("Missing property \"bottomOversize\" in note size textures"); }
                        }
                        noteTex.Add(tex);
                    }

                    noteTex.Sort((c1, c2) =>
                    {
                        if (c1.maxSize < c2.maxSize) return -1;
                        if (c1.maxSize > c2.maxSize) return 1;
                        return 0;
                    });
                    noteTex.Last().maxSize = double.PositiveInfinity;
                    pack.NoteTextures = noteTex.ToArray();
                }
                catch (Exception e)
                {
                    pack.error = true;
                    pack.description = e.Message;
                }
                resourcePacks.Add(pack);
                pluginList.Items.Add(new ListBoxItem()
                {
                    Content = pack.name
                });
            }

            if ((string)((ListBoxItem)pluginList.Items[lastSelected]).Content == lastSelectedName)
            {
                pluginList.SelectedIndex = lastSelected;
            }
            else
            {
                foreach (ListBoxItem p in pluginList.Items)
                {
                    if ((string)p.Content == lastSelectedName)
                    {
                        pluginList.SelectedItem = p;
                        break;
                    }
                }
            }
        }

        private void PluginList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var pack = resourcePacks[pluginList.SelectedIndex];
                if (!pack.error)
                {
                    pluginDesc.Foreground = Brushes.Black;
                    settings.currPack = pack;
                }
                else
                {
                    pluginDesc.Foreground = Brushes.Red;
                    settings.currPack = null;
                }
                if (pack.preview == null)
                    previewImg.Source = null;
                else
                    previewImg.Source = BitmapToImageSource(pack.preview);
                pluginDesc.Text = pack.description;
                settings.lastPackChangeTime = DateTime.Now.Ticks;
            }
            catch { }
        }

        public void SetValues()
        {
            firstNote.Value = settings.firstNote;
            lastNote.Value = settings.lastNote - 1;
            noteDeltaScreenTime.Value = Math.Log(settings.deltaTimeOnScreen, 2);
            blackNotesAbove.IsChecked = settings.blackNotesAbove;
            paletteList.SelectImage(settings.palette);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadPacks();
        }

        bool screenTimeLock = false;
        private void ScreenTime_nud_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!inited) return;
            try
            {
                if (screenTimeLock) return;
                screenTimeLock = true;
                noteDeltaScreenTime.Value = Math.Log((double)screenTime_nud.Value, 2);
                settings.deltaTimeOnScreen = (double)screenTime_nud.Value;
                screenTimeLock = false;
            }
            catch
            {
                screenTimeLock = false;
            }
        }
        
        private void BlackNotesAbove_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited) return;
            try
            {
                settings.blackNotesAbove = (bool)blackNotesAbove.IsChecked;
            }
            catch (NullReferenceException) { }
        }

        private void NoteDeltaScreenTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!inited) return;
            try
            {
                if (screenTimeLock) return;
                screenTimeLock = true;
                settings.deltaTimeOnScreen = Math.Pow(2, noteDeltaScreenTime.Value);
                screenTime_nud.Value = (decimal)settings.deltaTimeOnScreen;
                screenTimeLock = false;
            }
            catch (NullReferenceException)
            {
                screenTimeLock = false;
            }
        }

        private void Nud_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!inited) return;
            try
            {
                if (sender == firstNote) settings.firstNote = (int)firstNote.Value;
                if (sender == lastNote) settings.lastNote = (int)lastNote.Value + 1;
                if (sender == noteDeltaScreenTime) settings.deltaTimeOnScreen = (int)noteDeltaScreenTime.Value;
            }
            catch (NullReferenceException) { }
            catch (InvalidOperationException) { }
        }
    }
}
