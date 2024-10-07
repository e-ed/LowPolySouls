using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{

    public GameObject panel;
    public GameObject playerStats;
    public GameObject playerItems;
    public CinemachineFreeLook vcam;
    public GameObject levelUpPanel;
    public PlayerScript player;

    public void handlePlayButton()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void handleExitButton()
    {
        PlayerScript player = FindObjectOfType<PlayerScript>();
        if (player != null)
        {
            // Create a new PlayerData object from the player's data
            PlayerData data = new PlayerData(player);

            // Save the player's data using DataHandler's singleton instance
            DataHandler.Instance.SaveData(data);
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (levelUpPanel.activeSelf)
            {
                levelUpPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                vcam.enabled = true;
                Time.timeScale = 1;
                playerItems.SetActive(true);
                return;
            }

            if (SceneManager.GetActiveScene().name != "Menu")
            {

                Cursor.visible = !Cursor.visible;
                panel.SetActive(!panel.activeSelf);
                if (panel.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    vcam.enabled = false;
                    Time.timeScale = 0;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    vcam.enabled = true;
                    Time.timeScale = 1;

                }
            }
        } else if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = !Cursor.visible;
            playerStats.SetActive(!playerStats.activeSelf);
            if (playerStats.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                vcam.enabled = false;
                Time.timeScale = 0;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                vcam.enabled = true;
                Time.timeScale = 1;
            }
        }
    }

    public void handleLevelUpStrengthButton()
    {
        if (player.canPlayerLevelUp())
        {
            player.IncreaseStrength();
            player.levelUpAndDecreaseSouls();
            EventManager.TriggerEvent("UpdateSoulsPanel", player.Souls);
            EventManager.TriggerEvent("UpdateLevelUpPanel", null);

        }
    }
    public void handleLevelUpDexterityButton()
    {
        if (player.canPlayerLevelUp())
        {
            player.IncreaseDexterity();
            player.levelUpAndDecreaseSouls();
            EventManager.TriggerEvent("UpdateSoulsPanel", player.Souls);
            EventManager.TriggerEvent("UpdateLevelUpPanel", null);
        }
    }

    public void handleLevelUpIntelligenceButton()
    {
        if (player.canPlayerLevelUp())
        {
            player.IncreaseIntelligence();
            player.levelUpAndDecreaseSouls();
            EventManager.TriggerEvent("UpdateSoulsPanel", player.Souls);
            EventManager.TriggerEvent("UpdateLevelUpPanel", null);
        }
    }


}
