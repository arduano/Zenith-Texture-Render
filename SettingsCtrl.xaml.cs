using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Cryptography;
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

        enum PackType
        {
            Folder, Zip, Zrp
        }

        public event Action PaletteChanged
        {
            add { paletteList.PaletteChanged += value; }
            remove { paletteList.PaletteChanged -= value; }
        }

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
            paletteList.SetPath("Plugins\\Assets\\Palettes", 1f);
            ReloadPacks();
            SetValues();
        }

        void WriteDefaultPack()
        {
            string dir = "Plugins\\Assets\\Textured\\Resources\\Default";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            Properties.Resources.keyBlack.Save(dir + "\\keyBlack.png");
            Properties.Resources.keyBlackPressed.Save(dir + "\\keyBlackPressed.png");
            Properties.Resources.keyWhite.Save(dir + "\\keyWhite.png");
            Properties.Resources.keyWhitePressed.Save(dir + "\\keyWhitePressed.png");
            Properties.Resources.note.Save(dir + "\\note.png");
            Properties.Resources.bar.Save(dir + "\\bar.png");
            Properties.Resources.noteEdge.Save(dir + "\\noteEdge.png");
            Properties.Resources.preview.Save(dir + "\\preview.png");
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

            string dir = "Plugins\\Assets\\Textured\\Resources";
            foreach (var r in resourcePacks)
            {
                if (r.whiteKeyTex != null)
                    r.whiteKeyTex.Dispose();
                if (r.blackKeyTex != null)
                    r.blackKeyTex.Dispose();
                if (r.whiteKeyPressedTex != null)
                    r.whiteKeyPressedTex.Dispose();
                if (r.blackKeyPressedTex != null)
                    r.blackKeyPressedTex.Dispose();
                if (r.preview != null)
                    r.preview.Dispose();
                if (r.NoteTextures != null)
                    foreach (var n in r.NoteTextures)
                    {
                        if (n.noteMiddleTex != null)
                            n.noteMiddleTex.Dispose();
                        if (n.noteBottomTex != null)
                            n.noteBottomTex.Dispose();
                        if (n.noteTopTex != null)
                            n.noteTopTex.Dispose();
                    }
            }
            resourcePacks.Clear();
            WriteDefaultPack();
            pluginList.Items.Clear();
            foreach (var p in Directory.GetDirectories(dir))
            {
                var pack = LoadPack(p, PackType.Folder);
                resourcePacks.Add(pack);
                pluginList.Items.Add(new ListBoxItem()
                {
                    Content = pack.name
                });
            }
            foreach (var p in Directory.GetFiles(dir, "*.zip"))
            {
                var pack = LoadPack(p, PackType.Zip);
                resourcePacks.Add(pack);
                pluginList.Items.Add(new ListBoxItem()
                {
                    Content = pack.name,
                    Foreground = Brushes.Green
                });
            }
            foreach (var p in Directory.GetFiles(dir, "*.zrp"))
            {
                var pack = LoadPack(p, PackType.Zrp);
                resourcePacks.Add(pack);
                pluginList.Items.Add(new ListBoxItem()
                {
                    Content = pack.name,
                    Foreground = Brushes.Green
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

        Pack LoadPack(string p, PackType type)
        {
            var pack = new Pack() { name = p.Split('\\').Last() };

            string pbase = "";

            ZipArchive archive = null;
            MemoryStream zrpstream = null;

            TextureShaderType strToShader(string s)
            {
                if (s == "normal") return TextureShaderType.Normal;
                if (s == "inverse") return TextureShaderType.Inverted;
                if (s == "hybrid") return TextureShaderType.Hybrid;
                throw new Exception("Unknown shader type \"" + s + "\"");
            }

            KeyType strToKeyType(string s)
            {
                if (s == "black") return KeyType.Black;
                if (s == "white") return KeyType.White;
                if (s == "both") return KeyType.Both;
                throw new Exception("Unknown key type \"" + s + "\"");
            }

            Bitmap GetBitmap(string path)
            {
                if (type == PackType.Folder)
                {
                    path = Path.Combine(p, pbase, path);
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
                else
                {
                    path = Path.Combine(pbase, path);
                    Stream s;
                    try
                    {
                        s = archive.GetEntry(path).Open();
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
            }
            try
            {
                string json = "";
                if (type == PackType.Folder)
                {
                    var files = Directory.GetFiles(p, "*pack.json", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith("\\pack.json"))
                        .Select(s => s.Substring(p.Length + 1))
                        .ToArray();
                    Array.Sort(files.Select(s => s.Length).ToArray(), files);
                    var jsonpath = files[0];
                    pbase = jsonpath.Substring(0, jsonpath.Length - "pack.json".Length);
                    if (files.Length == 0) throw new Exception("Could not find pack.json file");
                    try
                    {
                        json = File.ReadAllText(Path.Combine(p, jsonpath));
                    }
                    catch { throw new Exception("Could not read pack.json file"); }
                }
                else
                {
                    if (type == PackType.Zrp)
                    {
                        var encoded = File.OpenRead(p);
                        var key = new byte[16];
                        var iv = new byte[16];
                        encoded.Read(key, 0, 16);
                        encoded.Read(iv, 0, 16);

                        zrpstream = new MemoryStream();

                        using (AesManaged aes = new AesManaged())
                        {
                            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                            using (CryptoStream cs = new CryptoStream(encoded, decryptor, CryptoStreamMode.Read))
                            {
                                cs.CopyTo(zrpstream);
                            }
                            zrpstream.Position = 0;
                            archive = new ZipArchive(zrpstream);
                        }
                    }
                    else
                    {
                        archive = ZipFile.OpenRead(p);
                    }
                    var files = archive.Entries.Where(e => e.Name == "pack.json").ToArray();
                    Array.Sort(files.Select(s => s.FullName.Length).ToArray(), files);
                    if (files.Length == 0) throw new Exception("Could not find pack.json file");
                    var jsonfile = files[0];
                    pbase = jsonfile.FullName.Substring(0, jsonfile.FullName.Length - "pack.json".Length);
                    using (var jfile = new StreamReader(jsonfile.Open()))
                    {
                        json = jfile.ReadToEnd();
                    }
                }
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
                    pack.preview = GetBitmap(pname);
                }
                catch { }

                #region Misc
                try
                {
                    pack.keyboardHeight = (double)data.keyboardHeight / 100;
                }
                catch { }
                try
                {
                    pack.sameWidthNotes = data.sameWidthNotes;
                }
                catch { }
                try
                {
                    pack.blackKeysFullOctave = data.blackKeysFullOctave;
                }
                catch { }
                try
                {
                    pack.whiteKeysFullOctave = data.whiteKeysFullOctave;
                }
                catch { }
                try
                {
                    pack.blackKeyHeight = (double)data.blackKeyHeight / 100;
                }
                catch { }
                try
                {
                    pack.blackKeyDefaultWhite = data.blackKeysWhiteShade;
                }
                catch { }
                #endregion

                #region Shaders
                string shader = null;
                try
                {
                    shader = data.noteShader;
                }
                catch { }
                if (shader != null) pack.noteShader = strToShader(shader);
                shader = null;
                try
                {
                    shader = data.whiteKeyShader;
                }
                catch { }
                if (shader != null) pack.whiteKeyShader = strToShader(shader);
                shader = null;
                try
                {
                    shader = data.blackKeyShader;
                }
                catch { }
                if (shader != null) pack.blackKeyShader = strToShader(shader);
                #endregion

                #region Get Keys
                try
                {
                    pname = data.blackKey;
                }
                catch { throw new Exception("Missing property \"blackKey\""); }
                pack.blackKeyTex = GetBitmap(pname);
                try
                {
                    pname = data.blackKeyPressed;
                }
                catch { throw new Exception("Missing property \"blackKeyPressed\""); }
                pack.blackKeyPressedTex = GetBitmap(pname);
                try
                {
                    pname = data.whiteKey;
                }
                catch { throw new Exception("Missing property \"whiteKey\""); }
                pack.whiteKeyTex = GetBitmap(pname);
                try
                {
                    pname = data.whiteKeyPressed;
                }
                catch { throw new Exception("Missing property \"whiteKeyPressed\""); }
                pack.whiteKeyPressedTex = GetBitmap(pname);

                try
                {
                    pname = data.whiteKeyLeft;
                    pack.whiteKeyLeftTex = GetBitmap(pname);
                }
                catch { pack.whiteKeyLeftTex = null; }
                try
                {
                    pname = data.whiteKeyLeftPressed;
                    pack.whiteKeyPressedLeftTex = GetBitmap(pname);
                }
                catch { pack.whiteKeyPressedLeftTex = null; }

                try
                {
                    pname = data.whiteKeyRight;
                    pack.whiteKeyRightTex = GetBitmap(pname);
                }
                catch { pack.whiteKeyRightTex = null; }
                try
                {
                    pname = data.whiteKeyRightPressed;
                    pack.whiteKeyPressedRightTex = GetBitmap(pname);
                }
                catch { pack.whiteKeyPressedRightTex = null; }

                if ((pack.whiteKeyLeftTex == null) ^ (pack.whiteKeyPressedLeftTex == null))
                    if (pack.whiteKeyLeftTex == null)
                        throw new Exception("whiteKeyLeft is incliuded while whiteKeyLeftPressed is missing. Include or remove both.");
                    else
                        throw new Exception("whiteKeyLeftPressed is incliuded while whiteKeyLeft is missing. Include or remove both.");

                if ((pack.whiteKeyRightTex == null) ^ (pack.whiteKeyPressedRightTex == null))
                    if (pack.whiteKeyRightTex == null)
                        throw new Exception("whiteKeyRight is incliuded while whiteKeyRightPressed is missing. Include or remove both.");
                    else
                        throw new Exception("whiteKeyRightPressed is incliuded while whiteKeyRight is missing. Include or remove both.");

                #endregion

                #region Oversizes
                try
                {
                    pack.whiteKeyOversize = (double)data.whiteKeyOversize / 100;
                }
                catch { }
                try
                {
                    pack.blackKeyOversize = (double)data.blackKeyOversize / 100;
                }
                catch { }
                try
                {
                    pack.whiteKeyPressedOversize = (double)data.whiteKeyPressedOversize / 100;
                }
                catch { }
                try
                {
                    pack.blackKeyPressedOversize = (double)data.blackKeyPressedOversize / 100;
                }
                catch { }
                #endregion

                #region Bar
                try
                {
                    pname = data.bar;
                    pack.barTex = GetBitmap(pname);
                    pack.useBar = true;
                }
                catch { }

                if (pack.useBar)
                {
                    try
                    {
                        pack.barHeight = (double)data.barHeight / 100;
                    }
                    catch { }
                }
                #endregion

                #region Notes
                JArray noteSizes;
                try
                {
                    noteSizes = data.notes;
                }
                catch { throw new Exception("Missing Array Property \"notes\""); }
                if (noteSizes.Count == 0) throw new Exception("Note textures array can't be 0");
                if (noteSizes.Count > 4) throw new Exception("Only up to 4 note textures are supported");

                List<NoteTexture> noteTex = new List<NoteTexture>();
                bool hasBothKeyType = false;
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
                    tex.noteMiddleTex = GetBitmap(pname);
                    tex.noteMiddleAspect = (double)tex.noteMiddleTex.Height / tex.noteMiddleTex.Width;

                    try
                    {
                        tex.darkenBlackNotes = (double)s.darkenBlackNotes;
                    }
                    catch { }

                    try
                    {
                        tex.highlightHitNotes = (double)s.highlightHitNotes;
                    }
                    catch { }
                    if (tex.highlightHitNotes > 1 || tex.highlightHitNotes < 0) throw new Exception("highlightHitNotes must be between 0 and 1");

                    JArray array = null;
                    bool notArray = false;
                    try
                    {
                        var _array = s.highlightHitNotesColor;
                        if (_array != null)
                        {
                            notArray = _array.GetType() != typeof(JArray);
                            if (!notArray) array = _array;
                        }
                    }
                    catch { if (notArray) throw new Exception("highlightHitNotes must be an array of 3 numbers (RGB or RGBA, e.g. [255, 255, 255, 100])"); }
                    if (array != null)
                    {
                        if (!(array.Count == 3)) throw new Exception("highlightHitNotes must be an array of 3-4 numbers (RGB or RGBA, e.g. [255, 255, 255, 100])");
                        else
                        {
                            tex.highlightHitNotesColor = System.Drawing.Color.FromArgb(255, (int)array[0], (int)array[1], (int)array[2]);
                        }
                    }

                    try
                    {
                        tex.squeezeEndCaps = (bool)s.squeezeEndCaps;
                    }
                    catch { }

                    string keyType = null;
                    try
                    {
                        keyType = (string)s.keyType;
                    }
                    catch { }
                    if (keyType != null) tex.keyType = strToKeyType(keyType);
                    if (tex.keyType == KeyType.Both) hasBothKeyType = true;

                    if (tex.useCaps)
                    {
                        try
                        {
                            pname = s.topTexture;
                        }
                        catch { throw new Exception("Missing property \"topTexture\""); }
                        tex.noteTopTex = GetBitmap(pname);
                        try
                        {
                            pname = s.bottomTexture;
                        }
                        catch { throw new Exception("Missing property \"bottomTexture\""); }
                        tex.noteBottomTex = GetBitmap(pname);
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

                if (!hasBothKeyType) throw new Exception("At least one note texture required with key type of \"both\"");

                noteTex.Sort((c1, c2) =>
                {
                    if (c1.maxSize < c2.maxSize) return -1;
                    if (c1.maxSize > c2.maxSize) return 1;
                    if (c2.keyType == KeyType.Both && c1.keyType != KeyType.Both) return -1;
                    if (c1.keyType == KeyType.Both && c2.keyType != KeyType.Both) return 1;
                    return 0;
                });
                bool firstBoth = false;
                for (int i = noteTex.Count - 1; i >= 0; i--)
                {
                    if (noteTex[i].keyType == KeyType.Both)
                    {
                        if (firstBoth) break;
                        else firstBoth = true;
                    }
                    noteTex[i].maxSize = double.PositiveInfinity;
                }
                pack.NoteTextures = noteTex.ToArray();
                #endregion
            }
            catch (Exception e)
            {
                pack.error = true;
                pack.description = e.Message;
            }
            finally
            {
                if (archive != null)
                    archive.Dispose();
                if (zrpstream != null)
                    zrpstream.Dispose();
            }
            return pack;
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
