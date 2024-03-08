using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class LongChampionController : MonoBehaviour
{
    // ���µ� ��Ƶ�
    public enum State { Idle, Find, Move, Attack, Avoid, Die }

    // ���µ� ������ ���¸ӽ��� ������
    private StateMachine<State> stateMachine = new StateMachine<State>();

    // �� è�Ǿ� ������ ��� Ŭ������ ������ 
    [SerializeField] ChampionData data;
    public ChampionData Data { get { return data; } set { data = value; } }

    // ���濡 ���ӿ�����Ʈ�� ������
    ChampionData[] enemy;
    public ChampionData[] Enemy { get { return enemy; } set { enemy = value; } }

    // ���ο� ����Ʈ�� ������
    [SerializeField] arrowSpawn arrowPoint;
    ChampionData targetEnemy;

    // ���� ��ġ�� �����´�.
    Transform enemyPos;
    public Transform EnemyPos { get { return enemyPos; } set { enemyPos = value; } }
    public arrowSpawn ArrowPoint { get { return arrowPoint; } set { arrowPoint = value; } }
    private void Awake()
    {
    }
    private void Start()
    {
        // �� ���µ��� ���¸ӽſ� ����
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Find, new FindState(this));
        stateMachine.AddState(State.Move, new MoveState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Avoid, new AvoidState(this));
        stateMachine.AddState(State.Die, new DieState(this));
        // ù ���¸� ������
        stateMachine.Start(State.Find);
    }

    private void Update()
    {
        // ���°� ���Ҷ����� Ȯ������
        stateMachine.Update();
    }
    public void Attack()
    {
        // ���� �ڷ�ƾ�� ����ϱ� ���� ����
        attackRouine = StartCoroutine(AttackRoutine());
    }
    public void StopAttack()
    {
        // ������ ���߰� ����
        StopCoroutine(attackRouine);
    }
    Coroutine attackRouine;
    IEnumerator AttackRoutine()
    {
        while (targetEnemy.gameObject)
        {
            // ���� �ִϸ��̼��� ��������
            data.animator.Play("Attack");
            arrowPoint.GetPool(arrowPoint.transform.position, arrowPoint.transform.rotation);
            // ���õɶ����� ü���� ����
            targetEnemy.GetComponent<ChampionData>().hp -= data.damage;
            // �� ���ø��� �ð��� ��
            yield return new WaitForSeconds(data.attackTime);
        }
    }

    private class ChampionState : BaseState<State>
    {

        protected LongChampionController controller;


        protected Vector3 dir;

        protected Vector3 startPos;

        // ���ο����ΰ� ���� ��� Ȱ ������ ���� ��������� ������
        protected Vector3 arrowPos;
        protected bool isOK;
        // ���⼭ �ʱ�ȭ
        public ChampionState(LongChampionController owner)
        {
            this.controller = owner;
        }

    }
    // �������ִ� ����
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
            // ���� ã�� ���·� ��ȯ���ش�.
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
            // ������ ��Ƴ��� 
            controller.enemy = Manager.Game.championsdata[!controller.data.team];

            controller.targetEnemy = controller.enemy[0];
            // ��Ƴ��� ������ ���� ����� ģ������ Ÿ������ ����
            foreach (ChampionData a in controller.enemy)
            {
                if (a == null)
                    continue;

                if ((a.gameObject.transform.position - controller.transform.position).magnitude <
                    (controller.targetEnemy.gameObject.transform.position - controller.transform.position).magnitude)
                {

                    controller.enemyPos = a.transform;
                    controller.targetEnemy = a;

                }
            }
            // ���� ������ ��ġ ���� ���ӿ�����Ʈ�� ��ġ�� �������� ������ ������ �ִ� ���·� ����.
            if (controller.targetEnemy != null)
            {
                // ���� ���� �ƴ� ģ�� ã��
                if (Manager.Game.championsdata.ContainsKey(!controller.data.team))
                {
                    controller.enemyPos = controller.targetEnemy.transform;
                }
            }
        }
        public override void Update()
        {

        }
        public override void Transition()
        {

            // ��������
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }
            // ���� ��ġ�� ������ �̵����·� ��ȯ �� �̿ܿ��� �������ִ� ����
            if (controller.enemyPos != null)
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
            dir = (controller.enemyPos.position - controller.transform.position).normalized;

        }

        public override void Update()
        {
            // ���� �� ���̿� x�� 0���� ������ ������ ���� ũ�� �����ʺ����Ѵ�.
            if (dir.x < 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = true;

            }
            else if (dir.x > 0)
            {
                controller.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            // �������� �̵�
            controller.transform.Translate(dir * controller.data.speed * Time.deltaTime);

        }
        public override void Transition()
        {
            if (controller.data.hp <= 0)
            {
                controller.stateMachine.ChangeState(State.Die);
            }

            // ��Ÿ��ȿ� �������� ��������
            if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range)
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

        // ������ �׼�
        public override void Enter()
        {
            // ������Ʈ�� flipx�� ���¸� Ȯ��
            isOK = controller.gameObject.GetComponent<SpriteRenderer>().flipX;

            arrowPos = controller.ArrowPoint.transform.localPosition;
            // ������Ʈ�� flipx�� ���� ȭ���� ������ point�� �ٲ���
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
            // �ڷ�ƾ�� ��������� ȭ���� ������ ���ư�
            controller.Attack();
            // �Ѵ�ġ�� ������ ��Ÿ��
            controller.data.avoidcool = 0;


        }
        public override void Update()
        {
            // �ð����� ���
            controller.data.avoidcool += Time.deltaTime;
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
            // ���������� �ȋ�����
            else if (controller.enemyPos != null)
            {
                if (controller.targetEnemy == true && Vector3.Distance(controller.enemyPos.position, controller.transform.position) >= controller.data.range)
                {
                    controller.stateMachine.ChangeState(State.Idle);
                }
                // ����� �� �Ÿ��� ȸ�ǰŸ��ȿ� ������ �Ѵ붧���� ���� �ð���ŭ ��ٷȴٰ� ���� ȸ�ǻ��·� �ٲ���
                if (Vector3.Distance(controller.enemyPos.position, controller.transform.position) <= controller.data.range
                    && controller.data.avoidcool > controller.data.attackTime)
                {
                    controller.stateMachine.ChangeState(State.Avoid);
                }
            }

        }
        public override void Exit()
        {
            controller.StopAttack();
        }
    }
    //ȸ�ǰ���
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
            dir = (controller.enemyPos.position - controller.transform.position).normalized;
        }
        public override void Update()
        {
            //�ݴ�� �̵� (�󸶳�?)
            controller.transform.Translate(-dir * controller.data.speed * Time.deltaTime);

            // ���� �� ���̿� x�� 0���� ������ ������ ���� ũ�� �����ʺ����Ѵ�.
            // ���⼱ �� �ݴ������� ���ߵǴ� -���ش�.
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
            // ������ �����Ŀ� ���� ���� ������ �ݴ������ ����ش�
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
    // ������
    private class DieState : ChampionState
    {
        public DieState(LongChampionController owner) : base(owner)
        {
        }
        public override void Enter()
        {
            // ���δ�.
            controller.data.animator.Play("Die");
            Destroy(controller.gameObject, 1f);
        }

    }
}
