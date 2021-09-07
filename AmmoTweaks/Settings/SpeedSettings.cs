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
        [SettingName("Modifier les projectiles")]
        public bool DoSpeedChanges = true;

        [SynthesisOrder]
        [SettingName("Vitesse des flèches")]
        public float ArrowSpeed = 5400;

        [SynthesisOrder]
        [SettingName("Vitesse des carreaux")]
        public float BoltSpeed = 8100;

        [SynthesisOrder]
        [SettingName("Gravité")]
        public float Gravity = 0.2f;
    }
}
