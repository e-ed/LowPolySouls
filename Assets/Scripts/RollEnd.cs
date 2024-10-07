using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollEnd : StateMachineBehaviour
{
    AnimatorStateInfo animStateInfo;
    private float NTime;
    private PlayerScript player;
    private bool hasGrantedOneExtraRollThisTime;
    public Transform cam;

    //// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cam = animator.gameObject.transform;
        PlayerScript player = animator.gameObject.GetComponent<PlayerScript>();
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = verticalInput * camForward;
        Vector3 rightRelative = horizontalInput * camRight;
        Vector3 movementDirection = forwardRelative + rightRelative;

        Vector3 inputDir = new Vector3(movementDirection.x, 0f, movementDirection.z).normalized;


        // Add a boost in the direction of the input while rolling
        Vector3 boostForce = inputDir * player.rollSpeed;
        player.GetComponent<Rigidbody>().AddForce(boostForce, ForceMode.Impulse);
        player.Stamina -= player.rollStaminaCost;
        player.lastRollTime = Time.time;
    }

  
}
