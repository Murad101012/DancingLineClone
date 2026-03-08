using Interfaces;
using UnityEngine;

namespace Player.States
{
    public class PlayerIdleState : IPlayerState
    {
        private PlayerCoreLogic _playerCoreLogic;

        public PlayerIdleState(PlayerCoreLogic playerCoreLogic) => _playerCoreLogic = playerCoreLogic;
        
        public void StateBegin()
        {
        }

        public void StateTick()
        {
        }

        public void StateEnd()
        {
        }
    }
}