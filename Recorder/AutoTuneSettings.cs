using System.Collections.Generic;

namespace Recorder
{
    public class AutoTuneSettings
    {
        public AutoTuneSettings()
        {
            // set up defaults
            SnapMode = true;
            PluggedIn = true;
            AutoPitches = new HashSet<Note>();
            AutoPitches.Add(Note.C);
            AutoPitches.Add(Note.CSharp);
            AutoPitches.Add(Note.D);
            AutoPitches.Add(Note.DSharp);
            AutoPitches.Add(Note.E);
            AutoPitches.Add(Note.F);
            AutoPitches.Add(Note.FSharp);
            AutoPitches.Add(Note.G);
            AutoPitches.Add(Note.GSharp);
            AutoPitches.Add(Note.A);
            AutoPitches.Add(Note.ASharp);
            AutoPitches.Add(Note.B);
            VibratoDepth = 0.0;
            VibratoRate = 4.0;
            AttackTimeMilliseconds = 0.0;
        }

        public bool Enabled { get; set; }
        public bool SnapMode { get; set; } // snap mode finds a note from the list to snap to, non-snap mode is provided with target pitches from outside
        public double AttackTimeMilliseconds { get; set; }
        public HashSet<Note> AutoPitches { get; private set; }
        public bool PluggedIn { get; set; } // not currently used
        public double VibratoRate { get; set; } // not currently used
        public double VibratoDepth { get; set; }
    }
    public enum Note
    {
        C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B
    }
}