using Interfaces;
using UnityEngine;

namespace Player.States
{
    public class PlayerDeadState : IPlayerState
    {
        private PlayerCoreLogic _playerCoreLogic;

        public PlayerDeadState(PlayerCoreLogic playerCoreLogic) => _playerCoreLogic = playerCoreLogic;
        
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