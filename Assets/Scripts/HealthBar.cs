using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Slider healthBarSlider;
    Actor currentActor;
    public Slider easeHealthSlider;
    private float lerpDuration = 1.5f;
    private Coroutine lerpCoroutine;

    private void OnEnable()
    {
        EventManager.StartListening("SetMaxHp", OnSetMaxHP);
    }

    void OnSetMaxHP(object obj)
    {
        if (currentActor != null && currentActor.name.Equals("Player"))
        {
            healthBarSlider.maxValue = currentActor.MaxHP;
            healthBarSlider.value = currentActor.MaxHP;
            easeHealthSlider.maxValue = currentActor.MaxHP;
            easeHealthSlider.value = currentActor.MaxHP;
        }
    }

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

                break;
            }
        }

        if (currentActor != null)
        {
            //if (currentActor.name == "Player")
            //{
            //    healthBarSlider = GetComponent<Slider>();
            //}
            //else
            //{
            easeHealthSlider = transform.GetChild(0).GetComponent<Slider>();
            healthBarSlider = transform.GetChild(1).GetComponent<Slider>();

            healthBarSlider.maxValue = currentActor.MaxHP;
            healthBarSlider.value = currentActor.MaxHP;
            easeHealthSlider.maxValue = currentActor.MaxHP;
            easeHealthSlider.value = currentActor.MaxHP;

            //}
        }
        else
        {
            Debug.LogError("Actor component not found in any parent GameObject.");
        }
    }

    void Update()
    {
        if (healthBarSlider.value != currentActor.CurrentHP)
        {
            healthBarSlider.value = currentActor.CurrentHP;

            if (easeHealthSlider != null)
            {
                if (lerpCoroutine != null)
                {
                    StopCoroutine(lerpCoroutine);
                }

                lerpCoroutine = StartCoroutine(LerpEaseHealth(healthBarSlider.value));
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
        float startValue = easeHealthSlider.value;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            easeHealthSlider.value = Mathf.Lerp(startValue, targetHealth, elapsedTime / lerpDuration);
            yield return null;
        }

        easeHealthSlider.value = targetHealth;
        lerpCoroutine = null;
    }
}
