using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LongChampionController : MonoBehaviour
{
    // 적들을 모아둠
    public List<ChampionData> Enemy = new List<ChampionData>();
    // 상태들 모아둠
    public enum State { Idle, Find, Move, Attack, Avoid, Die }

    // 상태들 저장할 상태머신을 가져옴
    private StateMachine<State> stateMachine = new StateMachine<State>();
    public StateMachine<State> StateMachine { get { return stateMachine; } }

    // 각 챔피언에 스택이 담긴 클래스를 가져옴 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } }
    [SerializeField] LayerMask wallLayer;

    // 에로우 포인트를 가져옴
    [SerializeField] arrowSpawn arrowPoint;
    public arrowSpawn ArrowPoint { get { return arrowPoint; } set { arrowPoint = value; } }
    ChampionData targetEnemy;
    public ChampionData TargetEnemy { get { return targetEnemy; } set { targetEnemy = value; } }
    // 적의 위치를 가져온다.
    Transform enemyPos;
    public Transform EnemyPos { get { return enemyPos; } set { enemyPos = value; } }

    private bool isAttack;
    public bool IsAttack { get { return isAttack; } }
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

    private void OnTriggerEnter(Collider other)
    {

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
        // 끝나면 멈추게 해줌
        if (attackRouine != null)
        {
            StopCoroutine(attackRouine);
        }
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (EnemyPos)
        {
            if (stateMachine.CheckState(State.Attack))
            {
                isAttack = false;
                // 어택 애니메이션을 실행해줌
                data.animator.Play("Attack");
                if (arrowPoint != null)
                {
                    PooledObject pooledObject = arrowPoint.GetPool(arrowPoint.transform.position, arrowPoint.transform.rotation);
                }
                yield return new WaitForSeconds(data.attackTime);
                isAttack = true;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return null;
            }
        }

    }
    public void HitDamag()
    {
        enemyPos.GetComponent<ChampionData>().hp -= data.damage;
    }

    private class ChampionState : BaseState<State>
    {

        protected LongChampionController controller;


        protected Vector3 dir;

        protected Vector3 startPos;

        // 에로우포인가 공격 모션 활 앞으로 가게 만들기위한 변수들
        protected Vector3 arrowPos;
        protected bool isOK;
        // 여기서 초기화
        public ChampionState(LongChampionController owner)
        {
            this.controller = owner;
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
            // 메니저에 있는 데이터를 가져와서 팀 구분
            foreach (ChampionData a in Manager.Game.championDatas)
            {
                if (controller.data.Team != a.Team)
                {
                    if (controller.Enemy.Contains(a))
                        continue;
                    controller.Enemy.Add(a);
                }
            }

            if (controller.data.Team == 0 && Manager.Game.countBuleteam == 0)
            {// A팀이며 상대방 팀이 0명
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if (controller.data.Team == 1 && Manager.Game.countRedteam == 0)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else
            {
                controller.EnemyPos = controller.Enemy[0].transform;
                controller.targetEnemy = controller.Enemy[0];
            }
            // 이래도 없으면 계속 대기
            if (controller.EnemyPos == null)
                controller.stateMachine.ChangeState(State.Idle);
            // 가까운 적 찾기
            foreach (ChampionData a in controller.Enemy)
            {
                if (controller.EnemyPos == null)
                    continue;
                if (a == null)
                {
                    controller.Enemy.Remove(a);
                }
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

    }
    private class MoveState : ChampionState
    {
        public MoveState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            controller.data.animator.Play("Move");
            // 가까운 친구한와 거리 계산
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
            else if (controller.enemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
                            }
            // 사거리안에 들어왔을때 공격으로
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
            }
            if (controller.targetEnemy == null)
            {
                controller.Enemy.Remove(controller.targetEnemy);
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
            // 오브젝트의 flipx의 상태를 확인
            isOK = controller.gameObject.GetComponent<SpriteRenderer>().flipX;

            arrowPos = controller.ArrowPoint.transform.localPosition;
            // 오브젝트의 flipx의 따라 화살이 나가는 point를 바꿔줌
            if (isOK == true)
            {
                if (controller.arrowPoint.transform.localPosition.x > 0)
                    controller.arrowPoint.transform.localPosition = -arrowPos;
                else
                    controller.arrowPoint.transform.localPosition = arrowPos;
            }
            else if (isOK == false)
            {
                if (controller.arrowPoint.transform.localPosition.x < 0)
                    controller.arrowPoint.transform.localPosition = -arrowPos;
                else
                    controller.arrowPoint.transform.localPosition = arrowPos;
            }
            // 코루틴이 실행됐을때 화살이 적에게 날아감
            controller.Attack();
        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            if (controller.enemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) >= controller.data.range)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            // 상대방과 내 거리가 회피거리안에 들어오고 한대때리고 떄린 시간만큼 기다렸다가 도망 회피상태로 바꿔줌
            else if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= 3f && controller.IsAttack)
            {
                controller.stateMachine.ChangeState(State.Avoid);
            }

        }
        public override void Exit()
        {
            if (controller.Enemy == null)
            {
                controller.StopAttack();
            }
            if (controller.targetEnemy == null)
            {
                controller.Enemy.Remove(controller.targetEnemy);
            }
        }
    }
    //회피가동
    private class AvoidState : ChampionState
    {
        bool curnt;
        RaycastHit2D hit;
        Vector3 hitDir;
        public AvoidState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {

            controller.data.animator.Play("Move");
            startPos = controller.transform.position;
            dir = (controller.enemyPos.position - startPos).normalized;
            hit = Physics2D.Raycast(startPos, -dir, 3f, controller.wallLayer);
            if (hit)
            {
                hitDir = ((Vector3)hit.point - controller.transform.position).normalized;
            }
        }
        public override void Update()
        {
            //반대로 이동 (얼마나?)
            if (hit)
            {
                Vector3 perpendicular = new Vector3(hitDir.y, -hitDir.x, hitDir.z);
                perpendicular.Normalize();
                controller.transform.Translate(perpendicular * controller.data.speed * Time.deltaTime);
                if (perpendicular.x < 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

                }
                else if (perpendicular.x > 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            else
            {
                controller.transform.Translate(-dir * controller.data.speed * Time.deltaTime);
                if (-dir.x < 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

                }
                else if (-dir.x > 0)
                {
                    controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
            // 나와 적 사이에 x가 0보다 작으면 왼쪽을 보고 크면 오른쪽보게한다.
            // 여기선 그 반대쪽으로 가야되니 -해준다.

            curnt = controller.gameObject.GetComponent<SpriteRenderer>().flipX;
        }


        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            else if (controller.EnemyPos == null)
            {
                controller.stateMachine.ChangeState(State.Idle);
            }
            else if (Vector3.Distance(controller.transform.position, startPos) >= 2)
            {
                controller.stateMachine.ChangeState(State.Attack);
            }
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
            if (controller.Enemy == null)
            {
                controller.StopAttack();
                controller.Enemy.Remove(controller.targetEnemy);
            }
        }
    }
    // 죽을때
    private class DieState : ChampionState
    {

        bool isDie = true;
        public DieState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            if (isDie)
            {
                if (controller.data.Team == 0)
                {
                    Manager.Game.SetBuleScore();
                }
                else
                {
                    Manager.Game.SetRedScore();
                }
                controller.StopAttack();
                isDie = false;
                controller.data.animator.Play("Die");
                Manager.Game.RemoveChampion(this.controller.data);
                Destroy(controller.gameObject, 1f);
            }
        }

    }
}
