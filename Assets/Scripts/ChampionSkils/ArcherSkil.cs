/*using System;
using System.Collections;
using UnityEngine;

public class ArcherSkil : MonoBehaviour
{
    [SerializeField] LongChampionController controller;
    Vector3 dir;
    // ��ó ��ų : ������ ������Ű�� �ڷ� ��������. ��Ÿ�� 3��
    // ������ ������ ������ ��ų ���
    bool isRoutine = false;

    private void Start()
    {
    }

    private void Update()
    {
        // ���� ��Ÿ�� �����̶��
        if (isRoutine == true)
        {
            Debug.Log($"���� ���� : {controller.StateMachine.CurState}");
            Debug.Log("��Ÿ����..");
            return;
        }

        if (controller.StateMachine.CheckState(LongChampionController.State.Avoid))
        { 
            Debug.Log("ȸ�� ����");
            StartCoroutine(CoolTime());             // ��Ÿ�� ����
            controller.Data.animator.Play("Skill"); // ��ų�� ����
        }
    }


    IEnumerator CoolTime()
    {
        isRoutine = true;
        yield return new WaitForSeconds(3f);
        isRoutine = false;
    }
}
*/