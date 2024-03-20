using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : Actor
{
    Animator animator;
    public bool hasDied = false;
    AudioSource audioSource;

    void Awake()
    {
        MaxHP = 100;
        CurrentHP = 100;
        Level = 1;
        Strength = 3;
        Dexterity = 3;
        Intelligence = 3;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
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
        }
    }




}
