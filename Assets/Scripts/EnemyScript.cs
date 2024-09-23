using UnityEngine;

public class EnemyScript : Actor
{
    Animator animator;
    public bool hasDied = false;
    AudioSource audioSource;
    public int soulsDrop;
    public EnemyStats stats;
    public float SlowDown;


    public void ApplyStats()
    {
        if (stats != null)
        {
            Level = stats.Level;
            Strength = stats.Strength;
            Dexterity = stats.Dexterity;
            Intelligence = stats.Intelligence;
            MaxHP = stats.MaxHP;
            CurrentHP = MaxHP;
            SlowDown = stats.SlowDown;
        }
    }


    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        ApplyStats();

    }

    void Start()
    {
    }

    private void Step()
    {
        audioSource.Play();
    }

    private void Update()
    {
        if (CurrentHP <= 0 && !hasDied)
        {
            hasDied = true;
            animator.SetTrigger("Dead");

            EventManager.TriggerEvent("EnemyDied", soulsDrop);
        }
    }


}
