using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private GameObject weaponSocket;

    // Start is called before the first frame update
    void Start()
    {
        weaponSocket = GameObject.FindGameObjectWithTag("WeaponSocket");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Player")
        {
            Transform parentTransform = weaponSocket.transform;

            // Iterate through all children
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                // Get the child at index i
                Transform child = parentTransform.GetChild(i);

                // Do something with the child
                Debug.Log("Child " + i + ": " + child.gameObject.name);
                if (child.gameObject.name == "Club")
                {
                    child.gameObject.GetComponent<WeaponScript>().isInPlayerInventory = true;
                }
            }

            Destroy(gameObject);
        }
    }
}
