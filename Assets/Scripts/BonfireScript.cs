using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonfireScript : MonoBehaviour
{
    BoxCollider col;
    GameObject fire;
    public GameObject levelUpPanel;
    public GameObject interactPanel;
    private TextMeshProUGUI interactText;
    public CinemachineFreeLook vcam;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        fire = gameObject.transform.GetChild(0).gameObject;
        interactText = interactPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        interactPanel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        interactPanel.SetActive(false);

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
                interactPanel.SetActive(false);
                player.CurrentHP = player.MaxHP;
                player.flaskCharges = 10;
                levelUpPanel.SetActive(true);
                Cursor.visible = !Cursor.visible;
                Cursor.lockState = CursorLockMode.Confined;
                vcam.enabled = false;
                Time.timeScale = 0;
                player.flaskCharges = 10;
                EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);
            }
        }
    }
}
