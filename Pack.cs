using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexturedRender
{
    public enum TextureShaderType
    {
        Normal,
        Inverted,
        Hybrid
    }

    public class NoteTexture
    {
        public double maxSize;
        public bool useCaps;
        public bool stretch;

        public double darkenBlackNotes = 1;

        public double noteMiddleAspect;
        public Bitmap noteMiddleTex;
        public int noteMiddleTexID;
        public double noteTopOversize;
        public double noteTopAspect;
        public Bitmap noteTopTex;
        public int noteTopTexID;
        public double noteBottomOversize;
        public double noteBottomAspect;
        public Bitmap noteBottomTex;
        public int noteBottomTexID;
    }

    public class Pack
    {
        public string pathName;
        public string name;
        public bool error = false;
        public Bitmap preview = null;

        public bool sameWidthNotes = false;
        public double keyboardHeight = 0.15;
        public double blackKeyHeight = 0.4;

        public string description = "";

        public TextureShaderType noteShader = TextureShaderType.Normal;
        public TextureShaderType whiteKeyShader = TextureShaderType.Normal;
        public TextureShaderType blackKeyShader = TextureShaderType.Normal;
        public bool blackKeyDefaultWhite = false;

        public Bitmap whiteKeyTex;
        public int whiteKeyTexID;
        public double whiteKeyOversize = 0;
        public Bitmap blackKeyTex;
        public int blackKeyTexID;
        public double blackKeyOversize = 0;
        public Bitmap whiteKeyPressedTex;
        public int whiteKeyPressedTexID;
        public double whiteKeyPressedOversize = 0;
        public Bitmap blackKeyPressedTex;
        public int blackKeyPressedTexID;
        public double blackKeyPressedOversize = 0;

        public bool useBar = false;
        public Bitmap barTex;
        public int barTexID;
        public double barHeight = 0.05;

        public bool whiteKeysFullOctave = false;
        public bool blackKeysFullOctave = false;
        public NoteTexture[] NoteTextures;
    }
}
