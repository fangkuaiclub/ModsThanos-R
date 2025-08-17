using ModsThanos.CustomOption;
using ModsThanos.Utility.Enumerations;

namespace ModsThanos {
    public static class CustomGameOptions {
        public static string[] visibilityValue = new string[] { "Everyone", "Thanos", "Crewmate" };

        public static CustomHeaderOption ModHeader = new CustomHeaderOption("Thanos Mod Settings");
        public static CustomToggleOption EnableThanosMods = new CustomToggleOption("Enable Thanos Mod", true);
        public static CustomStringOption ThanosSide = new CustomStringOption("Thanos Side", new string[] { "Impostor", "Crewmate" });
        public static CustomNumberOption NumberThanos = new CustomNumberOption("Thanos Number", 1f, 1f, 10f, 1f);
        public static CustomToggleOption DisableSnap = new CustomToggleOption("Disable Snap", false);

        public static CustomHeaderOption StoneHeader = new CustomHeaderOption("Stone Settings");
        public static CustomNumberOption CooldownTimeStone = new CustomNumberOption("Cooldown Time Stone", 30f, 10f, 300f, 5f);
        public static CustomNumberOption CooldownRealityStone = new CustomNumberOption("Cooldown Reality Stone", 10f, 10f, 300f, 5f);
        public static CustomNumberOption CooldownSoulStone = new CustomNumberOption("Cooldown Soul Stone", 30f, 10f, 300f, 5f);
        public static CustomNumberOption CooldownSpaceStone = new CustomNumberOption("Cooldown Space Stone", 30f, 10f, 300f, 5f);
        public static CustomNumberOption CooldownMindStone = new CustomNumberOption("Cooldown Mind Stone", 30f, 10f, 300f, 5f);
        public static CustomNumberOption CooldownPowerStone = new CustomNumberOption("Cooldown Power Stone", 30f, 10f, 300f, 5f);
        public static CustomNumberOption TimeDuration = new CustomNumberOption("Time Duration", 5f, 5f, 40f, 2.5f);
        public static CustomNumberOption RealityDuration = new CustomNumberOption("Reality Duration", 10f, 5f, 40f, 2.5f);
        public static CustomNumberOption MindDuration = new CustomNumberOption("Mind Duration", 4f, 5f, 40f, 2.5f);
        public static CustomNumberOption MaxPortal = new CustomNumberOption("Max Portal", 4f, 1f, 20f, 1f);
        public static CustomNumberOption StoneSize = new CustomNumberOption("Stone Size", 320f, 50f, 1000f, 10f);

        public static CustomHeaderOption VisibilityHeader = new CustomHeaderOption("Visibility Settings");
        public static CustomStringOption VisibilityTime = new CustomStringOption("Time Stone Visibility", visibilityValue);
        public static CustomStringOption VisibilityPower = new CustomStringOption("Power Stone Visibility", visibilityValue);
        public static CustomStringOption VisibilityMind = new CustomStringOption("Mind Stone Visibility", visibilityValue);
        public static CustomStringOption VisibilitySoul = new CustomStringOption("Soul Stone Visibility", visibilityValue);
        public static CustomStringOption VisibilitySpace = new CustomStringOption("Space Stone Visibility", visibilityValue);
        public static CustomStringOption VisibilityReality = new CustomStringOption("Reality Stone Visibility", visibilityValue);

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
