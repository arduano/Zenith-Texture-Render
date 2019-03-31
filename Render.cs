using BMEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexturedRender
{
    public class Render : IPluginRender
    {
        public string Name => "Textured";

        public string Description => "Plugin for loading and rendering custom resource packs, " +
            "with settings defined in a .json file";

        public bool Initialized { get; set; }

        public System.Windows.Media.ImageSource PreviewImage { get; set; } = null;

        public bool ManualNoteDelete => false;

        public int NoteCollectorOffset => 0;

        public double LastMidiTimePerTick { get; set; }
        public MidiInfo CurrentMidi { get; set; }

        public double NoteScreenTime { get; set; }

        public long LastNoteCount { get; set; }

        public System.Windows.Controls.Control SettingsControl { get; set; } = null;

        public void Dispose()
        {
            Initialized = false;
        }

        public Render(RenderSettings settings)
        {

        }

        public void Init()
        {
            Initialized = true;
        }

        public void RenderFrame(FastList<Note> notes, double midiTime, int finalCompositeBuff)
        {

        }

        public void SetTrackColors(NoteColor[][] trakcs)
        {

        }
    }
}
