using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro damageText;
    public float initialVerticalSpeed = 0.05f;
    public float horizontalSpeed = 0.02f;
    public float acceleration = -0.001f; // Negative for deceleration

    private Vector3 movementDirection;
    private float currentVerticalSpeed;

    private void Start()
    {
        damageText = GetComponent<TextMeshPro>();

        // Randomly choose left or right
        float horizontalDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        movementDirection = new Vector3(horizontalSpeed * horizontalDirection, initialVerticalSpeed, 0);
        currentVerticalSpeed = initialVerticalSpeed;
    }

    private void FixedUpdate()
    {
        if (damageText != null)
        {
            // Apply acceleration to the vertical speed
            currentVerticalSpeed += acceleration;
            movementDirection.y = currentVerticalSpeed;

            // Update position
            Vector3 newPosition = transform.position + movementDirection;
            transform.position = newPosition;

            // fade-out effect
            Color color = damageText.color;
            color.a -= Time.deltaTime * 0.5f;
            damageText.color = color;

            // transparent
            if (color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
