using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactPanel;

    public bool isPlayer(Collider other)
    {
        return other.gameObject.name.Equals("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayer(other)) return;
        interactPanel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayer(other)) return;
        interactPanel.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayer(other))
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactPanel.SetActive(false);
                gameObject.GetComponent<Animator>().SetTrigger("open");
            }
        }
    }
}
