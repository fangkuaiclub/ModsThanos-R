using AmongUs.Data;
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
                    ImportantTasks.Text = ModTranslation.getString("importantText");
                    if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                        __instance.myTasks.Insert(0, ImportantTasks);
                }
            }
        }
    }
}