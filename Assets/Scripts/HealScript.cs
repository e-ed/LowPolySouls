using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealScript : StateMachineBehaviour
{
    private GameObject shieldSocket;
    public TextMeshProUGUI charges;
    PlayerScript player;

    private void OnEnable()
    {
        shieldSocket = GameObject.Find("Shield Socket").transform.GetChild(0).gameObject;
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        //charges = GameObject.Find("Charges").GetComponent<TextMeshProUGUI>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       player.isHealing = true;
       shieldSocket.SetActive(false);
        player.CurrentHP += 30;
        if (player.CurrentHP > player.MaxHP)
        {
            player.CurrentHP = player.MaxHP;
        }
        player.flaskCharges--;
        //charges.SetText(player.flaskCharges.ToString());
        EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shieldSocket?.SetActive(true);
        player.isHealing = false;

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
