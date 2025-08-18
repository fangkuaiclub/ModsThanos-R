using ModsThanos.CustomOption;
using ModsThanos.Utility.Enumerations;
using UnityEngine;
using static ModsThanos.Utility.Utils;

namespace ModsThanos {
    public static class CustomGameOptions {
        public static string[] visibilityValue = new string[] { ColorString(Color.white, "visibilityValue1"), ColorString(new Color(0.749f, 0f, 0.839f, 1f), "visibilityValue2"), ColorString(Palette.CrewmateBlue, "visibilityValue3") };

        public static CustomHeaderOption ModHeader = new CustomHeaderOption(1, ColorString(Color.cyan, "ModHeader"));
        public static CustomToggleOption EnableThanosMods = new CustomToggleOption(2, ColorString(Color.yellow, "EnableThanosMods"), true);
        public static CustomStringOption ThanosSide = new CustomStringOption(3, ColorString(Color.yellow, "ThanosSide"), new string[] { ColorString(Palette.ImpostorRed, "ThanosSide1"), ColorString(Palette.CrewmateBlue, "ThanosSide2") });
        public static CustomNumberOption NumberThanos = new CustomNumberOption(4, ColorString(Color.yellow, "NumberThanos"), 1f, 1f, 10f, 1f);
        public static CustomToggleOption DisableSnap = new CustomToggleOption(5, ColorString(Color.yellow, "DisableSnap"), false);

        public static CustomHeaderOption StoneHeader = new CustomHeaderOption(10, ColorString(Color.cyan, "StoneHeader"));
        public static CustomNumberOption CooldownTimeStone = new CustomNumberOption(11, ColorString(Color.yellow, "CooldownTimeStone"), 30f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption CooldownRealityStone = new CustomNumberOption(12, ColorString(Color.yellow, "CooldownRealityStone"), 10f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption CooldownSoulStone = new CustomNumberOption(13, ColorString(Color.yellow, "CooldownSoulStone"), 30f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption CooldownSpaceStone = new CustomNumberOption(14, ColorString(Color.yellow, "CooldownSpaceStone"), 30f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption CooldownMindStone = new CustomNumberOption(15, ColorString(Color.yellow, "CooldownMindStone"), 30f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption CooldownPowerStone = new CustomNumberOption(16, ColorString(Color.yellow, "CooldownPowerStone"), 30f, 10f, 300f, 5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption TimeDuration = new CustomNumberOption(17, ColorString(Color.yellow, "TimeDuration"), 5f, 5f, 40f, 2.5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption RealityDuration = new CustomNumberOption(18, ColorString(Color.yellow, "RealityDuration"), 10f, 5f, 40f, 2.5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption MindDuration = new CustomNumberOption(19, ColorString(Color.yellow, "MindDuration"), 4f, 5f, 40f, 2.5f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption MaxPortal = new CustomNumberOption(20, ColorString(Color.yellow, "MaxPortal"), 4f, 1f, 20f, 1f, CustomOption.CustomOption.CooldownFormat);
        public static CustomNumberOption StoneSize = new CustomNumberOption(21, ColorString(Color.yellow, "StoneSize"), 320f, 50f, 1000f, 10f, value => $"{value:0#}f");

        public static CustomHeaderOption VisibilityHeader = new CustomHeaderOption(30, ColorString(Color.cyan, "VisibilityHeader"));
        public static CustomStringOption VisibilityTime = new CustomStringOption(31, ColorString(Color.yellow, "VisibilityTime"), visibilityValue);
        public static CustomStringOption VisibilityPower = new CustomStringOption(32, ColorString(Color.yellow, "VisibilityPower"), visibilityValue);
        public static CustomStringOption VisibilityMind = new CustomStringOption(33, ColorString(Color.yellow, "VisibilityMind"), visibilityValue);
        public static CustomStringOption VisibilitySoul = new CustomStringOption(34, ColorString(Color.yellow, "VisibilitySoul"), visibilityValue);
        public static CustomStringOption VisibilitySpace = new CustomStringOption(35, ColorString(Color.yellow, "VisibilitySpace"), visibilityValue);
        public static CustomStringOption VisibilityReality = new CustomStringOption(36, ColorString(Color.yellow, "VisibilityReality"), visibilityValue);

        public static Visibility VisibilityStringToEnum(int visibility) {
            return visibility switch {
                0 => Visibility.Everyone,
                1 => Visibility.OnlyImpostor,
                2 => Visibility.OnlyCrewmate,
                _ => Visibility.OnlyImpostor,
            };
        }

        public static Visibility SideStringToEnum(int visibility) {
            return visibility switch
            {
                0 => Visibility.OnlyImpostor,
                1 => Visibility.OnlyCrewmate,
                _ => Visibility.OnlyImpostor,
            };
        }
    }
}
