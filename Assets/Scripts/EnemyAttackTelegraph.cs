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
        slowDown = 1.4f;
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
            weapon.GetComponent<MeshCollider>().enabled = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Actor>().isAttacking = false;
        if (weapon == null) return;
        MeshCollider collider = weapon.GetComponent<MeshCollider>();
        if (collider  != null) {
            weapon.GetComponent<MeshCollider>().enabled = false;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
