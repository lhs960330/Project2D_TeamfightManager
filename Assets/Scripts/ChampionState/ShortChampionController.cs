using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ShortChampionController : MonoBehaviour
{
    // 적들을 모아둠
    public List<ChampionData> Enemy;
    // 상태들 모아둠
    public enum State { Idle, Find, Move, Attack, Die }

    // 상태들 저장할 상태머신을 가져옴
    private StateMachine<State> stateMachine = new StateMachine<State>();

    // 각 챔피언에 스택이 담긴 클래스를 가져옴 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }
    ChampionData targetEnemy;
    public ChampionData TargetEnemy { get { return targetEnemy; } set { targetEnemy = value; } }
    // 적의 위치를 가져온다.
    Transform enemyPos;
    public Transform EnemyPos { get { return enemyPos; } set { enemyPos = value; } }

    private void Awake()
    {

    }
    private void Start()
    {
        // 각 상태들을 상태머신에 저장
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Die, new DieState(this));

        // 첫 상태를 가져옴
        stateMachine.Start(State.Find);
        Enemy = new List<ChampionData>();

    }

    private void Update()
    {
        // 상태가 변할때마다 확인해줌
        stateMachine.Update();
    }
    public void Attack()
    {

        // 어택 코루틴을 사용하기 위해 만듬
        if (attackRouine == null)
        {
            attackRouine = StartCoroutine(AttackRoutine());
        }
    }

    public void StopAttack()
    {
        Debug.Log("공격 중지");
        // 끝나면 멈추게 해줌
        if (attackRouine != null)
            StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (EnemyPos)
        {
            // 현재상태가 어택일때만 코루틴을 사용
            if (stateMachine.CheckState(State.Attack))
            {
                data.animator.Play("Attack");
                yield return new WaitForSeconds(data.attackTime);
            }
            else
            {
                yield return null;
            }
        }
    }
    public void HitDamag()
    {
        targetEnemy.hp -= data.damage;
    }
    private class ChampionState : BaseState<State>
    {

        protected ShortChampionController controller;
        protected Vector3 dir;

        // 여기서 초기화
        public ChampionState(ShortChampionController owner)
        {
            this.controller = owner;
        }
    }
    // 가만히있는 상태
    private class IdleState : ChampionState
    {
        public IdleState(ShortChampionController owner) : base(owner)
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
        public FindState(ShortChampionController owner) : base(owner)
        {
        }

        public override void Enter()
        {
            // 메니저에 있는 데이터를 가져와서 팀 구분
            foreach (ChampionData a in Manager.Game.championDatas)
            {
                if (controller.data.Team != a.Team)
                {
                    controller.Enemy.Add(a);
                }
            }

            if(controller.data.Team == 0 && Manager.Game.countBteam == 0)
            {// A팀이며 상대방 팀이 0명
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if( controller.data.Team == 1 && Manager.Game.countAteam == 0)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else
            {
                controller.EnemyPos = controller.Enemy[0].transform;
                controller.targetEnemy = controller.Enemy[0];
            }
            // 가까운 적 찾기
            foreach (ChampionData a in controller.Enemy)
            {
                if (a == null)
                    return;

                // 처음 지정한 친구와 나머지 다 비교
                if (Vector3.Distance(controller.transform.position, controller.EnemyPos.position) >= Vector3.Distance(controller.transform.position, a.gameObject.GetComponent<Transform>().position))
                {
                    // 가장 가까운 적 저장
                    controller.EnemyPos = a.transform;
                    controller.targetEnemy = a;
                }
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
            if (controller.TargetEnemy == null)
            {
                controller.stateMachine.ChangeState(State.Idle);

            }
            else
            {
                controller.stateMachine.ChangeState(State.Move);
            }
        }
        public override void Exit()
        {
        }
    }
    private class MoveState : ChampionState
    {
        public MoveState(ShortChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            dir = (controller.enemyPos.position - controller.transform.position).normalized;
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
            // 적이 사라졋을때(죽었을때)
            if (controller.EnemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);

            }
            // 사거리안에 들어왔을때 공격으로
            if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {
            if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.data.animator.Play("Idle");
            }
        }
    }
    private class AttackState : ChampionState
    {
        public AttackState(ShortChampionController owner) : base(owner)
        {
        }

        // 때리는 액션
        public override void Enter()
        {
            // 어택 코루틴 시작
            controller.Attack();
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            if (controller.EnemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            // 떨어졌을때 안떄리기
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
            }
        }
    }

    // 죽을때
    private class DieState : ChampionState
    {
        public DieState(ShortChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            // 죽인다.
            controller.data.animator.Play("Die");
            Manager.Game.RemoveChampion(this.controller.data);
            Destroy(controller.gameObject, 1f);
        }

    }
}
