using System;
using System.Collections;
using UnityEngine;

public class ArcherSkil : MonoBehaviour
{
    [SerializeField] LongChampionController controller;
    // ��ó ��ų : ������ ������Ű�� �ڷ� ��������. ��Ÿ�� 3��
    // ������ ������ ������ ��ų ���

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
