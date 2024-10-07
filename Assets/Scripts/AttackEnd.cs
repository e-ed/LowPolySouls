using UnityEngine;

public class AttackEnd : StateMachineBehaviour
{
    private GameObject weapon;
    private GameObject weaponSocket;
    private float weaponSpeed;
    private int previousStateHash;
    public bool canRollAgain;
    public GameObject playerGameObject;
    private PlayerScript player;
    public void OnEnable()
    {
        weaponSocket = GameObject.FindGameObjectWithTag("WeaponSocket");
        playerGameObject = GameObject.Find("Player");
        player = playerGameObject.GetComponent<PlayerScript>();

    }

    // Function to find a child with a specific tag
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
        weapon = weaponSocket.GetComponent<WeaponManager>().currentWeapon;
        weaponSpeed = weapon.GetComponent<WeaponScript>().weaponSpeed;

        player.Stamina -= player.attackStaminaCost;
        animator.gameObject.GetComponent<PlayerScript>().isAttacking = true;
        previousStateHash = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash;

        animator.gameObject.GetComponent<Actor>().isAttacking = true;
        animator.SetBool("isAttacking", true);


        
        animator.SetFloat("weaponSpeed", weaponSpeed+(0.04f * animator.gameObject.GetComponent<PlayerScript>().Dexterity));


        // Check if the weapon GameObject was found
        if (weapon != null)
        {
            //Debug.Log("Weapon GameObject found: " + weapon.name);

            // Access the MeshCollider and enable it
            Collider collider = weapon.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
                //Debug.Log("MeshCollider enabled on weapon.");
            }
            else
            {
                Debug.LogError("MeshCollider not found on the weapon GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Weapon GameObject not found with the tag 'Weapon'. Make sure it has the correct tag.");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        string currentStateName = "";

        if (clipInfo != null && clipInfo.Length > 0)
        {
            currentStateName = clipInfo[0].clip.name;
        }


        if (currentStateName.Equals("mixamo.com"))
        {
            return;
        }


        // Access the MeshCollider and disable it
        Collider collider = weapon.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("MeshCollider not found on the weapon GameObject.");
        }


        Actor currentActor = animator.gameObject.GetComponent<Actor>();


        //if (string.IsNullOrEmpty(currentStateName))
        //{
        //    return;
        //}

        collider.enabled = false;
        currentActor.isAttacking = false;
        animator.SetBool("isAttacking", false);


    }

}
