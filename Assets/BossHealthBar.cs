using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Slider healthBarSlider;
    EnemyScript currentActor;
    public Slider easeHealthSlider;
    private float lerpDuration = 1.5f;  // Adjust this value as needed
    private Coroutine lerpCoroutine;
    public GameObject bossHealthBar;
    private TextMeshProUGUI bossNameText;

    private void OnEnable()
    {
        EventManager.StartListening("PlayerAggro", OnPlayerAggro);
    }

    void OnPlayerAggro(object obj)
    {
        bossHealthBar.SetActive(true);
        easeHealthSlider = bossHealthBar.transform.GetChild(0).GetComponent<Slider>();
        healthBarSlider = bossHealthBar.transform.GetChild(1).GetComponent<Slider>();
        bossNameText = bossHealthBar.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        currentActor = obj as EnemyScript;

        healthBarSlider.maxValue = currentActor.MaxHP;
        healthBarSlider.value = currentActor.MaxHP;
        easeHealthSlider.maxValue = currentActor.MaxHP;
        easeHealthSlider.value = currentActor.MaxHP;


        healthBarSlider.maxValue = currentActor.MaxHP;
        healthBarSlider.value = currentActor.MaxHP;
        easeHealthSlider.maxValue = currentActor.MaxHP;
        easeHealthSlider.value = currentActor.MaxHP;

        bossNameText.SetText(((EnemyScript) obj).name);



    }

    void Update()
    {
        if (!bossHealthBar.activeSelf)
        {
            return;
        }

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

    private void OnDestroy()
    {
        bossHealthBar.SetActive(false);
    }
}
