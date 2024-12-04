using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameObject interactPanel;
    private bool shouldCheckForInput = false;
    private Collider currentCollider = null;

    void Update()
    {
        if (!shouldCheckForInput) return;

        if (Input.GetKeyDown(KeyCode.E) && interactPanel.activeSelf)
        {
            DoInteract(currentCollider);
        }
    }

    public bool isPlayer(Collider other)
    {
        return other.gameObject.name.Equals("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayer(other)) return;
        shouldCheckForInput = true;
        currentCollider = other;
        interactPanel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayer(other)) return;
        shouldCheckForInput = false;
        currentCollider = null;
        interactPanel.SetActive(false);
    }

    public abstract void DoInteract(Collider collider);
}
