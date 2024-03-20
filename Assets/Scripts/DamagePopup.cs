using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro damageText;

    private void Start()
    {
        damageText = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
        }
        }




}
