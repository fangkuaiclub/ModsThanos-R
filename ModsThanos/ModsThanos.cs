using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using System;
using System.Linq;
using System.Net;
using UnityEngine;

namespace ModsThanos
{

    [BepInPlugin(Id, "Mods Thanos", VersionString)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class ModThanos : BasePlugin
    {
        public const string Id = "gg.fuzeIII.ModsThanos";

        public const string VersionString = "1.3.0";

        public static ManualLogSource Logger;

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; set; }

        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            Logger = Log;
            Logger.LogInfo("ThanosMods!");
            Harmony.PatchAll();
            CustomOption.Patches.LoadSettings();
            //ResourceLoader.LoadAssets();
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class DebugManager
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                System.Console.WriteLine($"new Vector2({PlayerControl.LocalPlayer.transform.localPosition.x}, {PlayerControl.LocalPlayer.transform.localPosition.y})");
            }
        }
    }
}