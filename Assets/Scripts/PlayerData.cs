[System.Serializable]
public class PlayerData
{
    public float positionX, positionY, positionZ;
    public int level;
    public int souls;
    public int strength;
    public int dexterity;
    public int intelligence;

    public PlayerData() { }

    public PlayerData(PlayerScript player)
    {
        positionX = player.transform.position.x;
        positionY = player.transform.position.y;
        positionZ = player.transform.position.z;
        level = player.Level;
        souls = player.Souls;
        strength = player.Strength;
        dexterity = player.Dexterity;
        intelligence = player.Intelligence;
    }
}
