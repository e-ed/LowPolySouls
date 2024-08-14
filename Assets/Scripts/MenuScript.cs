using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{

    public GameObject panel;
    public GameObject playerStats;
    public CinemachineFreeLook vcam;
    public void handlePlayButton()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void handleExitButton()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Menu")
        {
            Cursor.visible = !Cursor.visible;
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                vcam.enabled = false;
                Time.timeScale = 0;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                vcam.enabled = true;
                Time.timeScale = 1;

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
    
}
