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
    private float nextAttack;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        nextAttack = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<EnemyScript>().CurrentHP <= 0)
        {
            return;
        }


        // Get the speed from the NavMeshAgent
        float speed = agent.velocity.magnitude;

        // Set animator parameter based on the speed
        animator.SetFloat("Horizontal", speed);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < 10 && !gameObject.GetComponent<EnemyScript>().isAttacking)
        {
            agent.SetDestination(playerTransform.position);
        }


        if (nextAttack > 0)
        {
            nextAttack -= Time.deltaTime;
        }

        // Use the distance in your logic or animations
        if (distanceToPlayer < 2 && nextAttack <= 0)
        {
            animator.SetTrigger("Attack");
            nextAttack = cooldown;
        }


    }
}
