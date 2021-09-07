using Mutagen.Bethesda.Synthesis.Settings;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmoTweaks.Settings
{
    public class RenamingSettings
    {
        [SynthesisOrder]
        [SettingName("Renommer")]
        public bool DoRenaming;

        [SynthesisOrder]
        [SettingName("Séparateur")]
        public string Separator = " - ";
    }
}
