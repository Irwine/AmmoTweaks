using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Threading.Tasks;
using AmmoTweaks.Settings;
using Noggog;

namespace AmmoTweaks
{
    public class Program
    {
        static Lazy<AmmoTweaksSettings> _Settings = null!;
        static AmmoTweaksSettings Settings => _Settings.Value;

        private static List<IAmmunitionGetter> patchammo = new();

        private static List<String> overpowered = new();

        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "Settings.json", out _Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "AmmoTweaks.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            float vmin = Settings.Damage.MaxDamage;
            float vmax = Settings.Damage.MinDamage;
            foreach (var ammogetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())
            {
                if (ammogetter.Flags.HasFlag(Ammunition.Flag.NonPlayable)) continue;
                if (Settings.GlobalExclusions.Contains(ammogetter)) continue;
                
                patchammo.Add(ammogetter);

                if (Settings.Damage.Exclusions.Contains(ammogetter)) continue;
                var dmg = ammogetter.Damage;
                if (ammogetter.Damage == 0) continue;
                if (dmg < vmin) vmin = dmg;
                if (dmg > vmax && dmg <= Settings.Damage.MaxDamage) vmax = dmg;
                if (dmg > Settings.Damage.MaxDamage && ammogetter.Name?.String is string name) overpowered.Add(name);
            }

            foreach (var ammogetter in patchammo)
            {
                var ammo = state.PatchMod.Ammunitions.GetOrAddAsOverride(ammogetter);
                ammo.Weight = 0;

                if (Settings.Damage.DoRescaling 
                    && ammo.Damage != 0
                    && !Settings.Damage.Exclusions.Contains(ammo))
                {
                    var dmg = ammo.Damage;
                    if (dmg > Settings.Damage.MaxDamage) ammo.Damage = Settings.Damage.MaxDamage;
                    else ammo.Damage = (float)Math.Round(((ammo.Damage - vmin) / (vmax - vmin)) * (Settings.Damage.MaxDamage - Settings.Damage.MinDamage) + Settings.Damage.MinDamage);
                    Console.WriteLine($"Changing {ammo.Name} damage from {dmg} to {ammo.Damage}.");
                }

                if (Settings.Speed.DoSpeedChanges 
                    && !Settings.Speed.Exclusions.Contains(ammo.Projectile)
                    && ammo.Projectile.TryResolve(state.LinkCache, out var proj)
                    && (!proj.Gravity.EqualsWithin(Settings.Speed.Gravity)
                        || (!proj.Speed.EqualsWithin(Settings.Speed.ArrowSpeed) && ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                        || (!proj.Speed.EqualsWithin(Settings.Speed.BoltSpeed) && !ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))))
                {
                    var projectile = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                    Console.WriteLine($"Adjusting {proj.Name} projectile.");
                    projectile.Gravity = Settings.Speed.Gravity;
                    if (ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                    {
                        projectile.Speed = Settings.Speed.ArrowSpeed;
                    }
                    else
                    {
                        projectile.Speed = Settings.Speed.BoltSpeed;
                    }

                }

                if (Settings.Renaming.DoRenaming
                    && !Settings.Renaming.Exclusions.Contains(ammo))
                {
                    ammo.Name = RenameAmmo(ammo);
                }
            }

            if (!Settings.Loot.Mult.EqualsWithin(1))
            {
                if (Skyrim.GameSetting.iArrowInventoryChance.TryResolve(state.LinkCache, out var gmst))
                {
                    var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);

                    int data = ((GameSettingInt)modifiedGmst).Data.GetValueOrDefault();
                    int newData = (int)Math.Round(data * Settings.Loot.Mult);
                    ((GameSettingInt)modifiedGmst).Data = newData < 100 ? newData : 100;
                    Console.WriteLine($"Setting iArrowInventoryChance from {data} to {(newData < 100 ? newData : 100)}");
                }
                
                if (Skyrim.Perk.HuntersDiscipline.TryResolve(state.LinkCache, out var perk))
                {
                    var modifiedPerk = state.PatchMod.Perks.GetOrAddAsOverride(perk);

                    foreach (var effect in modifiedPerk.Effects)
                    {
                        if (effect is not PerkEntryPointModifyValue modValue) continue;
                        if (modValue.EntryPoint == APerkEntryPointEffect.EntryType.ModRecoverArrowChance)
                        {
                            var value = modValue.Value ?? 0;
                            var newValue = (float)Math.Round(value * Settings.Loot.Mult);
                            modValue.Value = newValue < 100 ? newValue : 100;
                            Console.WriteLine($"Setting {modifiedPerk.Name} chance from {value} to {(newValue < 100 ? newValue : 100)}");
                        }
                    }
                }
            }
            
            if (overpowered.Count == 0) return;
            Console.WriteLine("Warning: The following ammunitions were above the upper damage limit. They have been reduced to the maximum.");
            foreach (var item in overpowered)
            {
                Console.WriteLine(item);
            }
        }

        private static String RenameAmmo(IAmmunitionGetter ammo)
        {
            if (ammo.Name?.String is not string name) return "";
            string oldname = name;
            string prefix = "";
            string pattern = "Arrow$|Bolt$";

            if (name.Contains("Arrow"))
            {
                prefix = "Arrow";
            }
            else if (name.Contains("Bolt"))
            {
                prefix = "Bolt";
            }
            else
            {
                return name;
            }
            name = prefix + Settings.Renaming.Separator + Regex.Replace(name, pattern, String.Empty);
            name = name.Trim(' ');
            Console.WriteLine($"Renaming {oldname} to {name}.");
            return name;
        }
    }
}
