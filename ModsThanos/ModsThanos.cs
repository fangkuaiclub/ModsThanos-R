using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using UnityEngine;

namespace ModsThanos
{

    [BepInPlugin(Id, "Mods Thanos", VersionString)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class ModThanos : BasePlugin
    {
        public const string Id = "fangkuai.fun.ModsThanos";

        public const string VersionString = "1.3.0";

        public static ManualLogSource Logger;

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; set; }

        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            Logger = Log;
            ModTranslation.Load();
            Logger.LogInfo("ThanosMods!");
            Harmony.PatchAll();
            CustomOption.Patches.LoadSettings();
            //ResourceLoader.LoadAssets();
        }
    }
}