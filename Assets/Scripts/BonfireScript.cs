using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireScript : MonoBehaviour
{
    BoxCollider col;
    GameObject fire;
    public GameObject levelUpPanel;
    public CinemachineFreeLook vcam;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        fire = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            // TODO: save player position
            // inside player class? then read from file
            // also add a panel to show bonfire lit
            fire.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                player.CurrentHP = player.MaxHP;
                levelUpPanel.SetActive(true);
                Cursor.visible = !Cursor.visible;
                Cursor.lockState = CursorLockMode.Confined;
                vcam.enabled = false;
                Time.timeScale = 0;
            }
        }
    }
}
