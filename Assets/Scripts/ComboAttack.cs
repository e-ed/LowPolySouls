using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : StateMachineBehaviour
{
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
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weapon = FindChildWithTag(animator.gameObject, "Weapon");
        weapon.GetComponent<WeaponScript>().ResetDamageFlag();


    }

}
