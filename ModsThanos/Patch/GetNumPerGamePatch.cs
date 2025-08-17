using AmongUs.GameOptions;
using HarmonyLib;

namespace ModsThanos.Patch
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV09), nameof(RoleOptionsCollectionV09.GetNumPerGame))]
    internal class GetNumPerGamePatch
    {
        public static void Postfix(ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal && CustomGameOptions.EnableThanosMods.Get())
                __result = 0; // Deactivate Vanilla Roles if the mod roles are active
        }
    }
}
