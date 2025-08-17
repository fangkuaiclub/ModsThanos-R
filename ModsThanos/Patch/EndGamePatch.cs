using HarmonyLib;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
    public static class EndGamePatch {

        public static void Prefix(AmongUsClient __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            EndGameCommons.ResetGlobalVariable();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGameManagerPatch {
        public static bool Prefix(EndGameManager __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return true;

            EndGameCommons.ResetGlobalVariable();

            return true;
        }
    }

    public static class EndGameCommons {
        public static void ResetGlobalVariable() {
            GlobalVariable.GameStarted = false;
            GlobalVariable.hasMindStone = false;
            GlobalVariable.hasPowerStone = false;
            GlobalVariable.hasRealityStone = false;
            GlobalVariable.hasSoulStone = false;
            GlobalVariable.hasSpaceStone = false;
            GlobalVariable.hasTimeStone = false;
            GlobalVariable.useSnap = false;
            GlobalVariable.mindStoneUsed = false;
            GlobalVariable.realityStoneUsed = false;
            GlobalVariable.UsableButton = false;

            GlobalVariable.allThanos.Clear();
            GlobalVariable.stoneObjects.Clear();
            GlobalVariable.stonePositon.Clear();
        }
    }
}