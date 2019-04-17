using BMEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

namespace TexturedRender
{
    public class Render : IPluginRender
    {
        #region PreviewConvert
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
        #endregion

        public string Name => "Textured";

        public string Description => "Plugin for loading and rendering custom resource packs, " +
            "with settings defined in a .json file";

        #region Shaders
        string quadShaderVert = @"#version 330 core

layout(location=0) in vec2 in_position;
layout(location=1) in vec4 in_color;
layout(location=2) in vec2 in_uv;
layout(location=3) in float in_texid;

out vec4 v2f_color;
out vec2 uv;
out float texid;

void main()
{
    gl_Position = vec4(in_position.x * 2 - 1, in_position.y * 2 - 1, 1.0f, 1.0f);
    v2f_color = in_color;
    uv = in_uv;
    texid = in_texid;
}
";
        string quadShaderFrag = @"#version 330 core

in vec4 v2f_color;
in vec2 uv;
in float texid;

uniform sampler2D textureSampler1;
uniform sampler2D textureSampler2;
uniform sampler2D textureSampler3;
uniform sampler2D textureSampler4;
uniform sampler2D textureSampler5;
uniform sampler2D textureSampler6;
uniform sampler2D textureSampler7;
uniform sampler2D textureSampler8;
uniform sampler2D textureSampler9;
uniform sampler2D textureSampler10;
uniform sampler2D textureSampler11;
uniform sampler2D textureSampler12;

out vec4 out_color;

void main()
{
    vec4 col;
    if(texid < 0.5) col = texture2D( textureSampler1, uv );
    else if(texid < 1.5) col = texture2D( textureSampler2, uv );
    else if(texid < 2.5) col = texture2D( textureSampler3, uv );
    else if(texid < 3.5) col = texture2D( textureSampler4, uv );
    else if(texid < 4.5) col = texture2D( textureSampler5, uv );
    else if(texid < 5.5) col = texture2D( textureSampler6, uv );
    else if(texid < 6.5) col = texture2D( textureSampler7, uv );
    else if(texid < 7.5) col = texture2D( textureSampler8, uv );
    else if(texid < 8.5) col = texture2D( textureSampler9, uv );
    else if(texid < 9.5) col = texture2D( textureSampler10, uv );
    else if(texid < 10.5) col = texture2D( textureSampler11, uv );
    else if(texid < 11.5) col = texture2D( textureSampler12, uv );
    out_color = col * v2f_color;
}
";
        string invertQuadShaderFrag = @"#version 330 core

in vec4 v2f_color;
in vec2 uv;
in float texid;

uniform sampler2D textureSampler1;
uniform sampler2D textureSampler2;
uniform sampler2D textureSampler3;
uniform sampler2D textureSampler4;
uniform sampler2D textureSampler5;
uniform sampler2D textureSampler6;
uniform sampler2D textureSampler7;
uniform sampler2D textureSampler8;
uniform sampler2D textureSampler9;
uniform sampler2D textureSampler10;
uniform sampler2D textureSampler11;
uniform sampler2D textureSampler12;

out vec4 out_color;

void main()
{
    vec4 col;
    if(texid < 0.5) col = texture2D( textureSampler1, uv );
    else if(texid < 1.5) col = texture2D( textureSampler2, uv );
    else if(texid < 2.5) col = texture2D( textureSampler3, uv );
    else if(texid < 3.5) col = texture2D( textureSampler4, uv );
    else if(texid < 4.5) col = texture2D( textureSampler5, uv );
    else if(texid < 5.5) col = texture2D( textureSampler6, uv );
    else if(texid < 6.5) col = texture2D( textureSampler7, uv );
    else if(texid < 7.5) col = texture2D( textureSampler8, uv );
    else if(texid < 8.5) col = texture2D( textureSampler9, uv );
    else if(texid < 9.5) col = texture2D( textureSampler10, uv );
    else if(texid < 10.5) col = texture2D( textureSampler11, uv );
    else if(texid < 11.5) col = texture2D( textureSampler12, uv );
    col = 1 - col;
    col.w = 1 - col.w;
    vec4 col2 = 1 - v2f_color;
    col2.w = 1 - col2.w;
    out_color = 1 - col * col2;
    out_color.w = 1 - out_color.w;
}
";
        string evenQuadShaderFrag = @"#version 330 core

in vec4 v2f_color;
in vec2 uv;
in float texid;

uniform sampler2D textureSampler1;
uniform sampler2D textureSampler2;
uniform sampler2D textureSampler3;
uniform sampler2D textureSampler4;
uniform sampler2D textureSampler5;
uniform sampler2D textureSampler6;
uniform sampler2D textureSampler7;
uniform sampler2D textureSampler8;
uniform sampler2D textureSampler9;
uniform sampler2D textureSampler10;
uniform sampler2D textureSampler11;
uniform sampler2D textureSampler12;

out vec4 out_color;

void main()
{
    vec4 col;
    if(texid < 0.5) col = texture2D( textureSampler1, uv );
    else if(texid < 1.5) col = texture2D( textureSampler2, uv );
    else if(texid < 2.5) col = texture2D( textureSampler3, uv );
    else if(texid < 3.5) col = texture2D( textureSampler4, uv );
    else if(texid < 4.5) col = texture2D( textureSampler5, uv );
    else if(texid < 5.5) col = texture2D( textureSampler6, uv );
    else if(texid < 6.5) col = texture2D( textureSampler7, uv );
    else if(texid < 7.5) col = texture2D( textureSampler8, uv );
    else if(texid < 8.5) col = texture2D( textureSampler9, uv );
    else if(texid < 9.5) col = texture2D( textureSampler10, uv );
    else if(texid < 10.5) col = texture2D( textureSampler11, uv );
    else if(texid < 11.5) col = texture2D( textureSampler12, uv );
    col = col * 2;
    if(col.x > 1){
        out_color.x = 1 - (2 - col.x) * (1 - v2f_color.x);
    }
    else out_color.x = col.x * v2f_color.x;
    if(col.y > 1){
        out_color.y = 1 - (2 - col.y) * (1 - v2f_color.y);
    }
    else out_color.y = col.y * v2f_color.y;
    if(col.z > 1){
        out_color.z = 1 - (2 - col.z) * (1 - v2f_color.z);
    }
    else out_color.z = col.z * v2f_color.z;
    out_color.w = col.w * v2f_color.w;
}
";

        int MakeShader(string vert, string frag)
        {
            int _vertexObj = GL.CreateShader(ShaderType.VertexShader);
            int _fragObj = GL.CreateShader(ShaderType.FragmentShader);
            int statusCode;
            string info;

            GL.ShaderSource(_vertexObj, vert);
            GL.CompileShader(_vertexObj);
            info = GL.GetShaderInfoLog(_vertexObj);
            GL.GetShader(_vertexObj, ShaderParameter.CompileStatus, out statusCode);
            if (statusCode != 1) throw new ApplicationException(info);

            GL.ShaderSource(_fragObj, frag);
            GL.CompileShader(_fragObj);
            info = GL.GetShaderInfoLog(_fragObj);
            GL.GetShader(_fragObj, ShaderParameter.CompileStatus, out statusCode);
            if (statusCode != 1) throw new ApplicationException(info);

            int shader = GL.CreateProgram();
            GL.AttachShader(shader, _fragObj);
            GL.AttachShader(shader, _vertexObj);
            GL.LinkProgram(shader);
            return shader;
        }
        #endregion

        void loadImage(Bitmap image, int texID, bool loop, bool linear)
        {
            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            if (linear)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }
            if (loop)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }

            image.UnlockBits(data);
        }

        public bool Initialized { get; set; }

        public System.Windows.Media.ImageSource PreviewImage { get; set; } = null;

        public bool ManualNoteDelete => false;

        public int NoteCollectorOffset => 0;

        public double LastMidiTimePerTick { get; set; }
        public MidiInfo CurrentMidi { get; set; }

        public double NoteScreenTime => settings.deltaTimeOnScreen;

        public long LastNoteCount { get; set; }

        public System.Windows.Controls.Control SettingsControl { get; set; } = null;

        int quadShader;
        int evenquadShader;
        int inverseQuadShader;

        int vertexBufferID;
        int colorBufferID;
        int uvBufferID;
        int texIDBufferID;

        int quadBufferLength = 2048 * 64;
        double[] quadVertexbuff;
        float[] quadColorbuff;
        double[] quadUVbuff;
        float[] quadTexIDbuff;
        int quadBufferPos = 0;

        RenderSettings renderSettings;
        Settings settings;

        int indexBufferId;
        uint[] indexes = new uint[2048 * 128 * 6];

        bool[] blackKeys = new bool[257];
        int[] keynum = new int[257];

        public Pack currPack = null;
        public long lastPackChangeTime = 0;

        public void UnloadPack()
        {
            if (currPack == null) return;
            GL.DeleteTextures(4, new int[] {
                currPack.whiteKeyTexID, currPack.whiteKeyPressedTexID,
                currPack.blackKeyTexID, currPack.blackKeyPressedTexID
            });
            if (currPack.useBar) GL.DeleteTexture(currPack.barTexID);
            foreach (var n in currPack.NoteTextures)
            {
                GL.DeleteTexture(n.noteMiddleTexID);
                if (n.useCaps)
                {
                    GL.DeleteTexture(n.noteBottomTexID);
                    GL.DeleteTexture(n.noteTopTexID);
                }
            }
        }

        public void LoadPack()
        {
            if (currPack == null) return;
            currPack.whiteKeyTexID = GL.GenTexture();
            currPack.whiteKeyPressedTexID = GL.GenTexture();
            currPack.blackKeyTexID = GL.GenTexture();
            currPack.blackKeyPressedTexID = GL.GenTexture();
            if (currPack.useBar) currPack.barTexID = GL.GenTexture();

            loadImage(currPack.whiteKeyTex, currPack.whiteKeyTexID, false, true);
            loadImage(currPack.whiteKeyPressedTex, currPack.whiteKeyPressedTexID, false, true);
            loadImage(currPack.blackKeyTex, currPack.blackKeyTexID, false, false);
            loadImage(currPack.blackKeyPressedTex, currPack.blackKeyPressedTexID, false, false);
            if (currPack.useBar) loadImage(currPack.barTex, currPack.barTexID, false, true);

            foreach (var n in currPack.NoteTextures)
            {
                n.noteMiddleTexID = GL.GenTexture();
                loadImage(n.noteMiddleTex, n.noteMiddleTexID, true, true);
                if (n.useCaps)
                {
                    n.noteTopTexID = GL.GenTexture();
                    loadImage(n.noteTopTex, n.noteTopTexID, false, true);
                    n.noteBottomTexID = GL.GenTexture();
                    loadImage(n.noteBottomTex, n.noteBottomTexID, false, true);
                }
            }
        }

        public void CheckPack()
        {
            if (settings.lastPackChangeTime != lastPackChangeTime)
            {
                UnloadPack();
                currPack = settings.currPack;
                lastPackChangeTime = settings.lastPackChangeTime;
                LoadPack();
            }
        }

        public void Dispose()
        {
            GL.DeleteBuffers(3, new int[] { vertexBufferID, colorBufferID, uvBufferID });

            GL.DeleteProgram(quadShader);
            quadVertexbuff = null;
            quadColorbuff = null;
            quadUVbuff = null;
            Initialized = false;
            UnloadPack();
            Console.WriteLine("Disposed of TextureRender");
        }

        public Render(RenderSettings settings)
        {
            this.settings = new Settings();
            this.renderSettings = settings;
            SettingsControl = new SettingsCtrl(this.settings);
            PreviewImage = BitmapToImageSource(Properties.Resources.pluginPreview);

            for (int i = 0; i < blackKeys.Length; i++) blackKeys[i] = isBlackNote(i);
            int b = 0;
            int w = 0;
            for (int i = 0; i < keynum.Length; i++)
            {
                if (blackKeys[i]) keynum[i] = b++;
                else keynum[i] = w++;
            }
        }

        public void Init()
        {
            quadShader = MakeShader(quadShaderVert, quadShaderFrag);
            inverseQuadShader = MakeShader(quadShaderVert, invertQuadShaderFrag);
            evenquadShader = MakeShader(quadShaderVert, evenQuadShaderFrag);

            int loc;
            int[] samplers = new int[12];
            for (int i = 0; i < 12; i++)
            {
                samplers[i] = i;
            }

            GL.UseProgram(quadShader);
            for(int i = 0; i < 12; i++)
            {
                loc = GL.GetUniformLocation(quadShader, "textureSampler" + (i + 1));
                GL.Uniform1(loc, i);
            }
            GL.UseProgram(inverseQuadShader);
            for (int i = 0; i < 12; i++)
            {
                loc = GL.GetUniformLocation(inverseQuadShader, "textureSampler" + (i + 1));
                GL.Uniform1(loc, i);
            }
            GL.UseProgram(evenquadShader);
            for (int i = 0; i < 12; i++)
            {
                loc = GL.GetUniformLocation(evenquadShader, "textureSampler" + (i + 1));
                GL.Uniform1(loc, i);
            }

            quadVertexbuff = new double[quadBufferLength * 8];
            quadColorbuff = new float[quadBufferLength * 16];
            quadUVbuff = new double[quadBufferLength * 8];
            quadTexIDbuff = new float[quadBufferLength * 4];

            LoadPack();

            GL.GenBuffers(1, out vertexBufferID);
            GL.GenBuffers(1, out colorBufferID);
            GL.GenBuffers(1, out uvBufferID);
            GL.GenBuffers(1, out texIDBufferID);
            for (uint i = 0; i < indexes.Length / 6; i++)
            {
                indexes[i * 6 + 0] = i * 4 + 0;
                indexes[i * 6 + 1] = i * 4 + 1;
                indexes[i * 6 + 2] = i * 4 + 3;
                indexes[i * 6 + 3] = i * 4 + 1;
                indexes[i * 6 + 4] = i * 4 + 3;
                indexes[i * 6 + 5] = i * 4 + 2;
            }

            GL.GenBuffers(1, out indexBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(indexes.Length * 4),
                indexes,
                BufferUsageHint.StaticDraw);
            Initialized = true;
            Console.WriteLine("Initialised TextureRender");
        }

        Color4[] keyColors = new Color4[514];
        double[] x1array = new double[257];
        double[] wdtharray = new double[257];

        void SwitchShader(TextureShaderType shader)
        {
            if (shader == TextureShaderType.Normal) GL.UseProgram(quadShader);
            if (shader == TextureShaderType.Inverted) GL.UseProgram(inverseQuadShader);
            if (shader == TextureShaderType.Hybrid) GL.UseProgram(evenquadShader);
        }

        public void RenderFrame(FastList<Note> notes, double midiTime, int finalCompositeBuff)
        {
            CheckPack();
            if (currPack == null) return;
            GL.Enable(EnableCap.Blend);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, finalCompositeBuff);
            GL.Viewport(0, 0, renderSettings.width, renderSettings.height);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            #region Vars
            long nc = 0;
            int firstNote = settings.firstNote;
            int lastNote = settings.lastNote;
            int kbfirstNote = settings.firstNote;
            int kblastNote = settings.lastNote;
            if (blackKeys[firstNote]) kbfirstNote--;
            kblastNote++;

            double deltaTimeOnScreen = NoteScreenTime;
            double keyboardHeightFull = currPack.keyboardHeight / (lastNote - firstNote) * 128;
            double keyboardHeight = keyboardHeightFull;
            double barHeight = keyboardHeightFull * currPack.barHeight;
            if (currPack.useBar) keyboardHeight -= barHeight;
            bool sameWidth = currPack.sameWidthNotes;
            double viewAspect = (double)renderSettings.width / renderSettings.height;
            for (int i = 0; i < 514; i++) keyColors[i] = Color4.Transparent;
            double wdth;
            float r, g, b, a, r2, g2, b2, a2;
            double x1;
            double x2;
            double y1;
            double y2;
            int pos;
            quadBufferPos = 0;

            if (sameWidth)
            {
                for (int i = 0; i < 257; i++)
                {
                    x1array[i] = (float)(i - firstNote) / (lastNote - firstNote);
                    wdtharray[i] = 1.0f / (lastNote - firstNote);
                }
            }
            else
            {
                double knmfn = keynum[firstNote];
                double knmln = keynum[lastNote - 1];
                if (blackKeys[firstNote]) knmfn = keynum[firstNote - 1] + 0.5;
                if (blackKeys[lastNote - 1]) knmln = keynum[lastNote] - 0.5;
                for (int i = 0; i < 257; i++)
                {
                    if (!blackKeys[i])
                    {
                        x1array[i] = (float)(keynum[i] - knmfn) / (knmln - knmfn + 1);
                        wdtharray[i] = 1.0f / (knmln - knmfn + 1);
                    }
                    else
                    {
                        int _i = i + 1;
                        wdth = 0.6f / (knmln - knmfn + 1);
                        int bknum = keynum[i] % 5;
                        double offset = wdth / 2;
                        if(bknum == 0) offset += offset * 0.3;
                        if(bknum == 2) offset += offset * 0.5;
                        if (bknum == 1) offset -= offset * 0.3;
                        if (bknum == 4) offset -= offset * 0.5;

                        //if (bknum == 0 || bknum == 2)
                        //{
                        //    offset *= 1.3;
                        //}
                        //else if (bknum == 1 || bknum == 4)
                        //{
                        //    offset *= 0.7;
                        //}
                        x1array[i] = (float)(keynum[_i] - knmfn) / (knmln - knmfn + 1) - offset;
                        wdtharray[i] = wdth;
                    }
                }
            }

            #endregion

            #region Notes
            quadBufferPos = 0;
            double notePosFactor = 1 / deltaTimeOnScreen * (1 - keyboardHeightFull);
            double renderCutoff = midiTime + deltaTimeOnScreen;

            var currNoteTex = currPack.NoteTextures[0];
            var noteTextures = currPack.NoteTextures;
            GL.BindTexture(TextureTarget.Texture2D, currNoteTex.noteMiddleTexID);
            for (int i = 0; i < noteTextures.Length; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + (i * 3));
                GL.BindTexture(TextureTarget.Texture2D, noteTextures[i].noteMiddleTexID);
                if (noteTextures[i].useCaps)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + (i * 3) + 1);
                    GL.BindTexture(TextureTarget.Texture2D, noteTextures[i].noteBottomTexID);
                    GL.ActiveTexture(TextureUnit.Texture0 + (i * 3) + 2);
                    GL.BindTexture(TextureTarget.Texture2D, noteTextures[i].noteTopTexID);
                }
            }
            SwitchShader(currPack.noteShader);
            for (int i = 0; i < 2; i++)
            {
                bool black = false;
                bool blackabove = settings.blackNotesAbove;
                if (blackabove && i == 1) black = true;
                if (!blackabove && i == 1) break;
                foreach (Note n in notes)
                {
                    if (n.end >= midiTime || !n.hasEnded)
                    {
                        if (n.start < renderCutoff)
                        {
                            nc++;
                            int k = n.note;
                            if (blackabove && !black && blackKeys[k]) continue;
                            if (blackabove && black && !blackKeys[k]) continue;
                            if (!(k >= firstNote && k < lastNote)) continue;
                            Color4 coll = n.color.left;
                            Color4 colr = n.color.right;
                            if (n.start <= midiTime)
                            {
                                Color4 origcoll = keyColors[k * 2];
                                Color4 origcolr = keyColors[k * 2 + 1];
                                float blendfac = coll.A;
                                float revblendfac = 1 - blendfac;
                                keyColors[k * 2] = new Color4(
                                    coll.R * blendfac + origcoll.R * revblendfac,
                                    coll.G * blendfac + origcoll.G * revblendfac,
                                    coll.B * blendfac + origcoll.B * revblendfac,
                                    1);
                                blendfac = colr.A * 0.8f;
                                revblendfac = 1 - blendfac;
                                keyColors[k * 2 + 1] = new Color4(
                                    colr.R * blendfac + origcolr.R * revblendfac,
                                    colr.G * blendfac + origcolr.G * revblendfac,
                                    colr.B * blendfac + origcolr.B * revblendfac,
                                    1);
                            }
                            x1 = x1array[k];
                            wdth = wdtharray[k];
                            x2 = x1 + wdth;
                            y1 = 1 - (renderCutoff - n.end) * notePosFactor;
                            y2 = 1 - (renderCutoff - n.start) * notePosFactor;
                            if (!n.hasEnded)
                                y1 = 1;
                            double texSize = (y1 - y2) / wdth / viewAspect;
                            NoteTexture ntex = null;
                            int tex = 0;
                            foreach (var t in noteTextures)
                            {
                                if (t.maxSize > texSize)
                                {
                                    ntex = t;
                                    break;
                                }
                                tex++;
                            }
                            tex *= 3;

                            double topHeight;
                            double bottomHeight;
                            double yy1 = 0, yy2 = 0;
                            if (ntex.useCaps)
                            {
                                topHeight = wdth * ntex.noteTopAspect * viewAspect;
                                bottomHeight = wdth * ntex.noteBottomAspect * viewAspect;
                                yy1 = y1 + topHeight * ntex.noteTopOversize;
                                yy2 = y2 - bottomHeight * ntex.noteBottomOversize;
                                if (n.hasEnded)
                                    y1 -= topHeight * (1 - ntex.noteTopOversize);
                                y2 += bottomHeight * (1 - ntex.noteBottomOversize);
                                if (y2 > y1)
                                {
                                    double middley = (y2 + y1) / 2;
                                    y1 = middley;
                                    y2 = middley;
                                }
                                texSize = (y1 - y2) / wdth / viewAspect;
                            }

                            if (ntex.stretch)
                                texSize = 1;
                            else
                                texSize /= currNoteTex.noteMiddleAspect;
                            if (n.hasEnded)
                                texSize = -Math.Round(texSize);
                            if (texSize == 0) texSize = -1;
                            pos = quadBufferPos * 8;
                            quadVertexbuff[pos++] = x2;
                            quadVertexbuff[pos++] = y2;
                            quadVertexbuff[pos++] = x2;
                            quadVertexbuff[pos++] = y1;
                            quadVertexbuff[pos++] = x1;
                            quadVertexbuff[pos++] = y1;
                            quadVertexbuff[pos++] = x1;
                            quadVertexbuff[pos++] = y2;

                            pos = quadBufferPos * 16;
                            if (black)
                            {
                                float multiply = (float)currNoteTex.darkenBlackNotes;
                                r = coll.R * multiply;
                                g = coll.G * multiply;
                                b = coll.B * multiply;
                                a = coll.A;
                                r2 = colr.R * multiply;
                                g2 = colr.G * multiply;
                                b2 = colr.B * multiply;
                                a2 = colr.A;
                            }
                            else
                            {
                                r = coll.R;
                                g = coll.G;
                                b = coll.B;
                                a = coll.A;
                                r2 = colr.R;
                                g2 = colr.G;
                                b2 = colr.B;
                                a2 = colr.A;
                            }
                            quadColorbuff[pos++] = r;
                            quadColorbuff[pos++] = g;
                            quadColorbuff[pos++] = b;
                            quadColorbuff[pos++] = a;
                            quadColorbuff[pos++] = r;
                            quadColorbuff[pos++] = g;
                            quadColorbuff[pos++] = b;
                            quadColorbuff[pos++] = a;
                            quadColorbuff[pos++] = r2;
                            quadColorbuff[pos++] = g2;
                            quadColorbuff[pos++] = b2;
                            quadColorbuff[pos++] = a2;
                            quadColorbuff[pos++] = r2;
                            quadColorbuff[pos++] = g2;
                            quadColorbuff[pos++] = b2;
                            quadColorbuff[pos++] = a2;

                            pos = quadBufferPos * 8;
                            quadUVbuff[pos++] = 1;
                            quadUVbuff[pos++] = 0;
                            quadUVbuff[pos++] = 1;
                            quadUVbuff[pos++] = texSize;
                            quadUVbuff[pos++] = 0;
                            quadUVbuff[pos++] = texSize;
                            quadUVbuff[pos++] = 0;
                            quadUVbuff[pos++] = 0;

                            pos = quadBufferPos * 4;
                            quadTexIDbuff[pos++] = tex;
                            quadTexIDbuff[pos++] = tex;
                            quadTexIDbuff[pos++] = tex;
                            quadTexIDbuff[pos++] = tex;

                            quadBufferPos++;
                            FlushQuadBuffer();

                            if (ntex.useCaps)
                            {
                                pos = quadBufferPos * 8;
                                quadVertexbuff[pos++] = x2;
                                quadVertexbuff[pos++] = yy2;
                                quadVertexbuff[pos++] = x2;
                                quadVertexbuff[pos++] = y2;
                                quadVertexbuff[pos++] = x1;
                                quadVertexbuff[pos++] = y2;
                                quadVertexbuff[pos++] = x1;
                                quadVertexbuff[pos++] = yy2;

                                pos = quadBufferPos * 16;
                                quadColorbuff[pos++] = r;
                                quadColorbuff[pos++] = g;
                                quadColorbuff[pos++] = b;
                                quadColorbuff[pos++] = a;
                                quadColorbuff[pos++] = r;
                                quadColorbuff[pos++] = g;
                                quadColorbuff[pos++] = b;
                                quadColorbuff[pos++] = a;
                                quadColorbuff[pos++] = r2;
                                quadColorbuff[pos++] = g2;
                                quadColorbuff[pos++] = b2;
                                quadColorbuff[pos++] = a2;
                                quadColorbuff[pos++] = r2;
                                quadColorbuff[pos++] = g2;
                                quadColorbuff[pos++] = b2;
                                quadColorbuff[pos++] = a2;

                                pos = quadBufferPos * 8;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 1;

                                pos = quadBufferPos * 4;
                                tex++;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;

                                quadBufferPos++;
                                FlushQuadBuffer();

                                pos = quadBufferPos * 8;
                                quadVertexbuff[pos++] = x2;
                                quadVertexbuff[pos++] = y1;
                                quadVertexbuff[pos++] = x2;
                                quadVertexbuff[pos++] = yy1;
                                quadVertexbuff[pos++] = x1;
                                quadVertexbuff[pos++] = yy1;
                                quadVertexbuff[pos++] = x1;
                                quadVertexbuff[pos++] = y1;

                                pos = quadBufferPos * 16;
                                quadColorbuff[pos++] = r;
                                quadColorbuff[pos++] = g;
                                quadColorbuff[pos++] = b;
                                quadColorbuff[pos++] = a;
                                quadColorbuff[pos++] = r;
                                quadColorbuff[pos++] = g;
                                quadColorbuff[pos++] = b;
                                quadColorbuff[pos++] = a;
                                quadColorbuff[pos++] = r2;
                                quadColorbuff[pos++] = g2;
                                quadColorbuff[pos++] = b2;
                                quadColorbuff[pos++] = a2;
                                quadColorbuff[pos++] = r2;
                                quadColorbuff[pos++] = g2;
                                quadColorbuff[pos++] = b2;
                                quadColorbuff[pos++] = a2;

                                pos = quadBufferPos * 8;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 1;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 0;
                                quadUVbuff[pos++] = 1;

                                pos = quadBufferPos * 4;
                                tex++;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;
                                quadTexIDbuff[pos++] = tex;

                                quadBufferPos++;
                                FlushQuadBuffer();
                            }

                        }
                        else break;
                    }

                }
            }
            FlushQuadBuffer(false);
            quadBufferPos = 0;

            LastNoteCount = nc;
            #endregion

            #region Keyboard

            GL.UseProgram(quadShader);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, currPack.barTexID);
            pos = quadBufferPos * 8;
            quadVertexbuff[pos++] = 0;
            quadVertexbuff[pos++] = keyboardHeightFull;
            quadVertexbuff[pos++] = 1;
            quadVertexbuff[pos++] = keyboardHeightFull;
            quadVertexbuff[pos++] = 1;
            quadVertexbuff[pos++] = keyboardHeight;
            quadVertexbuff[pos++] = 0;
            quadVertexbuff[pos++] = keyboardHeight;

            pos = quadBufferPos * 16;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;
            quadColorbuff[pos++] = 1;

            pos = quadBufferPos * 8;
            quadUVbuff[pos++] = 0;
            quadUVbuff[pos++] = 0;
            quadUVbuff[pos++] = 1;
            quadUVbuff[pos++] = 0;
            quadUVbuff[pos++] = 1;
            quadUVbuff[pos++] = 1;
            quadUVbuff[pos++] = 0;
            quadUVbuff[pos++] = 1;

            pos = quadBufferPos * 4;
            quadTexIDbuff[pos++] = 0;
            quadTexIDbuff[pos++] = 0;
            quadTexIDbuff[pos++] = 0;
            quadTexIDbuff[pos++] = 0;
            quadBufferPos++;
            FlushQuadBuffer(false);

            y1 = keyboardHeight;
            y2 = 0;
            Color4[] origColors = new Color4[257];
            SwitchShader(currPack.whiteKeyShader);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, currPack.whiteKeyPressedTexID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, currPack.whiteKeyTexID);
            for (int k = kbfirstNote; k < kblastNote; k++)
            {
                if (isBlackNote(k))
                    origColors[k] = Color4.Black;
                else
                    origColors[k] = Color4.White;
            }
            
            float pressed;
            for (int n = kbfirstNote; n < kblastNote; n++)
            {
                x1 = x1array[n];
                wdth = wdtharray[n];
                x2 = x1 + wdth;

                if (!blackKeys[n])
                {
                    y2 = 0;
                    if (sameWidth)
                    {
                        int _n = n % 12;
                        if (_n == 0)
                            x2 += wdth * 0.666;
                        else if (_n == 2)
                        {
                            x1 -= wdth / 3;
                            x2 += wdth / 3;
                        }
                        else if (_n == 4)
                            x1 -= wdth / 3 * 2;
                        else if (_n == 5)
                            x2 += wdth * 0.75;
                        else if (_n == 7)
                        {
                            x1 -= wdth / 4;
                            x2 += wdth / 2;
                        }
                        else if (_n == 9)
                        {
                            x1 -= wdth / 2;
                            x2 += wdth / 4;
                        }
                        else if (_n == 11)
                            x1 -= wdth * 0.75;
                    }
                }
                else continue;

                var coll = keyColors[n * 2];
                var colr = keyColors[n * 2 + 1];
                var origcol = origColors[n];
                float blendfac1 = coll.A;
                float revblendfac = 1 - blendfac1;
                coll = new Color4(
                    coll.R * blendfac1 + origcol.R * revblendfac,
                    coll.G * blendfac1 + origcol.G * revblendfac,
                    coll.B * blendfac1 + origcol.B * revblendfac,
                    1);
                r = coll.R;
                g = coll.G;
                b = coll.B;
                a = coll.A;
                float blendfac2 = coll.A;
                blendfac2 = colr.A;
                revblendfac = 1 - blendfac2;
                colr = new Color4(
                    colr.R * blendfac2 + origcol.R * revblendfac,
                    colr.G * blendfac2 + origcol.G * revblendfac,
                    colr.B * blendfac2 + origcol.B * revblendfac,
                    1);
                r2 = colr.R;
                g2 = colr.G;
                b2 = colr.B;
                a2 = colr.A;
                if (blendfac1 + blendfac2 != 0) pressed = 1;
                else pressed = 0;

                pos = quadBufferPos * 8;
                quadVertexbuff[pos++] = x1;
                quadVertexbuff[pos++] = y2;
                quadVertexbuff[pos++] = x2;
                quadVertexbuff[pos++] = y2;
                quadVertexbuff[pos++] = x2;
                quadVertexbuff[pos++] = y1;
                quadVertexbuff[pos++] = x1;
                quadVertexbuff[pos++] = y1;

                pos = quadBufferPos * 16;
                quadColorbuff[pos++] = r;
                quadColorbuff[pos++] = g;
                quadColorbuff[pos++] = b;
                quadColorbuff[pos++] = a;
                quadColorbuff[pos++] = r;
                quadColorbuff[pos++] = g;
                quadColorbuff[pos++] = b;
                quadColorbuff[pos++] = a;
                quadColorbuff[pos++] = r2;
                quadColorbuff[pos++] = g2;
                quadColorbuff[pos++] = b2;
                quadColorbuff[pos++] = a2;
                quadColorbuff[pos++] = r2;
                quadColorbuff[pos++] = g2;
                quadColorbuff[pos++] = b2;
                quadColorbuff[pos++] = a2;

                if (!currPack.whiteKeysFullOctave)
                {
                    pos = quadBufferPos * 8;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 0;
                }
                else
                {
                    var k = keynum[n] % 7;
                    double uvl = k / 7.0;
                    double uvr = (k + 1) / 7.0;
                    pos = quadBufferPos * 8;
                    quadUVbuff[pos++] = uvl;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = uvr;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = uvr;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = uvl;
                    quadUVbuff[pos++] = 0;
                }

                pos = quadBufferPos * 4;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;
                quadBufferPos++;
                FlushQuadBuffer();
            }
            FlushQuadBuffer(false);
            SwitchShader(currPack.blackKeyShader);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, currPack.blackKeyPressedTexID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, currPack.blackKeyTexID);
            for (int n = kbfirstNote; n < kblastNote; n++)
            {
                x1 = x1array[n];
                wdth = wdtharray[n];
                x2 = x1 + wdth;

                if (blackKeys[n])
                {
                    y2 = keyboardHeight * currPack.blackKeyHeight;
                }
                else continue;

                var coll = keyColors[n * 2];
                var colr = keyColors[n * 2 + 1];
                var origcol = origColors[n];
                float blendfac1 = coll.A;
                float revblendfac = 1 - blendfac1;
                coll = new Color4(
                    coll.R * blendfac1 + origcol.R * revblendfac,
                    coll.G * blendfac1 + origcol.G * revblendfac,
                    coll.B * blendfac1 + origcol.B * revblendfac,
                    1);
                r = coll.R;
                g = coll.G;
                b = coll.B;
                a = coll.A;
                float blendfac2 = coll.A;
                blendfac2 = colr.A;
                revblendfac = 1 - blendfac2;
                colr = new Color4(
                    colr.R * blendfac2 + origcol.R * revblendfac,
                    colr.G * blendfac2 + origcol.G * revblendfac,
                    colr.B * blendfac2 + origcol.B * revblendfac,
                    1);
                r2 = colr.R;
                g2 = colr.G;
                b2 = colr.B;
                a2 = colr.A;
                if (blendfac1 + blendfac2 != 0) pressed = 1;
                else pressed = 0;

                double yy1 = y1;
                if (pressed == 1) yy1 += keyboardHeightFull * currPack.blackKeyPressedOversize;
                else yy1 += keyboardHeightFull * currPack.blackKeyOversize;
                if(pressed == 0 && currPack.blackKeyDefaultWhite)
                {
                    r = g = b = a = 1;
                    r2 = g2 = b2 = a2 = 1;
                }

                pos = quadBufferPos * 8;
                quadVertexbuff[pos++] = x1;
                quadVertexbuff[pos++] = y2;
                quadVertexbuff[pos++] = x2;
                quadVertexbuff[pos++] = y2;
                quadVertexbuff[pos++] = x2;
                quadVertexbuff[pos++] = yy1;
                quadVertexbuff[pos++] = x1;
                quadVertexbuff[pos++] = yy1;

                pos = quadBufferPos * 16;
                quadColorbuff[pos++] = r;
                quadColorbuff[pos++] = g;
                quadColorbuff[pos++] = b;
                quadColorbuff[pos++] = a;
                quadColorbuff[pos++] = r;
                quadColorbuff[pos++] = g;
                quadColorbuff[pos++] = b;
                quadColorbuff[pos++] = a;
                quadColorbuff[pos++] = r2;
                quadColorbuff[pos++] = g2;
                quadColorbuff[pos++] = b2;
                quadColorbuff[pos++] = a2;
                quadColorbuff[pos++] = r2;
                quadColorbuff[pos++] = g2;
                quadColorbuff[pos++] = b2;
                quadColorbuff[pos++] = a2;

                if (!currPack.blackKeysFullOctave)
                {
                    pos = quadBufferPos * 8;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = 0;
                }
                else
                {
                    var k = keynum[n] % 5;
                    double uvl = k / 5.0;
                    double uvr = (k + 1) / 5.0;
                    pos = quadBufferPos * 8;
                    quadUVbuff[pos++] = uvl;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = uvr;
                    quadUVbuff[pos++] = 1;
                    quadUVbuff[pos++] = uvr;
                    quadUVbuff[pos++] = 0;
                    quadUVbuff[pos++] = uvl;
                    quadUVbuff[pos++] = 0;
                }

                pos = quadBufferPos * 4;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;
                quadTexIDbuff[pos++] = pressed;

                quadBufferPos++;
                FlushQuadBuffer(true);
            }
            FlushQuadBuffer(false);
            #endregion

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Disable(EnableCap.Blend);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.Disable(EnableCap.Texture2D);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        void FlushQuadBuffer(bool check = true)
        {
            if (quadBufferPos < quadBufferLength && check) return;
            if (quadBufferPos == 0) return;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(quadBufferPos * 8 * 8),
                quadVertexbuff,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Double, false, 16, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferID);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(quadBufferPos * 16 * 4),
                quadColorbuff,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 16, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBufferID);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(quadBufferPos * 8 * 8),
                quadUVbuff,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Double, false, 16, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texIDBufferID);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(quadBufferPos * 1 * 4 * 4),
                quadTexIDbuff,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 4, 0);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 4, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.IndexPointer(IndexPointerType.Int, 1, 0);
            GL.DrawElements(PrimitiveType.Triangles, quadBufferPos * 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            quadBufferPos = 0;
        }

        bool isBlackNote(int n)
        {
            n = n % 12;
            return n == 1 || n == 3 || n == 6 || n == 8 || n == 10;
        }

        public void SetTrackColors(NoteColor[][] trakcs)
        {
            var cols = ((SettingsCtrl)SettingsControl).paletteList.GetColors(trakcs.Length);

            for (int i = 0; i < trakcs.Length; i++)
            {
                for (int j = 0; j < trakcs[i].Length; j++)
                {
                    trakcs[i][j].left = cols[i * 32 + j * 2];
                    trakcs[i][j].right = cols[i * 32 + j * 2 + 1];
                }
            }
        }
    }
}
