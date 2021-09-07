using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.Skyrim;
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
        [SettingName("Reechelonner")]
        public bool DoRescaling = true;

        [SynthesisOrder]
        [SettingName("Dégâts minimaux")]
        public float MinDamage = 8;

        [SynthesisOrder]
        [SettingName("Dégâts maximaux")]
        public float MaxDamage = 35;

    }
}
