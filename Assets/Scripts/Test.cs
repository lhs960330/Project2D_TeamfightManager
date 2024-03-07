using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 화살 관리
public class Test : ObjectPool
{
    [Range(0f, 1f)] public float range;
    private void Awake()
    {     
        CreatePool(prefab, size, capacity);
    }
    private void Update()
    {
        
    }

}
