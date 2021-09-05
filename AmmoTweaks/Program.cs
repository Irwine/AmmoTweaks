using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using AmmoTweaks.Settings;

namespace AmmoTweaks
{
    public class Program
    {
        static Lazy<AmmoTweaksSettings> _Settings = null!;
        static AmmoTweaksSettings Settings => _Settings.Value;

        private static HashSet<IFormLinkGetter<IProjectileGetter>> blacklist = new(){
            Dragonborn.Projectile.DLC2ArrowRieklingSpearProjectile,
            Skyrim.Projectile.MQ101ArrowSteelProjectile
        };

        private static List<IAmmunitionGetter> patchammo = new List<IAmmunitionGetter>();

        private static List<String> overpowered = new List<String>();

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
            float vmin = Settings.Degats.DegatsMax;
            float vmax = Settings.Degats.DegatsMin;
            foreach (var ammogetter in state.LoadOrder.PriorityOrder.Ammunition().WinningOverrides())
            {
                if (!ammogetter.Flags.HasFlag(Ammunition.Flag.NonPlayable))
                {
                    patchammo.Add(ammogetter);
                    var dmg = ammogetter.Damage;
                    if (ammogetter.Damage == 0) continue;
                    if (dmg < vmin) vmin = dmg;
                    if (dmg > vmax && dmg <= Settings.Degats.DegatsMax) vmax = dmg;
                    if (dmg > Settings.Degats.DegatsMax && ammogetter.Name?.String is string name) overpowered.Add(name);
                }
            }

            foreach (var ammogetter in patchammo)
            {
                var ammo = state.PatchMod.Ammunitions.GetOrAddAsOverride(ammogetter);
                ammo.Weight = 0;
                
                string i18nAmmoName = "";
                ammo.Name?.TryLookup(Language.French, out i18nAmmoName);
                if (i18nAmmoName != null) {
                    ammo.Name = Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes(i18nAmmoName));
                }

                if (Settings.Degats.Reechelonner && ammo.Damage != 0)
                {
                    var dmg = ammo.Damage;
                    if (dmg > Settings.Degats.DegatsMax) ammo.Damage = Settings.Degats.DegatsMax;
                    else ammo.Damage = (float)Math.Round(((ammo.Damage - vmin) / (vmax - vmin)) * (Settings.Degats.DegatsMax - Settings.Degats.DegatsMin) + Settings.Damage.DegatsMin);
                    Console.WriteLine($"Changing {ammo.Name} damage from {dmg} to {ammo.Damage}.");
                }

                if (Settings.Vitesse.ModifierProjectiles && !blacklist.Contains(ammo.Projectile) && ammo.Projectile.TryResolve(state.LinkCache, out var proj)
                        && (proj.Gravity != Settings.Vitesse.Gravite
                        || (proj.Speed != Settings.Vitesse.VitesseFleches && ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                        || (proj.Speed != Settings.Vitesse.VitesseCarreau && !ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))))
                {
                    var projectile = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                    Console.WriteLine($"Adjusting {proj.Name} projectile.");
                    projectile.Gravity = Settings.Vitesse.Gravite;
                    if (ammo.Flags.HasFlag(Ammunition.Flag.NonBolt))
                    {
                        projectile.Speed = Settings.Vitesse.VitesseFleches;
                    }
                    else
                    {
                        projectile.Speed = Settings.Vitesse.VitesseCarreau;
                    }

                }

                if (Settings.Renommage.Renommer) ammo.Name = RenameAmmo(ammo);
            }

            if (Settings.Butin.Multiplicateur != 1)
            {
                if (Skyrim.GameSetting.iArrowInventoryChance.TryResolve(state.LinkCache, out var gmst))
                {
                    var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);

                    int data = ((GameSettingInt)modifiedGmst).Data.GetValueOrDefault();
                    int newData = (int)Math.Round(data * Settings.Butin.Multiplicateur);
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
                            var newValue = (float)Math.Round(value * Settings.Butin.Multiplicateur);
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
            string pattern = Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes("Flèche$|Carreau$"));

            if (name.Contains(Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes("Flèche"))))
            {
                prefix = Encoding.GetEncoding("ISO-8859-1").GetString(Encoding.UTF8.GetBytes("Flèche"));
            }
            else if (name.Contains("Carreau"))
            {
                prefix = "Carreau";
            }
            else
            {
                return name;
            }
            name = prefix + Settings.Renommage.Separateur + Regex.Replace(name, pattern, String.Empty);
            name = name.Trim(' ');
            Console.WriteLine($"Renommage {oldname} to {name}.");
            return name;
        }
    }
}
