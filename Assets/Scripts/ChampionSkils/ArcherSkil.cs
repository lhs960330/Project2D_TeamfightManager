using System;
using System.Collections;
using UnityEngine;

public class ArcherSkil : MonoBehaviour
{
    [SerializeField] LongChampionController controller;
    // 아처 스킬 : 상대방을 경직시키고 뒤로 물러난다. 쿨타임 3초
    // 상대방이 가까이 왔을때 스킬 사용

    private void Start()
    {
        if(skilCoroutine == null)
        skilCoroutine = StartCoroutine(SkilRutine());
        
    }
    Coroutine skilCoroutine;
    IEnumerator SkilRutine()
    {
        int loop = 0;
        if (controller.StateMachine.CheckState(LongChampionController.State.Avoid))
        {
            while (controller.StateMachine.CheckState(LongChampionController.State.Avoid))
            {
                loop++;
                if (loop > 10000)
                    throw new InvalidOperationException("A");
                controller.Data.animator.Play("Skil");
                yield return new WaitForSeconds(3);
            }
        }
        else
        {
            yield return null;
        }

    }

    
}
