using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class ChampionController : MonoBehaviour
{
    public enum State { Idle, Find, Move, Attack, Die }

    private StateMachine<State> stateMachine = new StateMachine<State>();

    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }

    [SerializeField] GameObject enemy;
    public GameObject Enemy { get { return enemy; } set { enemy = value; } }


    private void Start()
    {
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Die, new DieState(this));


        stateMachine.Start(State.Find);
    }

    private void Update()
    {
        stateMachine.Update();
    }
    public void Attack()
    {
        attackRouine = StartCoroutine(AttackRoutine());

    }
    public void StopAttack()
    {
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (Enemy.gameObject)
        {
            data.animator.Play("Attack");

            Debug.Log("겁나 때린다.");
            Enemy.GetComponent<ChampionData>().hp -= data.damage;
            yield return new WaitForSeconds(data.attackTime);

        }
    }
    private class ChampionState : BaseState<State>
    {

        protected ChampionController controller;
        // 적의 위치를 가져온다.
        protected Transform enemyPos;

        protected Vector3 dir;

        // 여기서 초기화
        public ChampionState(ChampionController owner)
        {
            this.controller = owner;
            enemyPos = owner.Enemy.transform;
        }
    }
    // 가만히있는 상태
    private class IdleState : ChampionState
    {
        public IdleState(ChampionController owner) : base(owner)
        {
        }
        public override void Update()
        {

            if (controller.data.hp >= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
            // 적을 찾는 상태로 변환해준다.
            controller.stateMachine.ChangeState(State.Find);
        }
    }
    private class FindState : ChampionState
    {
        public FindState(ChampionController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // 적이 있으면 위치 추적 게임오브젝트에 위치를 가져오고 없으면 가만히 있는 상태로 간다.
            if (controller.Enemy == true)
            {
                enemyPos = controller.Enemy.transform;
            }
            else
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }

        public override void Update()
        {
            // 뒤졌을때
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
            // 적의 위치가 있으면 이동상태로 변환 그 이외에는 가만히있는 상태
            if (enemyPos != null)
            {
                controller.stateMachine.ChangeState(State.Move);
            }
            else
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }
    private class MoveState : ChampionState
    {
        public MoveState(ChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
        }

        public override void Update()
        {

            // 상대방한테 이동
            dir = (enemyPos.position - controller.transform.position).normalized;
            controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
        }
        public override void Transition()
        {
            // 사거리안에 들어왔을때 공격으로
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(ChampionController owner) : base(owner)
        {
        }

        // 때리는 액션
        public override void Enter()
        {

            Debug.Log("때린다.");
            controller.Attack();
        }
        public override void Update()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            // 떨어졌을때 안떄리기
            if (controller.Enemy == true && Vector3.Distance(enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                Debug.Log("떨어짐");
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }

    // 죽을때
    private class DieState : ChampionState
    {
        public DieState(ChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            // 죽인다.
            controller.data.animator.Play("Die");
            Destroy(controller.gameObject, 1f);
        }
        
    }


}
