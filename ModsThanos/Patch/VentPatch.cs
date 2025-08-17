using HarmonyLib;
using UnityEngine;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatch {
        public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek)
            {
                canUse = true;
                couldUse = true;
                return true;
            }

            float maxFloat = float.MaxValue;
            PlayerControl player = playerInfo.Object;
            couldUse = (playerInfo.Role.IsImpostor || RoleHelper.IsThanos(playerInfo.PlayerId) && !playerInfo.IsDead && (player.CanMove || player.inVent));
            canUse = couldUse;
            if (canUse) {
                maxFloat = Vector2.Distance(player.GetTruePosition(), __instance.transform.position);
                canUse &= maxFloat <= __instance.UsableDistance;
            }

            __result = maxFloat;
            return false;
        }
    }
}
