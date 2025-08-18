using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    [HarmonyPriority(Priority.First)]
    public static class PingTrackerPatch {
        private static Vector3 lastDist = Vector3.zero;

        public static void Postfix(ref PingTracker __instance) {
            var position = __instance.GetComponent<AspectPosition>();
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                __instance.text.alignment = TextAlignmentOptions.Top;
                position.Alignment = AspectPosition.EdgeAlignments.Top;
                position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
            }
            else
            {
                position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
                __instance.text.alignment = TextAlignmentOptions.TopLeft;
                position.DistanceFromEdge = new Vector3(0.5f, 0.11f);
            }
            var byFangkuaiYaText = MeetingHud.Instance == null ? $"{ModTranslation.getString("pingTrackerModdedBy")} <color=#00ffff>FangkuaiYa</color>" : "";
                __instance.text.text = $"<size=130%><color=#BF00D6FF>Thanos Mods</color> v{ModThanos.VersionString}</size>\n{byFangkuaiYaText}\n{__instance.text.text}";
            //Mod also by Hardel, but this mod is very old, most code changed, so I have not write him in PingTracker
        }
    }
}