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

    float offset = -0.5f;


    [SerializeField]
    public float weaponDamage;
    [SerializeField]
    public float weaponSpeed;

    public Boolean isInPlayerInventory = false;

    private float damageCooldown = 2.0f;
    private float damageCooldownTimer = 0.0f;
    private Boolean canPlayTakeDamageAnimation = true;

    public bool hasDealtDamage = false;

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

        if (hasDealtDamage || attackTarget == null || attackTarget.isRolling) return;

        if (!attackSource.name.Equals("Player") && attackTarget.name.Equals("Player"))
        {
            hasDealtDamage = true;
        }

        int attackDamage = ((int)weaponDamage) + (attackSource.Strength * attackSource.Level) + UnityEngine.Random.Range(0, attackSource.Level + 5);

        bool isCritical = UnityEngine.Random.Range(0, 100) < attackSource.Dexterity;

        if (isCritical) attackDamage *= 2;

        attackTarget.CurrentHP -= attackDamage;

        if (attackTarget.CurrentHP < 0)
        {
            if (attackTarget.name.Equals("Player"))
            {
                EventManager.TriggerEvent("PlayerDied", null);
            }
            attackTarget.CurrentHP = 0;
        }

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
        Transform currentParent = other.transform.parent;

        while (currentParent != null)
        {
            Transform nextParent = currentParent.parent;

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
            dynamicDamagePopupText.transform.localPosition = new Vector3(0, 0f, 0);
            if (attackTarget.name == "Player")
            {
                dynamicDamagePopupText.color = Color.red;
                dynamicDamagePopupText.transform.localPosition = new Vector3(dynamicDamagePopupText.transform.localPosition.x, dynamicDamagePopupText.transform.localPosition.y + offset, dynamicDamagePopupText.transform.localPosition.z); 
            }
            dynamicDamagePopupText.fontSize = 2.5f;
            Destroy(dynamicDamagePopup, 1.5f);

        }
    }

    public void ResetDamageFlag()
    {
        hasDealtDamage = false;
    }
}
