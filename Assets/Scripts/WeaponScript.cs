using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class WeaponScript : MonoBehaviour
{
    public Animator animator;

    public GameObject newWeapon;

    GameObject damagePopupPrefab;

    TextMeshPro textMeshPro;

    Vector3 originalPosition;


    [SerializeField]
    public float weaponDamage;
    [SerializeField]
    public float weaponSpeed;

    public Boolean isInPlayerInventory = false;

    private float damageCooldown = 2.0f;
    private float damageCooldownTimer = 0.0f;
    private Boolean canPlayTakeDamageAnimation = true;

    private void Update()
    {

        if (damageCooldownTimer > 0.0f)
        {
            damageCooldownTimer += Time.deltaTime;

            if (damageCooldownTimer > damageCooldown)
            {
                canPlayTakeDamageAnimation = true;
                damageCooldownTimer = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject topMostParentOfAttackSource = findTopMostParent(gameObject);

        Actor attackTarget = other.gameObject.GetComponent<Actor>();
        Actor attackSource = topMostParentOfAttackSource.GetComponent<Actor>();

        if (attackTarget == null) return;

        if (attackTarget.isRolling) return;

        int attackDamage = ((int)weaponDamage) + (attackSource.Strength * attackSource.Level) + UnityEngine.Random.Range(0, attackSource.Level);
        bool isCritical = UnityEngine.Random.Range(0, 100) < 30;
        if (isCritical) attackDamage *= 3;

        attackTarget.CurrentHP -= attackDamage;

        if (attackTarget.CurrentHP < 0) attackTarget.CurrentHP = 0;

        InstantiateDamagePopup(attackDamage, isCritical, attackTarget);

        if (canPlayTakeDamageAnimation)
        {
            attackTarget.GetComponentInChildren<Animator>().SetTrigger("takeDamage");
            canPlayTakeDamageAnimation = false;
            damageCooldownTimer = 0.1f;
        }

    }

    GameObject findTopMostParent(GameObject other)
    {
        // Get the immediate parent
        Transform currentParent = other.transform.parent;

        // Loop until there's no more parent
        while (currentParent != null)
        {
            // Move up the hierarchy
            Transform nextParent = currentParent.parent;

            // If there's another parent, update the reference
            if (nextParent != null)
            {
                currentParent = nextParent;
            }
            else
            {
                return currentParent.gameObject;

            }
        }
        return other.gameObject;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.ToString());
    }



    void InstantiateDamagePopup(int damage, bool isCritical, Actor attackTarget)
    {
        Canvas enemyCanvas = attackTarget.GetComponentInChildren<Canvas>();

        if (enemyCanvas != null)
        {
            UnityEngine.Object dynamicDamagePopup = Instantiate(Resources.Load("Prefabs/Damage Popup"), enemyCanvas.transform, false);
            TextMeshPro dynamicDamagePopupText = dynamicDamagePopup.GetComponentInChildren<TextMeshPro>();
            dynamicDamagePopupText.text = damage.ToString();
            dynamicDamagePopupText.color = isCritical ? Color.yellow : Color.white;
            if (attackTarget.name == "Player") dynamicDamagePopupText.color = Color.red;
            dynamicDamagePopupText.transform.localPosition = new Vector3(0, 0f, 0);
            Destroy(dynamicDamagePopup, 1.5f);

        }
    }
}
