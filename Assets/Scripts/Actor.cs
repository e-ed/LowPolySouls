using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    private int maxHP;
    private int currentHP;
    private int level;
    private int strength;
    private int dexterity;
    private int intelligence;
    public bool isAttacking = false;
    public bool isRolling = false;


    // Properties with getters and setters
    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public int CurrentHP
    {
        get { return currentHP; }
        set { currentHP = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public int Strength
    {
        get { return strength; }
        set { strength = value; }
    }

    public int Dexterity
    {
        get { return dexterity; }
        set { dexterity = value; }
    }

    public int Intelligence
    {
        get { return intelligence; }
        set { intelligence = value; }
    }

    public virtual void Attack()
    {
        Debug.Log("actor attacking");
    }
}

