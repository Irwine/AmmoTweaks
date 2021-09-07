using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class LootSettings
    {
        [SynthesisOrder]
        [SettingName("Multiplicateur")]
        public float Mult = 1f;
    }
}
