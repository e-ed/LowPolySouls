using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public int Level;
    public int Strength;
    public int Dexterity;
    public int Intelligence;
    public int MaxHP;
    public float SlowDown;
}
