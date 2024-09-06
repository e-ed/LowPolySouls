using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireScript : MonoBehaviour
{
    BoxCollider collider;
    GameObject fire;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        fire = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            // TODO: save player position
            // inside player class?
            // also add a panel to show bonfire lit
            fire.SetActive(true);
        }
    }
}
