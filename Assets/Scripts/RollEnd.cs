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

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        //animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //NTime = animStateInfo.normalizedTime;
        //// Set canRollAgain to true once the animation is 80% complete
        //if (NTime >= 0.8f && !animator.gameObject.GetComponent<PlayerScript>().canRollAgain && !hasGrantedOneExtraRollThisTime)
        //{
        //    Debug.Log("granting one extra roll at ntime" + NTime);
        //    animator.gameObject.GetComponent<PlayerScript>().canRollAgain = true;
        //    hasGrantedOneExtraRollThisTime = true;
        //}

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    PlayerScript player = animator.gameObject.GetComponent<PlayerScript>();

    //    // Use stateInfo to directly get the state name, ensuring you're getting the current state
    //    string currentStateName = stateInfo.IsName("RollForward") ? "RollForward" : stateInfo.shortNameHash.ToString();

    //    Debug.Log("Exiting state: " + currentStateName);

    //    // If the state isn't the RollForward animation, continue resetting
    //    if (!currentStateName.Equals("RollForward"))
    //    {
    //        player.isRolling = false;
    //        Debug.Log("Reset isRolling to false.");
    //    }
    //}


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
