using HarmonyLib;
using ModsThanos.Utility;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    public static class MeetingClosePatch {

        public static void Postfix(MeetingHud __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            if (GlobalVariable.allThanos != null) {
                if (RoleHelper.IsThanos(PlayerControl.LocalPlayer.PlayerId)) {
                    GlobalVariable.buttonMind.Timer = CustomGameOptions.CooldownMindStone.Get();
                    GlobalVariable.buttonPower.Timer = CustomGameOptions.CooldownPowerStone.Get();
                    GlobalVariable.buttonReality.Timer = CustomGameOptions.CooldownRealityStone.Get();
                    GlobalVariable.buttonSoul.Timer = CustomGameOptions.CooldownSoulStone.Get();
                    GlobalVariable.buttonTime.Timer = CustomGameOptions.CooldownTimeStone.Get();
                    GlobalVariable.buttonSpace.Timer = CustomGameOptions.CooldownSpaceStone.Get();
                    GlobalVariable.buttonSnap.Timer = 15f;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingUpdatePatch {
        public static void Postfix(MeetingHud __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            if (GlobalVariable.allThanos != null) {
                if (RoleHelper.IsThanos(PlayerControl.LocalPlayer.PlayerId)) {
                    GlobalVariable.buttonMind.SetCanUse(false);
                    GlobalVariable.buttonPower.SetCanUse(false);
                    GlobalVariable.buttonSnap.SetCanUse(false);
                    GlobalVariable.buttonReality.SetCanUse(false);
                    GlobalVariable.buttonMind.SetCanUse(false);
                    GlobalVariable.buttonTime.SetCanUse(false);
                    GlobalVariable.buttonSpace.SetCanUse(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public static class MeetingStartPatch {
        public static void Postfix(MeetingHud __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            if (GlobalVariable.allThanos != null && GlobalVariable.mindStoneUsed) {
                foreach (var playerData in GlobalVariable.allPlayersData) {
                    PlayerControl currentPlayer = PlayerControlUtils.FromPlayerId(playerData.PlayerId);

                    currentPlayer.SetPet(playerData.PlayerPet);
                    currentPlayer.SetSkin(playerData.PlayerSkin, playerData.PlayerColor);
                    currentPlayer.SetName(playerData.PlayerName);
                    currentPlayer.SetHat(playerData.PlayerHat, playerData.PlayerColor);
                    currentPlayer.SetColor(playerData.PlayerColor);
                    GlobalVariable.mindStoneUsed = false;
                }
            }

            Stone.System.Time.StopRewind();
            if (GlobalVariable.allThanos != null && GlobalVariable.realityStoneUsed && RoleHelper.IsThanos(PlayerControl.LocalPlayer.PlayerId)) {
                Stone.System.Reality.OnRealityPressed(false);
            }
        }
    }
}