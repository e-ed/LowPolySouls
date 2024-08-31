using UnityEngine;

public class AttackEnd : StateMachineBehaviour
{
    private GameObject weapon;
    private GameObject weaponSocket;
    private float weaponSpeed;
    private int previousStateHash;
    public void OnEnable()
    {
        weaponSocket = GameObject.FindGameObjectWithTag("WeaponSocket");
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
        previousStateHash = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash;

        animator.gameObject.GetComponent<Actor>().isAttacking = true;
        animator.SetBool("isAttacking", true);


        weapon = weaponSocket.GetComponent<WeaponManager>().currentWeapon;
        weaponSpeed = weapon.GetComponent<WeaponScript>().weaponSpeed;
        animator.SetFloat("weaponSpeed", weaponSpeed);


        // Check if the weapon GameObject was found
        if (weapon != null)
        {
            //Debug.Log("Weapon GameObject found: " + weapon.name);

            // Access the MeshCollider and enable it
            MeshCollider collider = weapon.GetComponent<MeshCollider>();
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
        MeshCollider collider = weapon.GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        else
        {
            Debug.LogError("MeshCollider not found on the weapon GameObject.");
        }

        // Access the PlayerScript and set isAttacking to false
        PlayerScript playerScript = animator.gameObject.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
        else
        {
            animator.gameObject.GetComponent<EnemyScript>().isAttacking = false;
        }

    }
}
