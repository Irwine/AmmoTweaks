using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class AmmoTweaksSettings
    {
        [SynthesisOrder]
        [SettingName("Dégats")]
        public DamageSettings Damage = new();

        [SynthesisOrder]
        [SettingName("Rennomage")]
        public RenamingSettings Renaming = new();

        [SynthesisOrder]
        [SettingName("Vitesse")]
        public SpeedSettings Speed = new();

        [SynthesisOrder]
        [SettingName("Butin")]
        public LootSettings Loot = new();
    }
}
