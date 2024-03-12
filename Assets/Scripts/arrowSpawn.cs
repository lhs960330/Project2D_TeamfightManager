using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

// È­»ì ¸®Á¨
public class arrowSpawn : ObjectPool
{

    Transform target;
    public void SetEnemy(Transform enemy)
    {
        target = enemy;
    }
    public Transform GetEnemy()
    {
        return target;
    }
    private void Awake()
    {     
        CreatePool(prefab, size, capacity);
    }

}
