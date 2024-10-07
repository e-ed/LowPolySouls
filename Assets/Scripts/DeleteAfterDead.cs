using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterDead : StateMachineBehaviour
{
    AnimatorStateInfo animStateInfo;
    public float NTime;


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        NTime = animStateInfo.normalizedTime;
        if (NTime > 1.0f && animStateInfo.IsName("Dying")) Destroy(animator.gameObject);
    }

}
