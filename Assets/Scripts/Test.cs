using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ȭ�� ����
public class Test : ObjectPool
{
    private void Awake()
    {     
        CreatePool(prefab, size, capacity);
    }

}
