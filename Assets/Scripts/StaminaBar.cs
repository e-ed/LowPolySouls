using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public int maxStamina = 100;
    public int stamina;
    public Slider staminaBarSlider;
    Actor currentActor;
    public Slider easeStaminaSlider;
    private float lerpDuration = 0.15f;
    private Coroutine lerpCoroutine;
    private PlayerScript playerScript;

    void Start()
    {
        Transform currentParent = transform.parent;

        while (currentParent != null)
        {
            Transform nextParent = currentParent.parent;

            if (nextParent != null)
            {
                currentParent = nextParent;
            }
            else
            {
                GameObject topmostParent = currentParent.gameObject;

                if (topmostParent.name == "Canvas")
                {
                    currentActor = GameObject.Find("Player").GetComponent<Actor>();
                }
                else
                {
                    currentActor = topmostParent.GetComponent<Actor>();
                }

                playerScript = currentActor.GetComponent<PlayerScript>();

                break;
            }
        }

        if (currentActor != null)
        {

            easeStaminaSlider = transform.GetChild(0).GetComponent<Slider>();
            staminaBarSlider = transform.GetChild(1).GetComponent<Slider>();

        }
        else
        {
            Debug.LogError("Actor component not found in any parent GameObject.");
        }
    }


    void Update()
    {
        if (staminaBarSlider.value != playerScript.Stamina)
        {
            staminaBarSlider.value = playerScript.Stamina;

            if (easeStaminaSlider != null)
            {
                if (lerpCoroutine != null)
                {
                    StopCoroutine(lerpCoroutine);
                }

                lerpCoroutine = StartCoroutine(LerpEaseHealth(staminaBarSlider.value));
            }
        }



    }

    void TakeDamage(int damage)
    {
        currentActor.CurrentHP -= damage;
    }

    IEnumerator LerpEaseHealth(float targetHealth)
    {
        float elapsedTime = 0f;
        float startValue = easeStaminaSlider.value;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            easeStaminaSlider.value = Mathf.Lerp(startValue, targetHealth, elapsedTime / lerpDuration);
            yield return null;
        }

        easeStaminaSlider.value = targetHealth;
        lerpCoroutine = null;
    }
}
