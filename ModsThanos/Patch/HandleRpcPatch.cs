using HarmonyLib;
using Hazel;
using ModsThanos.CustomOption;
using ModsThanos.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModsThanos.Patch
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

            switch (callId)
            {
                case (byte)CustomRPC.SyncStone:
                    HandleSyncStone(reader);
                    break;

                case (byte)CustomRPC.ReplaceStone:
                    HandleReplaceStone(reader);
                    break;

                case (byte)CustomRPC.SetVisiorColor:
                    HandleSetVisorColor(reader);
                    break;

                case (byte)CustomRPC.SetColorName:
                    HandleSetNameColor(reader);
                    break;

                case (byte)CustomRPC.SetPlayerSoulStone:
                    HandleSetPlayerSoulStone(reader);
                    break;

                case (byte)CustomRPC.RemovePlayerSoulStone:
                    HandleRemovePlayerSoulStone();
                    break;

                case (byte)CustomRPC.SetThanos:
                    HandleSetThanos(reader);
                    break;

                case (byte)CustomRPC.StonePickup:
                    HandleStonePickup(reader);
                    break;

                case (byte)CustomRPC.MindChangedValue:
                    HandleMindChangedValue(reader);
                    break;

                case (byte)CustomRPC.TimeRewind:
                    HandleTimeRewind();
                    break;

                case (byte)CustomRPC.TimeRevive:
                    HandleTimeRevive(reader);
                    break;

                case (byte)CustomRPC.PowerStone:
                    HandlePowerStone(reader);
                    break;

                case (byte)CustomRPC.TurnInvisibility:
                    HandleTurnInvisibility(reader, __instance);
                    break;

                case (byte)CustomRPC.Snap:
                    HandleSnap();
                    break;

                case (byte)CustomRPC.SnapEnded:
                    HandleSnapEnded(reader);
                    break;

                case (byte)CustomRPC.SyncCustomSettings:
                    RpcUpdateSetting.ReceiveRpc(reader);
                    break;
            }
        }

        private static void HandleSyncStone(MessageReader reader)
        {
            string stoneName = reader.ReadString();
            Vector2 vector = reader.ReadVector2();

            if (!GlobalVariable.stoneObjects.ContainsKey(stoneName))
                GlobalVariable.stonePositon.Add(stoneName, vector);
            else
                GlobalVariable.stoneObjects[stoneName].ModifyPosition(vector);
        }

        private static void HandleReplaceStone(MessageReader reader)
        {
            string stoneName = reader.ReadString();
            Vector2 vector = reader.ReadVector2();
            Stone.StoneDrop.ReplaceStone(stoneName, vector);
        }

        private static void HandleSetVisorColor(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            float colorR = reader.ReadSingle();
            float colorG = reader.ReadSingle();
            float colorB = reader.ReadSingle();
            float colorA = reader.ReadSingle();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == playerId)
                {
                    player.cosmetics.currentBodySprite.BodySprite.material.SetColor(
                        "_VisorColor",
                        new Color(colorR, colorG, colorB, colorA)
                    );
                }
            }
        }

        private static void HandleSetNameColor(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            float colorR = reader.ReadSingle();
            float colorG = reader.ReadSingle();
            float colorB = reader.ReadSingle();
            float colorA = reader.ReadSingle();

            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == playerId)
                    player.cosmetics.nameText.color = new Color(colorR, colorG, colorB, colorA);
        }

        private static void HandleSetPlayerSoulStone(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            GlobalVariable.PlayerSoulStone = PlayerControlUtils.FromPlayerId(playerId);
        }

        private static void HandleRemovePlayerSoulStone()
        {
            GlobalVariable.PlayerSoulStone = null;
        }

        private static void HandleSetThanos(MessageReader reader)
        {
            GlobalVariable.allThanos.Clear();
            List<byte> selectedPlayers = reader.ReadBytesAndSize().ToList();
            selectedPlayers.ForEach(id => GlobalVariable.allThanos.Add(PlayerControlUtils.FromPlayerId(id)));
        }

        private static void HandleStonePickup(MessageReader reader)
        {
            string nameStone = reader.ReadString();

            if (GlobalVariable.stoneObjects.ContainsKey(nameStone))
                GlobalVariable.stoneObjects[nameStone].DestroyThisObject();

            if (nameStone == "Soul")
                Object.DestroyImmediate(GlobalVariable.arrow);
        }

        private static void HandleMindChangedValue(MessageReader reader)
        {
            GlobalVariable.mindStoneUsed = reader.ReadBoolean();
        }

        private static void HandleTimeRewind()
        {
            Stone.System.Time.isRewinding = true;
            GlobalVariable.UsableButton = false;
            PlayerControl.LocalPlayer.moveable = false;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.639f, 0.211f, 0.3f);
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        }

        private static void HandleTimeRevive(MessageReader reader)
        {
            PlayerControl player = PlayerControlUtils.FromPlayerId(reader.ReadByte());
            player.Revive();
            var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == player.PlayerId);
            if (body != null) Object.Destroy(body.gameObject);
        }

        private static void HandlePowerStone(MessageReader reader)
        {
            PlayerControl murder = PlayerControlUtils.FromPlayerId(reader.ReadByte());
            HelperSprite.ShowAnimation(1, 24, true, "ModsThanos.Resources.anim-power.png", 10, 1,
                murder.gameObject.transform.position, 5);
            Vector2 murderPosition = reader.ReadVector2();
            PlayerControlUtils.KillPlayerArea(murderPosition, murder, 3f);
        }

        private static void HandleTurnInvisibility(MessageReader reader, PlayerControl __instance)
        {
            bool isInvis = reader.ReadBoolean();
            byte playerId = reader.ReadByte();
            PlayerControl player = PlayerControlUtils.FromPlayerId(playerId);

            HelperSprite.ShowAnimation(1, 8, true, "ModsThanos.Resources.anim-reality.png", 48, 1,
                player.gameObject.transform.position, 1);

            GlobalVariable.realityStoneUsed = isInvis;
            Stone.System.RpcFunctions.TurnInvis(isInvis, __instance);
        }

        private static void HandleSnap()
        {
            GlobalVariable.useSnap = true;
            Camera.main.GetComponent<FollowerCamera>().shakeAmount = 0.2f;
            Camera.main.GetComponent<FollowerCamera>().shakePeriod = 1200f;
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(1f, 1f, 1f, 0f);
        }

        private static void HandleSnapEnded(MessageReader reader)
        {
            GlobalVariable.useSnap = false;
            Camera.main.GetComponent<FollowerCamera>().shakeAmount = 0f;
            Camera.main.GetComponent<FollowerCamera>().shakePeriod = 0f;
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(1f, 1f, 1f, 0f);

            PlayerControl player = PlayerControlUtils.FromPlayerId(reader.ReadByte());
            PlayerControlUtils.KillEveryone(player);
        }
    }
}