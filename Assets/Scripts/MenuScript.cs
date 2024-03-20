using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{

    public GameObject panel;
    public CinemachineFreeLook vcam;
    public void handlePlayButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void handleExitButton()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Menu")
        {
            Cursor.visible = !Cursor.visible;
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Confined;
                vcam.enabled = false;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                vcam.enabled = true;
            }
        }
    }
    
}
