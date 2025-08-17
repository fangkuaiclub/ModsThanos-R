using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel.Crypto;
using Il2CppSystem.Text;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace ModsThanos.CustomOption
{
    public static class Patches
    {
        private const string SettingsFileName = "ModSettings.cfg";
        private static bool isLoadingSettings = false;

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
        private class MoreTasks
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (__instance.gameObject.name == "GAME SETTINGS TAB")
                {
                    try
                    {
                        var commonTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
                        if (commonTasks != null) commonTasks.ValidRange = new FloatRange(0f, 4f);

                        var shortTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).Cast<NumberOption>();
                        if (shortTasks != null) shortTasks.ValidRange = new FloatRange(0f, 8f);

                        var longTasks = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).Cast<NumberOption>();
                        if (longTasks != null) longTasks.ValidRange = new FloatRange(0f, 4f);
                    }
                    catch { /* Ignore errors */ }
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
        class ChangeTab
        {
            public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
            {
                if (previewOnly) return;
                foreach (var tab in SettingsUpdate.Tabs) if (tab != null) tab.SetActive(false);
                foreach (var button in SettingsUpdate.Buttons) button.SelectButton(false);

                if (tabNum > 2)
                {
                    tabNum -= 3;
                    SettingsUpdate.Tabs[tabNum].SetActive(true);
                    SettingsUpdate.Buttons[tabNum].SelectButton(true);

                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
                    {
                        foreach (CustomOption option in CustomOption.AllOptions)
                        {
                            if (option.Type == CustomOptionType.Number)
                            {
                                var number = option.Setting.Cast<NumberOption>();
                                number.TitleText.text = option.Name;
                                if (number.TitleText.text.StartsWith("<color="))
                                    number.TitleText.fontSize = 3f;
                                else if (number.TitleText.text.Length > 20)
                                    number.TitleText.fontSize = 2.25f;
                                else if (number.TitleText.text.Length > 40)
                                    number.TitleText.fontSize = 2f;
                                else number.TitleText.fontSize = 2.75f;
                            }

                            else if (option.Type == CustomOptionType.Toggle)
                            {
                                var tgl = option.Setting.Cast<ToggleOption>();
                                tgl.TitleText.text = option.Name;
                                if (tgl.TitleText.text.Length > 20)
                                    tgl.TitleText.fontSize = 2.25f;
                                else if (tgl.TitleText.text.Length > 40)
                                    tgl.TitleText.fontSize = 2f;
                                else tgl.TitleText.fontSize = 2.75f;
                            }

                            else if (option.Type == CustomOptionType.String)
                            {
                                var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                                if (option.Name.StartsWith("Slot "))
                                {
                                    try
                                    {
                                        int slotNumber = int.Parse(option.Name[5..]);
                                        if (slotNumber > GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers) continue;
                                    }
                                    catch { }
                                }

                                var str = option.Setting.Cast<StringOption>();
                                str.TitleText.text = option.Name;
                                if (str.TitleText.text.Length > 20)
                                    str.TitleText.fontSize = 2.25f;
                                else if (str.TitleText.text.Length > 40)
                                    str.TitleText.fontSize = 2f;
                                else str.TitleText.fontSize = 2.75f;
                            }
                        }
                    })));
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
        private class CloseSettings
        {
            public static void Prefix(GameSettingMenu __instance)
            {
                LobbyInfoPane.Instance.EditButton.gameObject.SetActive(true);
                SaveSettings();
            }
        }

        private static void SaveSettings()
        {
            try
            {
                var builder = new StringBuilder();
                foreach (var option in CustomOption.AllOptions)
                {
                    if (option.Type is CustomOptionType.Header) continue;
                    builder.AppendLine(option.Name);
                    builder.AppendLine($"{option.Value}");
                }

                var path = Path.Combine(Application.persistentDataPath, SettingsFileName);
                File.WriteAllText(path, builder.ToString());
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error saving settings: {e.Message}");
            }
        }

        internal static void LoadSettings()
        {
            try
            {
                var path = Path.Combine(Application.persistentDataPath, SettingsFileName);
                if (!File.Exists(path)) return;

                isLoadingSettings = true;

                var text = File.ReadAllText(path);
                var splitText = text.Split("\n").ToList();

                while (splitText.Count > 0)
                {
                    var name = splitText[0].Trim();
                    splitText.RemoveAt(0);
                    var option = CustomOption.AllOptions.FirstOrDefault(o => o.Name.Equals(name, StringComparison.Ordinal));
                    if (option == null)
                    {
                        try { splitText.RemoveAt(0); } catch { }
                        continue;
                    }

                    var value = splitText[0];
                    splitText.RemoveAt(0);
                    switch (option.Type)
                    {
                        case CustomOptionType.Number:
                            option.Set(float.Parse(value), false);
                            break;
                        case CustomOptionType.Toggle:
                            option.Set(bool.Parse(value), false);
                            break;
                        case CustomOptionType.String:
                            option.Set(int.Parse(value), false);
                            break;
                    }
                }

                isLoadingSettings = false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Error loading settings: {e.Message}");
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        private class SettingsUpdate
        {
            public static List<PassiveButton> Buttons = new List<PassiveButton>();
            public static List<GameObject> Tabs = new List<GameObject>();

            public static void Postfix(GameSettingMenu __instance)
            {
                LobbyInfoPane.Instance.EditButton.gameObject.SetActive(false);
                Buttons.ForEach(x => x?.Destroy());
                Tabs.ForEach(x => x?.Destroy());
                Buttons = new List<PassiveButton>();
                Tabs = new List<GameObject>();

                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

                var modSettingsButtonLocal = GameObject.Find("GamePresetButton")?.transform.localPosition;
                GameObject.Find("GamePresetButton")?.Destroy();
                __instance.ChangeTab(1, false);

                var settingsButton = GameObject.Find("GameSettingsButton");

                var panel = GameObject.Find("LeftPanel");
                var button = GameObject.Find("ModSettings");
                if (button == null)
                {
                    button = GameObject.Instantiate(settingsButton, panel.transform);
                    button.transform.localPosition = modSettingsButtonLocal.Value;
                    button.name = "ModSettings";
                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p => { button.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "Mod Settings"; })));
                    var passiveButton = button.GetComponent<PassiveButton>();
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener((System.Action)(() =>
                    {
                        __instance.ChangeTab(3, false);
                    }));
                    passiveButton.SelectButton(false);
                    Buttons.Add(passiveButton);
                }

                var settingsTab = GameObject.Find("GAME SETTINGS TAB");
                Tabs.RemoveAll(x => x == null);
                var tab = GameObject.Instantiate(settingsTab, settingsTab.transform.parent);
                tab.name = "ModSettings";
                var tabOptions = tab.GetComponent<GameOptionsMenu>();
                foreach (var child in tabOptions.Children) child.Destroy();
                tabOptions.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
                tabOptions.Children.Clear();

                var options = CustomOption.AllOptions.ToList();

                float num = 1.5f;

                foreach (CustomOption option in options)
                {
                    if (option.Type == CustomOptionType.Header)
                    {
                        CategoryHeaderMasked header = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(tabOptions.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                        header.SetHeader(StringNames.ImpostorsCategory, 20);
                        header.Title.text = option.Name;
                        header.transform.localScale = Vector3.one * 0.65f;
                        header.transform.localPosition = new Vector3(-0.9f, num, -2f);
                        num -= 0.625f;
                        continue;
                    }

                    else if (option.Type == CustomOptionType.Number)
                    {
                        OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<NumberOption>(tabOptions.numberOptionOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                        optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                        optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                        SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                        for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                        var numberOption = optionBehaviour as NumberOption;
                        option.Setting = numberOption;

                        tabOptions.Children.Add(optionBehaviour);
                    }

                    else if (option.Type == CustomOptionType.Toggle)
                    {
                        OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<ToggleOption>(tabOptions.checkboxOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                        optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                        optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                        SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                        for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                        var toggleOption = optionBehaviour as ToggleOption;
                        option.Setting = toggleOption;

                        tabOptions.Children.Add(optionBehaviour);
                    }

                    else if (option.Type == CustomOptionType.String)
                    {
                        var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                        if (option.Name.StartsWith("Slot "))
                        {
                            try
                            {
                                int slotNumber = int.Parse(option.Name[5..]);
                                if (slotNumber > playerCount) continue;
                            }
                            catch { }
                        }

                        OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(tabOptions.stringOptionOrigin, Vector3.zero, Quaternion.identity, tabOptions.settingsContainer);
                        optionBehaviour.transform.localPosition = new Vector3(0.95f, num, -2f);
                        optionBehaviour.SetClickMask(tabOptions.ButtonClickMask);
                        SpriteRenderer[] components = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                        for (int i = 0; i < components.Length; i++) components[i].material.SetInt(PlayerMaterial.MaskLayer, 20);

                        var stringOption = optionBehaviour as StringOption;
                        option.Setting = stringOption;

                        tabOptions.Children.Add(optionBehaviour);
                    }

                    num -= 0.45f;
                    tabOptions.scrollBar.SetYBoundsMax(-num - 1.65f);
                    option.OptionCreated();
                }

                for (int i = 0; i < tabOptions.Children.Count; i++)
                {
                    OptionBehaviour optionBehaviour = tabOptions.Children[i];
                    if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost) optionBehaviour.SetAsPlayer();
                }

                Tabs.Add(tab);
                tab.SetActive(false);
            }

        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
        class SetTabPane
        {
            public static bool Prefix(LobbyViewSettingsPane __instance)
            {
                if ((int)__instance.currentTab < 6)
                {
                    ChangeTabPane.Postfix(__instance, __instance.currentTab);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
        class ChangeTabPane
        {
            public static void Postfix(LobbyViewSettingsPane __instance, StringNames category)
            {
                int tab = (int)category;

                foreach (var button in SettingsAwake.Buttons) button.SelectButton(false);
                if (tab > 5) return;
                __instance.taskTabButton.SelectButton(false);

                if (tab > 0)
                {
                    tab -= 1;
                    SettingsAwake.Buttons[tab].SelectButton(true);
                    SettingsAwake.AddSettings(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
        class UpdatePane
        {
            public static void Postfix(LobbyViewSettingsPane __instance)
            {
                if (SettingsAwake.Buttons.Count == 0) SettingsAwake.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
        class SettingsAwake
        {
            public static List<PassiveButton> Buttons = new List<PassiveButton>();

            public static void Postfix(LobbyViewSettingsPane __instance)
            {
                Buttons.ForEach(x => x?.Destroy());
                Buttons.Clear();

                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

                var overview = GameObject.Find("RolesTabs");

                var tab = GameObject.Find("ModTab");
                if (tab == null)
                {
                    tab = GameObject.Instantiate(overview, overview.transform.parent);
                    tab.transform.localPosition = new Vector3(2.1f, 1.404f, 0f);
                    tab.name = "ModTab";
                    __instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p => { tab.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = "Mod Settings"; })));
                    var pTab = tab.GetComponent<PassiveButton>();
                    pTab.OnClick.RemoveAllListeners();
                    pTab.OnClick.AddListener((System.Action)(() => {
                        __instance.ChangeTab((StringNames)1);
                    }));
                    pTab.SelectButton(false);
                    Buttons.Add(pTab);
                }
            }

            public static void AddSettings(LobbyViewSettingsPane __instance)
            {
                var options = CustomOption.AllOptions.ToList();

                float num = 1.5f;
                int headingCount = 0;
                int settingsThisHeader = 0;
                int settingRowCount = 0;

                for (int j = 0; j < __instance.settingsInfo.Count; j++)
                {
                    __instance.settingsInfo[j].gameObject.Destroy();
                }

                __instance.settingsInfo.Clear();

                foreach (var option in options)
                {
                    if (option.Type == CustomOptionType.Header)
                    {
                        if (settingsThisHeader % 2 != 0) num -= 0.85f;
                        CategoryHeaderMasked header = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                        header.SetHeader(StringNames.ImpostorsCategory, 61);
                        header.Title.text = option.Name;
                        header.transform.SetParent(__instance.settingsContainer);
                        header.transform.localScale = Vector3.one;
                        header.transform.localPosition = new Vector3(-9.8f, num, -2f);
                        __instance.settingsInfo.Add(header.gameObject);
                        num -= 1f;
                        headingCount += 1;
                        settingsThisHeader = 0;
                        continue;
                    }

                    else
                    {
                        var playerCount = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                        if (option.Name.StartsWith("Slot "))
                        {
                            continue;
                        }

                        ViewSettingsInfoPanel panel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                        panel.transform.SetParent(__instance.settingsContainer);
                        panel.transform.localScale = Vector3.one;
                        if (settingsThisHeader % 2 != 0)
                        {
                            panel.transform.localPosition = new Vector3(-3f, num, -2f);
                            num -= 0.85f;
                        }
                        else
                        {
                            settingRowCount += 1;
                            panel.transform.localPosition = new Vector3(-9f, num, -2f);
                        }
                        settingsThisHeader += 1;
                        panel.SetInfo(option.StringName, option.ToString(), 61);
                        __instance.settingsInfo.Add(panel.gameObject);
                    }
                }

            float actual_spacing = (headingCount * 1.05f + settingRowCount * 0.85f) / (headingCount + settingRowCount) * 1.01f;
            __instance.scrollBar.CalculateAndSetYBounds(__instance.settingsInfo.Count + (headingCount + settingRowCount) * 2 + headingCount, 2f, 6f, actual_spacing);
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        private class PlayerJoinPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance ||
                    !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost) return;

                Coroutines.Start(RpcUpdateSetting.SendRpc(RecipientId: __instance.myPlayer.OwnerId));
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        private class ToggleButtonPatch
        {
            public static bool Prefix(ToggleOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomToggleOption toggle)
                {
                    toggle.Toggle();
                    if (!isLoadingSettings) SaveSettings();
                    return false;
                }
                if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek || __instance.boolOptionName == BoolOptionNames.VisualTasks ||
                    __instance.boolOptionName == BoolOptionNames.AnonymousVotes || __instance.boolOptionName == BoolOptionNames.ConfirmImpostor) return true;
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
        private class NumberOptionInitialise
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomNumberOption number)
                {
                    __instance.MinusBtn.isInteractable = true;
                    __instance.PlusBtn.isInteractable = true;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        private class NumberOptionPatchIncrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomNumberOption number)
                {
                    number.Increase();
                    if (!isLoadingSettings) SaveSettings();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        private class NumberOptionPatchDecrease
        {
            public static bool Prefix(NumberOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomNumberOption number)
                {
                    number.Decrease();
                    if (!isLoadingSettings) SaveSettings();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
        private class StringOptionPatchIncrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomStringOption str)
                {
                    str.Increase();
                    if (!isLoadingSettings) SaveSettings();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
        private class StringOptionPatchDecrease
        {
            public static bool Prefix(StringOption __instance)
            {
                var option = CustomOption.AllOptions.FirstOrDefault(option => option.Setting == __instance);
                if (option is CustomStringOption str)
                {
                    str.Decrease();
                    if (!isLoadingSettings) SaveSettings();
                    return false;
                }

                return true;
            }
        }
    }
}