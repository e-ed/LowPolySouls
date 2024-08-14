using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavScript : MonoBehaviour
{
    NavMeshAgent agent;
    Transform playerTransform; // Cache player's transform for performance
    Animator animator;
    public float cooldown;
    private float timeUntilNextAttack;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        timeUntilNextAttack = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<EnemyScript>().CurrentHP <= 0)
        {
            return;
        }

        float speed = agent.velocity.magnitude;

        animator.SetFloat("Horizontal", speed);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > 10) return;

        if (gameObject.GetComponent<EnemyScript>().isAttacking)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        agent.SetDestination(playerTransform.position);

        if (timeUntilNextAttack > 0)
        {
            timeUntilNextAttack -= Time.deltaTime;
        }

        if (distanceToPlayer < 2 && timeUntilNextAttack <= 0)
        {
            animator.SetTrigger("Attack");
            timeUntilNextAttack = cooldown;
        }

    }
}
