using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 화살 관리
public class Test : ObjectPool
{
    private void Awake()
    {     
        CreatePool(prefab, size, capacity);
    }

}
