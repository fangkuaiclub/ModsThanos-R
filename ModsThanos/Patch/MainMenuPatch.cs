using HarmonyLib;
using ModsThanos.Utility;
using TMPro;
using UnityEngine;

namespace ModsThanos.Patch
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class LogoPatch
    {
        static void Postfix(PingTracker __instance)
        {
            var torLogo = new GameObject("bannerLogo_MT");
            torLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            torLogo.transform.localPosition = new Vector3(-0.4f, 1f, 5f);

            var renderer = torLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = HelperSprite.LoadSpriteFromEmbeddedResources("ModsThanos.Resources.ModLogo.png", 300f);

            var credentialObject = new GameObject("credentialsMT");
            var credentials = credentialObject.AddComponent<TextMeshPro>();
            credentials.SetText($"v{ModThanos.VersionString}\nBy <color=#00ffff>FangkuaiYa</color> & Hardel\nSpecial Thanks <color=#9B05F9>乱码</color> & <color=#98F5F9>farewell</color>");
            credentials.alignment = TMPro.TextAlignmentOptions.Center;
            credentials.fontSize *= 0.05f;
            credentials.transform.SetParent(torLogo.transform);
            credentials.transform.localPosition = Vector3.down + new Vector3(0f, -0.6f, 0f);
        }
    }
}
