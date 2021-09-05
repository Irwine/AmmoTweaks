using Mutagen.Bethesda.Synthesis.Settings;
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
        public DamageSettings Degats = new();

        [SynthesisOrder]
        public RenamingSettings Renommer = new();

        [SynthesisOrder]
        public SpeedSettings Vitesse = new();

        [SynthesisOrder]
        public LootSettings Butin = new();
    }
}
