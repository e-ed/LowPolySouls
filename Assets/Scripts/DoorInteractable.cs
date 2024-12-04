using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    override
    public void DoInteract(Collider other)
    {
        if (isPlayer(other))
        {
            interactPanel.SetActive(false);
            gameObject.GetComponent<Animator>().SetTrigger("open");
        }
    }
}
