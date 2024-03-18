using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public enum State { Start, PlayerTurn, ComTurn, End }
public class ComBanPickController : MonoBehaviour//, IPointerClickHandler
{
    private StateMachine<State> BanPickMachune = new StateMachine<State>();
    public State curPickState = State.Start;
    [SerializeField] TeamPick teamPick;
    [SerializeField] List<GameObject> prefabs;
    Camera mainCamera;
    int playercount = 3;
    int comcount = 3;
    [SerializeField] Button GameStart;
    private void Start()
    {
        BanPickMachune.AddState(State.Start, new StartState(this));
        BanPickMachune.AddState(State.PlayerTurn, new PlayerTurnState(this));
        BanPickMachune.AddState(State.ComTurn, new ComTurnState(this));
        BanPickMachune.AddState(State.End, new EndState(this));

        BanPickMachune.Start(State.Start);
        mainCamera = Camera.main;
    }
    private void Update()
    {
        BanPickMachune.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector3.forward, 100f, LayerMask.NameToLayer("UI"));
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (hit.collider == null)
                {
                    Debug.Log("부딫히지 못함");
                }
                else if (hit.collider.gameObject.name == prefabs[i].name)
                {
                    // 1.
                    prefabs.RemoveAt(i);
                    playercount--;
                    // enemy enemycount++;
                }
            }
        }
    }


    public void ComPickStart(int randomPick)
    {
        comPickroutine = StartCoroutine(ComPickRoutine(randomPick));
    }
    public void ComPickStop()
    {
        StopCoroutine(comPickroutine);
    }
    Coroutine comPickroutine;

    IEnumerator ComPickRoutine(int randomPick)
    {
        yield return new WaitForSeconds(0.5f);
        prefabs[randomPick].GetComponent<TeamPick>().ComPick();
        prefabs[randomPick].GetComponent<Collider2D>().enabled = false;
        prefabs.RemoveAt(randomPick);
        comcount--;
    }
    /*    public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log(eventData.position);
                RaycastHit2D hit = Physics2D.Raycast(eventData.position, Vector3.down, 100f, LayerMask.NameToLayer("UI"));
                Debug.Log(hit.collider.gameObject.name);
            }
        }*/

    private class BanPickState : BaseState<State>
    {
        protected ComBanPickController controller;
        public BanPickState(ComBanPickController controller)
        {
            this.controller = controller;
        }
    }
    private class StartState : BanPickState
    {
        public StartState(ComBanPickController controller) : base(controller)
        {
        }
        public override void Enter()
        {
            Debug.Log("시작");
        }
        public override void Exit()
        {

        }
        public override void Transition()
        {
            controller.BanPickMachune.ChangeState(State.PlayerTurn);
        }
    }
    private class PlayerTurnState : BanPickState
    {
        // 플레이어 선택상태
        // 턴에 하나씩, 총 세 번
        // 한명을 뽑게 되면 상대 턴으로 전이?
        int curPlayercount;
        public PlayerTurnState(ComBanPickController controller) : base(controller)
        {
        }
        public override void Enter()
        {
            Debug.Log("플레이어 픽");

            curPlayercount = controller.playercount;
            Debug.Log($"처음 플레이어 카운트 {curPlayercount}");
        }
        public override void Transition()
        {
            if (curPlayercount > controller.playercount)
                controller.BanPickMachune.ChangeState(State.ComTurn);
        }

    }
    private class ComTurnState : BanPickState
    {
        int curComPickcount;
        int randomPick;
        public ComTurnState(ComBanPickController controller) : base(controller)
        {
        }
        public override void Enter()
        {
            curComPickcount = controller.comcount;
            randomPick = Random.Range(0, controller.prefabs.Count);
            Debug.Log("컴터 픽");
            controller.ComPickStart(randomPick);
        }



        public override void Transition()
        {
            if (controller.comcount != 0 && curComPickcount > controller.comcount)
            {
                controller.BanPickMachune.ChangeState(State.PlayerTurn);
            }
            else if (controller.comcount == 0)
            {
                controller.BanPickMachune.ChangeState(State.End);
            }
        }
        public override void Exit()
        {
            controller.ComPickStop();
        }
    }
    private class EndState : BanPickState
    {
        public EndState(ComBanPickController controller) : base(controller)
        {
        }
        public override void Enter()
        {
            Debug.Log("게임 시작");
            controller.GameStart.onClick?.Invoke();
        }
    }

}
