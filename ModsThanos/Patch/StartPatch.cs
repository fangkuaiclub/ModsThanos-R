using HarmonyLib;
using Hazel;
using ModsThanos.Map;
using ModsThanos.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace ModsThanos.Patch {

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class ShipStatusPatch {
        public static void Prefix(ShipStatus __instance) {
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay || GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            Stone.System.Time.pointsInTime.Clear();
            Stone.System.Time.recordTime = CustomGameOptions.TimeDuration.Get();
            GlobalVariable.hasMindStone = false;
            GlobalVariable.hasPowerStone = false;
            GlobalVariable.hasRealityStone = false;
            GlobalVariable.hasSoulStone = false;
            GlobalVariable.hasSpaceStone = false;
            GlobalVariable.realityStoneUsed = false;
            GlobalVariable.hasTimeStone = false;
            GlobalVariable.useSnap = false;
            GlobalVariable.mindStoneUsed = false;
            GlobalVariable.GameStarted = true;
            GlobalVariable.UsableButton = true;
            GlobalVariable.PlayerName = null;
            
            GlobalVariable.allPlayersData.Clear();
            foreach (var player in PlayerControl.AllPlayerControls) {
                GlobalVariable.allPlayersData.Add(new Stone.System.Mind.PlayerData(
                    player.PlayerId,
                    player.cosmetics.ColorId,
                    player.cosmetics.hat.name,
                    player.cosmetics.currentPet.name,
                    player.cosmetics.skin.name,
                    player.cosmetics.nameText.text
                ));
            }

            // Button Timer
            Stone.System.Time.recordTime = CustomGameOptions.TimeDuration.Get();
            GlobalVariable.buttonMind.MaxTimer = CustomGameOptions.CooldownMindStone.Get();
            GlobalVariable.buttonMind.EffectDuration = CustomGameOptions.MindDuration.Get();
            GlobalVariable.buttonReality.MaxTimer = CustomGameOptions.CooldownRealityStone.Get();
            GlobalVariable.buttonReality.EffectDuration = CustomGameOptions.RealityDuration.Get();
            GlobalVariable.buttonTime.MaxTimer = CustomGameOptions.CooldownTimeStone.Get();
            GlobalVariable.buttonTime.EffectDuration = CustomGameOptions.TimeDuration.Get() / 2;
            GlobalVariable.buttonPower.MaxTimer = CustomGameOptions.CooldownPowerStone.Get();
            GlobalVariable.buttonSpace.MaxTimer = CustomGameOptions.CooldownSpaceStone.Get();
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    public static class Strat2Patch
    {
        public static void Postfix(ShipStatus __instance)
        {
            Dictionary<string, Vector2> stonePosition = StonePlacement.SetAllStonePositions();
            StonePlacement.PlaceAllStone();

            if (PlayerControlUtils.AmHost())
            {
                foreach (var stone in stonePosition)
                {
                    MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncStone, SendOption.None, -1);
                    write.Write(stone.Key);
                    write.WriteVector2(stone.Value);
                    AmongUsClient.Instance.FinishRpcImmediately(write);
                }
            }
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public static class LobbyBehaviourPatch {
        public static void Prefix() {
            GlobalVariable.stoneObjects.Clear();
            GlobalVariable.stonePositon.Clear();
            GlobalVariable.allThanos.Clear();
            GlobalVariable.hasMindStone = false;
            GlobalVariable.hasPowerStone = false;
            GlobalVariable.hasRealityStone = false;
            GlobalVariable.hasSoulStone = false;
            GlobalVariable.hasSpaceStone = false;
            GlobalVariable.hasTimeStone = false;
            GlobalVariable.realityStoneUsed = false;
            GlobalVariable.useSnap = false;
            GlobalVariable.mindStoneUsed = false;
            GlobalVariable.UsableButton = false;
        }
    }
}
