using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ComBanPickController : MonoBehaviour
{
    public enum State { Start, PlayerTurn, ComTurn, End }
    private StateMachine<State> BanPickMachune = new StateMachine<State>();
    private void Start()
    {
        BanPickMachune.AddState(State.Start, new StartState(this));
        BanPickMachune.AddState(State.PlayerTurn, new PlayerTurnState(this));
        BanPickMachune.AddState(State.ComTurn, new ComTurnState(this));
        BanPickMachune.AddState(State.End, new EndState(this));

        BanPickMachune.Start(State.Start);
        
    }
    private void Update()
    {
        BanPickMachune.Update();
    }

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
            
        }
        public override void Exit()
        {
            controller.BanPickMachune.ChangeState(State.PlayerTurn);
        }
    }
    private class PlayerTurnState : BanPickState
    {
        public PlayerTurnState(ComBanPickController controller) : base(controller)
        {
        }
        public override void Exit()
        {
           
            controller.BanPickMachune.ChangeState(State.ComTurn);
        }
    }
    private class ComTurnState : BanPickState
    {
        int pickCount = 2;
        public ComTurnState(ComBanPickController controller) : base(controller)
        {
        }

        public override void Enter()
        {
            if(pickCount ==0)
            {
                controller.BanPickMachune.ChangeState(State.End);
            }
            else if(pickCount > 0)
            {
                controller.BanPickMachune.ChangeState(State.PlayerTurn);
            }
        }
    }
    private class EndState : BanPickState
    {
        public EndState(ComBanPickController controller) : base(controller)
        {
        }
    }

}
