/*using System;
using System.Collections;
using UnityEngine;

public class ArcherSkil : MonoBehaviour
{
    [SerializeField] LongChampionController controller;
    Vector3 dir;
    // 아처 스킬 : 상대방을 경직시키고 뒤로 물러난다. 쿨타임 3초
    // 상대방이 가까이 왔을때 스킬 사용
    bool isRoutine = false;

    private void Start()
    {
    }

    private void Update()
    {
        // 만약 쿨타임 도중이라면
        if (isRoutine == true)
        {
            Debug.Log($"현재 상태 : {controller.StateMachine.CurState}");
            Debug.Log("쿨타임중..");
            return;
        }

        if (controller.StateMachine.CheckState(LongChampionController.State.Avoid))
        { 
            Debug.Log("회피 시작");
            StartCoroutine(CoolTime());             // 쿨타임 시작
            controller.Data.animator.Play("Skill"); // 스킬은 시작
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