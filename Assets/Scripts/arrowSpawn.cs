using UnityEngine;


// ȭ�� ����
public class arrowSpawn : ObjectPool
{
    private void Start()
    {
        CreatePool(prefab, size, capacity);
    }
    
    
    
}
