using UnityEngine;


// ȭ�� ����
public class arrowSpawn : ObjectPool
{
    private void Awake()
    {
        CreatePool(prefab, size, capacity);
    }
    
    
    
}
