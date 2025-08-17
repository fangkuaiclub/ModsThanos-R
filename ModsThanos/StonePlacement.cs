using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModsThanos.Utility.Enumerations;

namespace ModsThanos.Map {
    public class StonePlacement {
        internal static Vector2 GetRandomLocation(string composent) {
            byte MapID = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            Dictionary<MapType, Vector2[]> mapLocations = new Dictionary<MapType, Vector2[]>();
            Vector2 currentPositon;

            mapLocations.Add(MapType.Skeld, new Vector2[] {
                new Vector2(-12.594f, -4.179f),
                new Vector2(-22.746f, -7.206f),
                new Vector2(-19.269f, -9.544f),
                new Vector2(-7.772f, -11.655f),
                new Vector2(-9.825f, -9.023f),
                new Vector2(-2.874f, -16.936f),
                new Vector2(0.336f, -9.152f),
                new Vector2(5.731f, -9.607f),
                new Vector2(-5.044f, -2.642f),
                new Vector2(-1.013f, 5.975f),
                new Vector2(10.772f, 1.998f),
                new Vector2(6.409f, -4.710f),
                new Vector2(17.997f, -5.689f),
                new Vector2(11.165f, -10.334f),
                new Vector2(7.522f, -14.285f),
                new Vector2(1.707f, -14.937f),
                new Vector2(-3.685f, -11.657f),
                new Vector2(-14.163f, -6.832f),
                new Vector2(-18.551f, 2.625f),
                new Vector2(-7.556f, -2.124f)
            });

            mapLocations.Add(MapType.Polus, new Vector2[] {
                new Vector2(4.458f, -3.387f),
                new Vector2(3.801F, -7.584F),
                new Vector2(7.193f, -13.089f),
                new Vector2(4.042f, -11.233f),
                new Vector2(0.660f, -15.868f),
                new Vector2(1.516f, -18.669f),
                new Vector2(2.331f, -24.491f),
                new Vector2(9.231f, -25.351f),
                new Vector2(12.681f, -24.541f),
                new Vector2(12.489f, -17.160f),
                new Vector2(17.958f, -25.710f),
                new Vector2(22.225f, -25.008f),
                new Vector2(23.933f, -20.589f),
                new Vector2(31.295f, -11.321f),
                new Vector2(18.055f, -13.020f),
                new Vector2(12.892f, -17.317f),
                new Vector2(6.582f -17,113f),
                new Vector2(23.639f, -2.799f),
                new Vector2(24.928f, -6.877f),
                new Vector2(32.344f, -10.047f),
                new Vector2(34.852f, -5.208f),
                new Vector2(40.516f, -8.102f),
                new Vector2(36.291f, -22.012f)
            });

            mapLocations.Add(MapType.MiraHQ, new Vector2[] {
                new Vector2(18.266f, -3.223f),
                new Vector2(28.257f, -2.250f),
                new Vector2(18.293f, 5.045f),
                new Vector2(28.276f, 2.735f),
                new Vector2(17.843f, 11.516f),
                new Vector2(13.750f, 17.214f),
                new Vector2(22.387f, 19.160f),
                new Vector2(13.862f, 23.878f),
                new Vector2(19.330f, 25.309f),
                new Vector2(16.177f, 3.085f),
                new Vector2(16.752f, -1.455f),
                new Vector2(11.755f, 10.300f),
                new Vector2(11.112f, 14.068f),
                new Vector2(2.444f, 13.352f),
                new Vector2(0.414f, 10.087f),
                new Vector2(-5.780f, -2.037f),
                new Vector2(16.752f, -1.455f),
                new Vector2(10.161f, 5.162f)
            });

            mapLocations.Add(MapType.Airship, new Vector2[] {
                new Vector2(-22.642f, 0.820f),
                new Vector2(-13.074f, -5.146f),
                new Vector2(-12.302f, 3.273f),
                new Vector2(-13.344f, -7.588f),
                new Vector2(-16.793f, -12.845f),
                new Vector2(10.594f, -14.732f),
                new Vector2(17.413f, -4.067f),
                new Vector2(35.906f, -3.038f),
                new Vector2(32.374f, 7.335f),
                new Vector2(21.236f, 12.213f),
                new Vector2(13.918f, 6.277f),
                new Vector2(-11.523f, 10.934f),
                new Vector2(-3.683f, 1.307f),
                new Vector2(6.495f, 14.094f),
                new Vector2(-5.720f, -8.783f),
                new Vector2(1.595f, -12.322f),
                new Vector2(-2.306f, 1.375f),
                new Vector2(5.633f, 3.567f),
                new Vector2(9.869f, 3.694f),
                new Vector2(12.012f, 2.922f),
                new Vector2(14.695f, 2.995f),
                new Vector2(20.673f, 2.658f)
            });

            mapLocations.Add(MapType.Fungle, new Vector2[] {
                new Vector2(-14.123f, 7.488f),
                new Vector2(-11.515f, 12.118f),
                new Vector2(-16.979f, 0.981f),
                new Vector2(-17.081f, -1.974f),
                new Vector2(-17.799f, -9.275f),
                new Vector2(2.125f, -1.243f),
                new Vector2(-5.174f, -9.659f),
                new Vector2(4.791f, -5.325f),
                new Vector2(8.195f, -9.661f),
                new Vector2(8.830f, -11.405f),
                new Vector2(8.296f, -15.493f),
                new Vector2(20.408f, -11.596f),
                new Vector2(21.669f, -6.103f),
                new Vector2(23.891f, -7.819f),
                new Vector2(9.908f, 1.042f),
                new Vector2(9.455f, 4.623f),
                new Vector2(13.748f, 10.369f),
                new Vector2(22.890f, 2.296f),
                new Vector2(24.414f, 14.683f),
                new Vector2(20.106f, 14.458f),
                new Vector2(2.352f, 4.035f),
                new Vector2(-19.717f, -3.981f)
            });

            bool RerollPosition;
            List<Vector2> vectors = mapLocations[(MapType) MapID].ToList();
            do {
                RerollPosition = false;
                var random = new System.Random();
                currentPositon = vectors[random.Next(vectors.Count())];

                if (GlobalVariable.stonePositon == null)
                    break;

                foreach (KeyValuePair<string, Vector2> element in GlobalVariable.stonePositon) {
                    float positionBeetween = Vector2.Distance(element.Value, currentPositon);
                    if (positionBeetween == 1f) {
                        RerollPosition = true;
                        vectors.Remove(currentPositon);
                    }
                }
            } while (RerollPosition);

            if (!GlobalVariable.stonePositon.ContainsKey(composent))
                GlobalVariable.stonePositon.Add(composent, currentPositon);

            return currentPositon;
        }

        internal static Dictionary<string, Vector2> SetAllStonePositions() {
            foreach (var stone in GlobalVariable.stonesNames) {
                Vector2 position = GetRandomLocation(stone);

                if (!GlobalVariable.stonePositon.ContainsKey(stone))
                    GlobalVariable.stonePositon.Add(stone, position);
            }

            return GlobalVariable.stonePositon;
        }

        internal static void PlaceAllStone() {
            Stone.Map.Mind.Place(GlobalVariable.stonePositon["Mind"]);
            Stone.Map.Power.Place(GlobalVariable.stonePositon["Power"]);
            Stone.Map.Soul.Place(GlobalVariable.stonePositon["Soul"]);
            Stone.Map.Time.Place(GlobalVariable.stonePositon["Time"]);
            Stone.Map.Space.Place(GlobalVariable.stonePositon["Space"]);
            Stone.Map.Reality.Place(GlobalVariable.stonePositon["Reality"]);
        }
    }
}
