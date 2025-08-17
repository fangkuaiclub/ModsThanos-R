using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    class TasksPatch {
        public static void Postfix(PlayerControl __instance) {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;
            if (PlayerControl.LocalPlayer != null) {
                if (GlobalVariable.allThanos != null && RoleHelper.IsThanos(PlayerControl.LocalPlayer.PlayerId)) {
                    ImportantTextTask ImportantTasks = PlayerControl.LocalPlayer.Data.Role.IsImpostor ? PlayerControl.LocalPlayer.myTasks.ToArray()[0].Cast<ImportantTextTask>() : new GameObject("ThanosTasks").AddComponent<ImportantTextTask>();
                    ImportantTasks.transform.SetParent(__instance.transform, false);
                    ImportantTasks.Text = @"<color=#ffffff>Objective: Find the stones to get the snap.

<color=#808080FF>Snap:</color> Ends the game.
<color=#008516FF>Time Stone:</color> Allows you to go back in time.
<color=#822FA8FF>Power Stone:</color> Allows killing in an area.
<color=#C46f1AFF>Soul Stone:</color> Crewmates can pick it up.
<color=#A6A02EFF>Mind Stone:</color> Allows you to transform into someone.
<color=#3482BAFF>Space Stone</color>: Places portals.
<color=#D43D3DFF>Reality Stone</color>: Allows you to become invisible
</color>";
                    if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                        __instance.myTasks.Insert(0, ImportantTasks);
                }
            }
        }
    }
}