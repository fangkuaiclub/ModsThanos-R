using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using ModsThanos.Utility.Enumerations;

namespace ModsThanos.Patch
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
    class SetRolePatch
    {
        public static void Postfix()
        {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            List<PlayerControl> playersSelections = PlayerControl.AllPlayerControls.ToArray().ToList();
            Visibility visibility = CustomGameOptions.SideStringToEnum(CustomGameOptions.ThanosSide.Get());
            GlobalVariable.allThanos.Clear();

            if (Visibility.OnlyImpostor == visibility)
                playersSelections.RemoveAll(x => !x.Data.Role.IsImpostor);

            if (Visibility.OnlyCrewmate == visibility)
                playersSelections.RemoveAll(x => x.Data.Role.IsImpostor);

            // playersSelections
            if (playersSelections != null && playersSelections.Count > 0 && CustomGameOptions.EnableThanosMods.Get())
            {
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetThanos, SendOption.None, -1);
                List<byte> playerSelected = new List<byte>();

                for (int i = 0; i < CustomGameOptions.NumberThanos.Get(); i++)
                {
                    Random random = new Random();
                    PlayerControl selectedPlayer = playersSelections[random.Next(0, playersSelections.Count)];
                    GlobalVariable.allThanos.Add(selectedPlayer);
                    playersSelections.Remove(selectedPlayer);
                    playerSelected.Add(selectedPlayer.PlayerId);
                }

                messageWriter.WriteBytesAndSize(playerSelected.ToArray());
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }
        }
    }
}