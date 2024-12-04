using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonfireScript : Interactable
{
    BoxCollider col;
    GameObject fire;
    public GameObject levelUpPanel;
    public GameObject playerItems;
    private TextMeshProUGUI interactText;
    public CinemachineFreeLook vcam;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        fire = gameObject.transform.GetChild(0).gameObject;
        interactText = interactPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    override
    public void DoInteract(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {

            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            fire.SetActive(true);
            EventManager.TriggerEvent("flaskChargesChanged", player.flaskCharges);
            playerItems.SetActive(false);
            interactPanel.SetActive(false);
            player.CurrentHP = player.MaxHP;
            player.flaskCharges = 10;
            levelUpPanel.SetActive(true);
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = CursorLockMode.Confined;
            vcam.enabled = false;
            Time.timeScale = 0;
            player.flaskCharges = 10;

        }
    }
}
