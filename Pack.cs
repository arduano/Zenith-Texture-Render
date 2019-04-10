using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexturedRender
{
    public class NoteTexture
    {
        public double maxSize;
        public bool useCaps;
        public bool stretch;
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
        public string description = "";
        public Bitmap whiteKeyTex;
        public int whiteKeyTexID;
        public Bitmap blackKeyTex;
        public int blackKeyTexID;
        public Bitmap whiteKeyPressedTex;
        public int whiteKeyPressedTexID;
        public Bitmap blackKeyPressedTex;
        public int blackKeyPressedTexID;
        public NoteTexture[] NoteTextures;
    }
}
