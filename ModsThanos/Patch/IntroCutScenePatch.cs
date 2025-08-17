using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using PowerTools;
using UnityEngine;

namespace ModsThanos.Patch
{

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    public static class IntroCutscenePatch
    {
        public static IEnumerator CoBegin(IntroCutscene __instance)
        {
            Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Starting intro cutscene", null);
            SoundManager.Instance.PlaySound(__instance.IntroStinger, false, 1f, null);
            if (GameManager.Instance.IsNormal())
            {
                Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Normal", null);
                __instance.LogPlayerRoleData();
                __instance.HideAndSeekPanels.SetActive(false);
                __instance.CrewmateRules.SetActive(false);
                __instance.ImpostorRules.SetActive(false);
                __instance.ImpostorName.gameObject.SetActive(false);
                __instance.ImpostorTitle.gameObject.SetActive(false);
                var list = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                list =
                    IntroCutscene.SelectTeamToShow(
                        (Func<NetworkedPlayerInfo, bool>)(pcd =>
                            !PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                            pcd.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType
                        )
                    );
                if (list == null || list.Count < 1)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
                }
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    __instance.ImpostorText.gameObject.SetActive(false);
                }
                else
                {
                    int adjustedNumImpostors = GameManager.Instance.LogicOptions.GetAdjustedNumImpostors(GameData.Instance.PlayerCount);
                    if (adjustedNumImpostors == 1)
                    {
                        __instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsS, new UnityEngine.Object());
                    }
                    else
                    {
                        __instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, (Il2CppReferenceArray<Il2CppSystem.Object>)(new object[] { adjustedNumImpostors }));
                    }
                    __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
                    __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
                }
                yield return __instance.ShowTeam(list, 3f);
                yield return ShowRole(__instance);
            }
            else
            {
                Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek", null);
                __instance.LogPlayerRoleData();
                __instance.HideAndSeekPanels.SetActive(true);
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    __instance.CrewmateRules.SetActive(false);
                    __instance.ImpostorRules.SetActive(true);
                }
                else
                {
                    __instance.CrewmateRules.SetActive(true);
                    __instance.ImpostorRules.SetActive(false);
                }
                Il2CppSystem.Collections.Generic.List<PlayerControl> list2 = IntroCutscene.SelectTeamToShow(
                    (Func<NetworkedPlayerInfo, bool>)(pcd => PlayerControl.LocalPlayer.Data.Role.IsImpostor != pcd.Role.IsImpostor)
                ); if (list2 == null || list2.Count < 1)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
                }
                PlayerControl impostor = PlayerControl.AllPlayerControls.Find(
                    (Il2CppSystem.Predicate<PlayerControl>)(pc => pc.Data.Role.IsImpostor)
                );
                if (impostor == null)
                {
                    Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: impostor is NULL", null);
                }
                GameManager.Instance.SetSpecialCosmetics(impostor);
                __instance.ImpostorName.gameObject.SetActive(true);
                __instance.ImpostorTitle.gameObject.SetActive(true);
                __instance.BackgroundBar.enabled = false;
                __instance.TeamTitle.gameObject.SetActive(false);
                if (impostor != null)
                {
                    __instance.ImpostorName.text = impostor.Data.PlayerName;
                }
                else
                {
                    __instance.ImpostorName.text = "???";
                }
                yield return new WaitForSecondsRealtime(0.1f);
                PoolablePlayer playerSlot = null;
                if (impostor != null)
                {
                    playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
                    playerSlot.SetBodyType(PlayerBodyTypes.Normal);
                    playerSlot.SetFlipX(false);
                    playerSlot.transform.localPosition = __instance.impostorPos;
                    playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
                }
                yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
                yield return new WaitForSecondsRealtime(6f);
                if (playerSlot != null)
                {
                    playerSlot.gameObject.SetActive(false);
                }
                __instance.HideAndSeekPanels.SetActive(false);
                __instance.CrewmateRules.SetActive(false);
                __instance.ImpostorRules.SetActive(false);
                LogicOptionsHnS logicOptionsHnS = GameManager.Instance.LogicOptions as LogicOptionsHnS;
                LogicHnSMusic logicHnSMusic = GameManager.Instance.GetLogicComponent<LogicHnSMusic>() as LogicHnSMusic;
                if (logicHnSMusic != null)
                {
                    logicHnSMusic.StartMusicWithIntro();
                }
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                {
                    float crewmateLeadTime = (float)logicOptionsHnS.GetCrewmateLeadTime();
                    __instance.HideAndSeekTimerText.gameObject.SetActive(true);
                    PoolablePlayer poolablePlayer;
                    AnimationClip animationClip;
                    if (AprilFoolsMode.ShouldHorseAround())
                    {
                        poolablePlayer = __instance.HorseWrangleVisualSuit;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        animationClip = __instance.HnSSeekerSpawnHorseAnim;
                        __instance.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
                        __instance.HorseWrangleVisualPlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                    }
                    else if (AprilFoolsMode.ShouldLongAround())
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.LongSeeker);
                        animationClip = __instance.HnSSeekerSpawnLongAnim;
                    }
                    else
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        animationClip = __instance.HnSSeekerSpawnAnim;
                    }
                    poolablePlayer.SetBodyCosmeticsVisible(false);
                    poolablePlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                    SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.ToggleName(false);
                    component.Play(animationClip, 1f);
                    while (crewmateLeadTime > 0f)
                    {
                        __instance.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
                        crewmateLeadTime -= UnityEngine.Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    ShipStatus.Instance.HideCountdown = (float)logicOptionsHnS.GetCrewmateLeadTime();
                    if (AprilFoolsMode.ShouldHorseAround())
                    {
                        if (impostor != null)
                        {
                            impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
                        }
                    }
                    else if (AprilFoolsMode.ShouldLongAround())
                    {
                        if (impostor != null)
                        {
                            impostor.AnimateCustom(__instance.HnSSeekerSpawnLongInGameAnim);
                        }
                    }
                    else if (impostor != null)
                    {
                        impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
                        impostor.cosmetics.SetBodyCosmeticsVisible(false);
                    }
                }
                impostor = null;
                playerSlot = null;
            }
            ShipStatus.Instance.StartSFX();
            UnityEngine.Object.Destroy(__instance.gameObject);
            yield break;
        }

        private static IEnumerator ShowRole(IntroCutscene __instance)
        {
            RoleBehaviour role = PlayerControl.LocalPlayer.Data.Role;
            if (RoleHelper.IsThanos(PlayerControl.LocalPlayer.PlayerId))
            {
                __instance.RoleText.text = "Thanos";
                __instance.RoleText.color = new Color(0.749f, 0f, 0.839f, 1f);
                __instance.RoleBlurbText.text = "Find the stones, and challenge the crewmates.";
                __instance.YouAreText.color = new Color(0.749f, 0f, 0.839f, 1f);
                __instance.RoleBlurbText.color = new Color(0.749f, 0f, 0.839f, 1f);
            }
            else
            {
                __instance.RoleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(role.StringName);
                __instance.RoleText.color = role.TeamColor;
                __instance.RoleBlurbText.text = "Find Thanos and complete tasks.";
                __instance.YouAreText.color = role.TeamColor;
                __instance.RoleBlurbText.color = role.TeamColor;
            }
            SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.Data.Role.IntroSound, loop: false);
            __instance.YouAreText.gameObject.SetActive(true);
            __instance.RoleText.gameObject.SetActive(true);
            __instance.RoleBlurbText.gameObject.SetActive(true);
            if (__instance.ourCrewmate == null)
            {
                __instance.ourCrewmate = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, impostorPositioning: false);
                __instance.ourCrewmate.gameObject.SetActive(false);
            }
            __instance.ourCrewmate.gameObject.SetActive(true);
            __instance.ourCrewmate.transform.localPosition = new Vector3(0f, -1.05f, -18f);
            __instance.ourCrewmate.transform.localScale = new Vector3(1f, 1f, 1f);
            __instance.ourCrewmate.ToggleName(active: false);
            yield return new WaitForSeconds(2.5f);
            __instance.YouAreText.gameObject.SetActive(false);
            __instance.RoleText.gameObject.SetActive(false);
            __instance.RoleBlurbText.gameObject.SetActive(false);
            __instance.ourCrewmate.gameObject.SetActive(false);
        }

        public static bool Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
        {
            if (GameOptionsManager.Instance.currentGameMode == AmongUs.GameOptions.GameModes.HideNSeek) return true;

            __result = CoBegin(__instance).WrapToIl2Cpp();

            return false;
        }
    }
}
