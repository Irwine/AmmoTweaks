using Mutagen.Bethesda.Synthesis.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class DamageSettings
    {
        [SynthesisOrder]
        public bool Reechelonner = true;

        [SynthesisOrder]
        public float DegatsMin = 8;

        [SynthesisOrder]
        public float DegatsMax = 35;

    }
}
