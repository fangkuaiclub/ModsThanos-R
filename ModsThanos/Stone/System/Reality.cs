using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using ModsThanos.Utility;
using UnityEngine;

namespace ModsThanos.Stone.System {
    class Reality {
        public static List<byte> invisPlayers = new List<byte>();

        public static void OnRealityPressed(bool isInvis) {
            HelperSprite.ShowAnimation(1, 8, true, "ModsThanos.Resources.anim-reality.png", 48, 1, PlayerControl.LocalPlayer.gameObject.transform.position, 1);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.TurnInvisibility, SendOption.Reliable);
            writer.Write(isInvis);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RpcFunctions.TurnInvis(isInvis, PlayerControl.LocalPlayer);
        }
    }

    public static class RpcFunctions {
        static public void TurnInvis(bool isInvis, PlayerControl player)
        {
            if (Reality.invisPlayers.Contains(player.PlayerId))
                Reality.invisPlayers.Remove(player.PlayerId);

            var alpha = isInvis
                ? (player == PlayerControl.LocalPlayer ? 0.5f : 0.0f)
                : 1.0f;

            if (player.cosmetics.currentBodySprite.BodySprite != null)
                player.cosmetics.currentBodySprite.BodySprite.SetColorAlpha(alpha);

            if (player.cosmetics.skin?.layer != null)
                player.cosmetics.skin.layer.SetColorAlpha(alpha);

            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.FrontLayer.SetColorAlpha(alpha);
                player.cosmetics.hat.BackLayer.SetColorAlpha(alpha);
            }

            if (player.cosmetics.currentPet != null)
                player.cosmetics.currentPet.SetAlpha(alpha);

            if (player.cosmetics.visor != null)
                player.cosmetics.visor.Image.SetColorAlpha(alpha);

            if (player.cosmetics.colorBlindText != null)
                player.cosmetics.colorBlindText.color.SetAlpha(alpha);

            if (player.cosmetics.PettingHand != null)
                player.cosmetics.PettingHand.SetAlpha(alpha);
        }

        static public void SetColorAlpha(this SpriteRenderer renderer, float alpha)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerStartPatch {
        public static void Prefix() {
            Reality.invisPlayers.Clear();
        }
    }
}
