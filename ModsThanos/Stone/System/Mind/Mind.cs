using Hazel;
using ModsThanos.Utility;
using System;
using System.Collections.Generic;

namespace ModsThanos.Stone.System.Mind {
    public static class CoreMind {

        public static void OnMindPressed() {
            HelperSprite.ShowAnimation(1, 15, true, "ModsThanos.Resources.anim-mind.png", 48, 1, PlayerControl.LocalPlayer.gameObject.transform.position, 1);

            GlobalVariable.PlayerHat = PlayerControl.LocalPlayer.Data.DefaultOutfit.HatId;
            GlobalVariable.PlayerPet = PlayerControl.LocalPlayer.Data.DefaultOutfit.PetId;
            GlobalVariable.PlayerSkin = PlayerControl.LocalPlayer.Data.DefaultOutfit.SkinId;
            GlobalVariable.PlayerColor = PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId;
            GlobalVariable.PlayerColorName = PlayerControl.LocalPlayer.cosmetics.nameText.color;
            GlobalVariable.PlayerName = PlayerControl.LocalPlayer.Data.PlayerName;
            GlobalVariable.mindStoneUsed = true;

            List<byte> players = new List<byte>();

            foreach (var element in PlayerControl.AllPlayerControls)
                if (element.PlayerId != PlayerControl.LocalPlayer.PlayerId && !element.Data.IsDead) 
                    players.Add(element.PlayerId);

            Random random = new Random();
            byte RandomPlayer = players[random.Next(0, players.Count)];

            PlayerControl player = PlayerControlUtils.FromPlayerId(PlayerControl.LocalPlayer.PlayerId);
            PlayerControl target = PlayerControlUtils.FromPlayerId(RandomPlayer);

            MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.MindChangedValue, SendOption.None, -1);
            write.Write(true);
            AmongUsClient.Instance.FinishRpcImmediately(write);

            player.RpcSetHat(target.Data.DefaultOutfit.HatId);
            player.RpcSetSkin(target.Data.DefaultOutfit.SkinId);
            player.RpcSetPet(target.Data.DefaultOutfit.PetId);
            player.RpcSetColor((byte)target.Data.DefaultOutfit.ColorId);
            player.RpcSetName(target.Data.PlayerName);
            PlayerControlUtils.RpcSetColorName(target.cosmetics.nameText.color, player.PlayerId);
        }

        public static void OnMindEnded(bool wihoutAnimation = false) {
            if (!wihoutAnimation) HelperSprite.ShowAnimation(1, 15, true, "ModsThanos.Resources.anim-mind.png", 48, 1, PlayerControl.LocalPlayer.gameObject.transform.position, 1);
            PlayerControl player = PlayerControlUtils.FromPlayerId(PlayerControl.LocalPlayer.PlayerId);
            MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.MindChangedValue, SendOption.None, -1);
            write.Write(false);
            AmongUsClient.Instance.FinishRpcImmediately(write);

            GlobalVariable.mindStoneUsed = false;
            player.RpcSetName(GlobalVariable.PlayerName);
            PlayerControlUtils.RpcSetColorName(new UnityEngine.Color(1f, 1f, 1f, 1f), PlayerControl.LocalPlayer.PlayerId);
            player.RpcSetHat(GlobalVariable.PlayerHat);
            player.RpcSetSkin(GlobalVariable.PlayerSkin);
            player.RpcSetPet(GlobalVariable.PlayerPet);
            player.RpcSetColor((byte)GlobalVariable.PlayerColor);
        }
    }
}
