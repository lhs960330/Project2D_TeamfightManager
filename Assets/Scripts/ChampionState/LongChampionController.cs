using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class LongChampionController : MonoBehaviour
{
    // 상태들 모아둠
    public enum State { Idle, Find, Move, Attack, Avoid, Die }

    // 상태들 저장할 상태머신을 가져옴
    private StateMachine<State> stateMachine = new StateMachine<State>();

    // 각 챔피언에 스택이 담긴 클래스를 가져옴 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }

    // 상대방에 게임오브젝트를 가져옴
    [SerializeField] GameObject enemy;
    public GameObject Enemy { get { return enemy; } set { enemy = value; } }



    private void Start()
    {
        // 각 상태들을 상태머신에 저장
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Avoid, new AvoidState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        // 첫 상태를 가져옴
        stateMachine.Start(State.Find);
    }

    private void Update()
    {
        // 상태가 변할때마다 확인해줌
        stateMachine.Update();
    }
    public void Attack()
    {
        // 어택 코루틴을 사용하기 위해 만듬
        attackRouine = StartCoroutine(AttackRoutine());
    }
    public void StopAttack()
    {
        // 끝나면 멈추게 해줌
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (Enemy.gameObject)
        {
            // 어택 애니메이션을 실행해줌
            data.animator.Play("Attack");
            // 어택될떄마다 체력이 깍임
            Enemy.GetComponent<ChampionData>().hp -= data.damage;
            // 각 어택마다 시간을 줌
            yield return new WaitForSeconds(data.attackTime);

        }
    }
    private class ChampionState : BaseState<State>
    {

        protected LongChampionController controller;
        // 적의 위치를 가져온다.
        protected Transform enemyPos;

        protected Vector3 dir;

        protected Vector3 startPos;
        // 여기서 초기화
        public ChampionState(LongChampionController owner)
        {
            this.controller = owner;
            enemyPos = owner.Enemy.transform;
        }
    }
    // 가만히있는 상태
    private class IdleState : ChampionState
    {
        public IdleState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Idle");
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // 적을 찾는 상태로 변환해준다.
            controller.stateMachine.ChangeState(State.Find);
        }
    }
    private class FindState : ChampionState
    {
        public FindState(LongChampionController owner) : base(owner)
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
        public override void Transition()
        {
            // 뒤졌을때
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
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
        public MoveState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            dir = (enemyPos.position - controller.transform.position).normalized;

        }

        public override void Update()
        {
            // 나와 적 사이에 x가 0보다 작으면 왼쪽을 보고 크면 오른쪽보게한다.
            if (dir.x < 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

            }
            else if (dir.x > 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            // 상대방한테 이동
            controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);

        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            // 사거리안에 들어왔을때 공격으로
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(LongChampionController owner) : base(owner)
        {
        }

        // 때리는 액션
        public override void Enter()
        {
            controller.data.avoidcool = 0;
            controller.Attack();
        }
        public override void Update()
        {
            controller.data.avoidcool += Time.deltaTime;
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // 상대방과 내 거리가 회피거리안에 들어왔을때 회피상태로 바꿔줌
            if (Vector3.Distance(enemyPos.position, controller.transform.position) <= controller.data.range && controller.data.avoidcool > controller.data.attackTime)
            {
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Avoid);
            }
            // 떨어졌을때 안떄리기
            if (controller.Enemy == true && Vector3.Distance(enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.StopAttack();
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }
    //회피가동
    private class AvoidState : ChampionState
    {
        bool curnt;
        public AvoidState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            startPos = controller.transform.position;
            dir = (enemyPos.position - controller.transform.position).normalized;
        }
        public override void Update()
        {
            //반대로 이동 (얼마나?)
            controller.transform.Translate(-dir * controller.data.speed * Time.deltaTime);

            // 나와 적 사이에 x가 0보다 작으면 왼쪽을 보고 크면 오른쪽보게한다.
            // 여기선 그 반대쪽으로 가야되니 -해준다.
            if (-dir.x < 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

            }
            else if (-dir.x > 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            curnt = controller.gameObject.GetComponent<SpriteRenderer>().flipX;
        }
        public override void Exit()
        {
            // 도망이 끝난후에 현재 보는 방향의 반대방향을 잡아준다
            if (curnt == true)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            if (Vector3.Distance(controller.transform.position, startPos) >= 2 && controller.Enemy != null)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
            else if (controller.Enemy == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
    }
    // 죽을때
    private class DieState : ChampionState
    {
        public DieState(LongChampionController owner) : base(owner)
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
