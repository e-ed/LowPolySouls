using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EstusScript : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void OnFlaskChargesChanged(object obj)
    {
        text.SetText(obj.ToString());

    }

    private void OnEnable()
    {
        EventManager.StartListening("flaskChargesChanged", OnFlaskChargesChanged);
    }


    private void OnDisable()
    {
        EventManager.StopListening("flaskChargesChanged", OnFlaskChargesChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
