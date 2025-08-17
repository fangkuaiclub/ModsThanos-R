using HarmonyLib;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch {
        public static void Postfix(VersionShower __instance) {
            __instance.text.text += " - <color=#BF00D6FF>Thanos Mod</color> fangkuai.fun";
        }
    }
}