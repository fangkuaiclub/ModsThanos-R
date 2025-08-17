namespace ModsThanos.Stone.System.Mind {
    public class PlayerData {
        public byte PlayerId;
        public int PlayerColor;
        public string PlayerHat;
        public string PlayerPet;
        public string PlayerSkin;
        public string PlayerName;

        public PlayerData(byte playerId, int playerColor, string playerHat, string playerPet, string playerSkin, string playerName) {
            PlayerId = playerId;
            PlayerColor = playerColor;
            PlayerHat = playerHat;
            PlayerPet = playerPet;
            PlayerSkin = playerSkin;
            PlayerName = playerName;
        }
    }
}
