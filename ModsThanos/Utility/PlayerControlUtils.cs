using Hazel;
using System;
using UnityEngine;

namespace ModsThanos.Utility {
    public class PlayerControlUtils {
        public const float OffsetTruePositionX = 0;
        public const float OffsetTruePositionY = 0.366667f;

        public static Vector2 TruePositionOffset = new Vector2(OffsetTruePositionX, OffsetTruePositionY);

        public static Vector2 Position(PlayerControl player) {
            return player.GetTruePosition() + TruePositionOffset;
        }

        public static PlayerControl FromNetId(uint netId) {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.NetId == netId)
                    return player;

            return null;
        }

        public static PlayerControl FromPlayerId(byte id) {
            for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
                if (PlayerControl.AllPlayerControls[i].PlayerId == id)
                    return PlayerControl.AllPlayerControls[i];

            return null;
        }

        /// <summary>
        /// Set the players opacity (hat bugs a bit)
        /// </summary>
        /// <param name="opacity">Opacity value from 0 - 1</param>
        public void SetOpacity(float opacity, PlayerControl player) {
            var toSetColor = new Color(1, 1, 1, opacity);
            player.GetComponent<SpriteRenderer>().color = toSetColor;

            player.cosmetics.hat.FrontLayer.color = toSetColor;
            player.cosmetics.hat.BackLayer.color = toSetColor;
            player.cosmetics.SetHatColor(toSetColor);
            player.cosmetics.skin.layer.color = toSetColor;
            player.cosmetics.nameText.color = toSetColor;
        }

        public void Telportation(Vector2 position, PlayerControl player) {
            player.NetTransform.RpcSnapTo(position);
        }

        public static void KillPlayerArea(Vector2 psotion, PlayerControl murder, float size) {
            foreach (var player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == murder.PlayerId)
                    continue;

                float distance = Vector2.Distance(psotion, Position(player));
                if (distance < size)
                    player.MurderPlayer(player, MurderResultFlags.Succeeded);
            }
        }

        public static void RpcSetColorName(Color color, byte playerid) {
            FromPlayerId(playerid).cosmetics.nameText.color = new Color(color.r, color.g, color.b, color.a);

            MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SetColorName, SendOption.None);
            write.Write(playerid);
            write.Write(color.r);
            write.Write(color.g);
            write.Write(color.b);
            write.Write(color.a);
            AmongUsClient.Instance.FinishRpcImmediately(write);
        }

        public static void KillEveryone(PlayerControl murder) {
            foreach (var player in PlayerControl.AllPlayerControls) {
                if (player.PlayerId == murder.PlayerId)
                    continue;

                player.MurderPlayer(player, MurderResultFlags.Succeeded);
            }
        }

        public static bool AmHost() {
            return AmongUsClient.Instance.AmHost;
        }
    }
}
