using AmongUs.Data.Player;
using HarmonyLib;

namespace ModsThanos.Patch {
    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static class BanPatch {
        public static void Postfix(out bool __result) {
            __result = false;
        }
    }
}