using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesData : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public EnemiesData(Transform transform)
    {
        this.position = transform.position;
        this.rotation = transform.rotation;
        this.scale = transform.localScale;
    }
}
