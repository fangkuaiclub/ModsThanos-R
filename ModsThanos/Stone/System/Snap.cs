using AmongUs.Data;
using HarmonyLib;
using Hazel;
using ModsThanos.Utility;
using UnityEngine;

namespace ModsThanos.Stone.System
{
    class Snap
    {
        public static bool CachedScreenShake;
        public static void OnSnapPressed()
        {
            MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Snap, SendOption.None, -1);
            write.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(write);

            GlobalVariable.useSnap = true;
            Camera.main.GetComponent<FollowerCamera>().shakeAmount = 0.3f;
            Camera.main.GetComponent<FollowerCamera>().shakePeriod = 600f;
            CachedScreenShake = DataManager.Settings.Gameplay.ScreenShake;
            DataManager.Settings.Gameplay.ScreenShake = true;

            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(1f, 1f, 1f, 0f);
        }

        public static void Incremente()
        {
            Camera.main.GetComponent<FollowerCamera>().shakeAmount = 0.3f;
            Camera.main.GetComponent<FollowerCamera>().shakePeriod = 600f;
            DataManager.Settings.Gameplay.ScreenShake = true;

            Color currentColor = DestroyableSingleton<HudManager>.Instance.FullScreen.color;
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(1f, 1f, 1f, currentColor.a + 0.002f);
        }

        public static void OnSnapEnded()
        {
            GlobalVariable.useSnap = false;
            Camera.main.GetComponent<FollowerCamera>().shakeAmount = 0f;
            Camera.main.GetComponent<FollowerCamera>().shakePeriod = 0f;
            DataManager.Settings.Gameplay.ScreenShake = CachedScreenShake;

            DestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(1f, 1f, 1f, 0f);
            DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(false);
            DestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;

            MessageWriter write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SnapEnded, SendOption.None, -1);
            write.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(write);

            PlayerControlUtils.KillEveryone(PlayerControl.LocalPlayer);
        }
    }
}