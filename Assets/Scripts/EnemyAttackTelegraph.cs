using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTelegraph : StateMachineBehaviour
{
    float slowDown;
    float timePassed;
    GameObject weapon;

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }

        return null;
    }


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapon = FindChildWithTag(animator.gameObject, "Weapon");
        weapon.GetComponent<WeaponScript>().ResetDamageFlag();
        animator.gameObject.GetComponent<EnemyNavScript>().hasSetTrigger = false;
        slowDown = animator.gameObject.GetComponent<EnemyScript>().stats.SlowDown;
        timePassed = 0;
        animator.SetFloat("Telegraph", 0.1f);
        animator.gameObject.GetComponent<Actor>().isAttacking = true;


    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timePassed < slowDown)
        {
            timePassed += Time.deltaTime;
        } else
        {
            animator.SetFloat("Telegraph", 1);
            weapon.GetComponent<Collider>().enabled = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Actor>().isAttacking = false;
        if (weapon == null) return;
        Collider collider = weapon.GetComponent<Collider>();
        if (collider  != null && !animator.gameObject.GetComponent<EnemyScript>().isBossType) {
            weapon.GetComponent<Collider>().enabled = false;
        }
    }

}
