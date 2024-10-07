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
    float distanceToPlayer = float.MaxValue;
    public float aggroRadius = 10;
    public bool hasSetTrigger = false;
    public bool hasAggrodOnce = false;


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


        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        }

        if (distanceToPlayer > aggroRadius) return;

        if (gameObject.GetComponent<EnemyScript>().isBossType && (!hasAggrodOnce))
        {
            EventManager.TriggerEvent("PlayerAggro", gameObject.GetComponent<EnemyScript>());
            hasAggrodOnce = true;
        }

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

        if (distanceToPlayer <= (gameObject.GetComponent<NavMeshAgent>().stoppingDistance)) {
            if (hasSetTrigger) return;

            //transform.LookAt(playerTransform);
            //transform.rotation = Quaternion.Slerp(transform.rotation, playerTransform.rotation, 5 * Time.deltaTime);
            Vector3 direction = playerTransform.position - transform.position; // Direction towards the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);    // Create rotation towards the player
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 650 * Time.deltaTime); // Smooth rotation

            if (!hasSetTrigger && timeUntilNextAttack <= 0)
            {
                animator.SetTrigger("Attack");
                timeUntilNextAttack = cooldown;
                hasSetTrigger = true;
            }

        }

    }

    private void SetAttackTrigger()
    {
        animator.SetTrigger("Attack");
    }
}
