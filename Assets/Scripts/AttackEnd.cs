using Unity.VisualScripting;
using UnityEngine;

public class AttackEnd : StateMachineBehaviour
{
    private GameObject weapon;
    private GameObject weaponSocket;
    private float weaponSpeed;
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
        animator.gameObject.GetComponent<Actor>().isAttacking = true;
        // Find the weapon GameObject as a child with the "Weapon" tag

        if (animator.gameObject.name == "Player")
        {
            weapon = weaponSocket.GetComponent<WeaponManager>().currentWeapon;
            weaponSpeed = weapon.GetComponent<WeaponScript>().weaponSpeed;
            animator.SetFloat("weaponSpeed", weaponSpeed);
        }
        else
        {
            weapon = FindChildWithTag(animator.gameObject, "Weapon");
        }
        

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
        // Check if the AttackTrigger is still active
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
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
            }
            else
            {
                animator.gameObject.GetComponent<EnemyScript>().isAttacking = false;
            }
        }
    }
}
