using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanPickOnOff : MonoBehaviour
{
    [SerializeField] List<BoxCollider2D> colliders;

    public void OnOff()
    {
        foreach(var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}
