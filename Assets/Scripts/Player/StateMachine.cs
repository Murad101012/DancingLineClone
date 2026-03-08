using Core;
using Interfaces;
using Player.States;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// It's responsible for if player in be <see cref="PlayerIdleState"/>,
    /// <see cref="PlayerMoveState"/> or <see cref="PlayerDeadState"/>
    /// </summary>
    [RequireComponent(typeof(PlayerCoreLogic))]
    public class StateMachine : MonoBehaviour, IOnRestart, IOnCheckPoint, ILevelState
    {
        private IPlayerState _currentState;
        private PlayerCoreLogic _playerCoreLogic;
        
        //Caching to current States
        private IPlayerState _idleState;
        private IPlayerState _moveState;
        private IPlayerState _deadState;

        private void OnEnable()
        {
            PlayerCoreLogic.Dead += ChangeStateIdle;
            LevelRegistrySo.Instance.Register(this);
        }

        private void Awake()
        {
            _playerCoreLogic = GetComponent<PlayerCoreLogic>();
            //Initializing Caches
            _idleState = new PlayerIdleState(_playerCoreLogic);
            _moveState = new PlayerMoveState(_playerCoreLogic);
            
            //Setting player state default to PlayerMoveState
            ChangeStateIdle();
        }
        
        private void Update()
        {
            _currentState?.StateTick();
        }

        private void OnDisable()
        {
            PlayerCoreLogic.Dead -= ChangeStateIdle;
            LevelRegistrySo.Instance.Unregister(this);
        }

        private void ChangeState(IPlayerState newState)
        {
            _currentState?.StateEnd();
            _currentState = newState;
            _currentState.StateBegin();
        }

        #region StateChanges

        private void ChangeStateMove()
        {
            ChangeState(_moveState);
        }
        
        private void ChangeStateIdle()
        {
            ChangeState(_idleState);
        }
        #endregion

        public void OnLevelRestart()
        {
           ChangeStateIdle();
        }

        public void OnLevelCheckPoint()
        {
            ChangeStateIdle();
        }

        public void OnLevelStart()
        {
            ChangeStateMove();
        }

        public void OnLevelStop() {/*EMPTY*/}
    }
}