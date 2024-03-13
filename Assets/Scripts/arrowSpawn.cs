using UnityEngine;


// È­»ì ¸®Á¨
public class arrowSpawn : ObjectPool
{
    private void Awake()
    {
        CreatePool(prefab, size, capacity);
    }
    
    
    
}
