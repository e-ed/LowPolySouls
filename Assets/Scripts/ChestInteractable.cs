using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : Interactable
{
    override
     public void DoInteract(Collider collider)
    {
        GameObject.FindWithTag("WeaponSocket").transform.GetChild(3).GetComponent<WeaponScript>().isInPlayerInventory = true;
        interactPanel.SetActive(false);
        Invoke("destroyChestGameObject", 1);
    }

    private void destroyChestGameObject()
    {
        Destroy(gameObject);
    }
}
