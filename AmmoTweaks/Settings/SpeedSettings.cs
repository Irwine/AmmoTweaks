using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class SpeedSettings
    {
        [SynthesisOrder]
        public bool ModifierProjectiles = true;

        [SynthesisOrder]
        public float VitesseFleches = 5400;

        [SynthesisOrder]
        public float VitesseCarreau = 8100;

        [SynthesisOrder]
        public float Gravite = 0.2f;
    }
}
