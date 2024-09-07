using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float zoomSpeed = 5f;
    public float minRadius = 5f;
    public float maxRadius = 20f;

    public void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        if (freeLookCamera.LookAt == null)
        {
            Transform player = GameObject.Find("Player").transform;
            freeLookCamera.LookAt = player;
            freeLookCamera.Follow = player;
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the radius of the Cinemachine FreeLook orbits based on the mouse wheel input
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            freeLookCamera.m_Orbits[0].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[0].m_Radius - zoomInput * zoomSpeed, minRadius, maxRadius);
            freeLookCamera.m_Orbits[1].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[1].m_Radius - zoomInput * zoomSpeed, minRadius, maxRadius);
            freeLookCamera.m_Orbits[2].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[2].m_Radius - zoomInput * zoomSpeed, minRadius, maxRadius);
        }
    }
}
