using System;
using Gameplay;
using Interfaces;
using Player;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// It helps to change States of Level with Interfaces
    /// </summary>
    public class LevelStateManager : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint, IVictory, ILevelRegistryUser
    {
        [SerializeField] private LevelRegistrySo levelRegistrySo;
        [SerializeField] private LevelPropertiesSo levelPropertiesSo;
        [SerializeField] private GameObject levelBeginButton; //:TODO Find a better location for this 
        private bool _isVictory;
        private LevelRegistrySo _levelRegistrySo;
        
        private void OnEnable()
        {
            PlayerCoreLogic.Dead += PlayerDead;
            VictoryTrigger.OnVictoryTriggered += SetTheVictory;
            VictoryLogic.OnRestartButtonPressed += RestartTheLevel;
        }

        private void Awake()
        {
            _levelRegistrySo.Register(this);
        }

        private void OnDisable()
        {
            PlayerCoreLogic.Dead -= PlayerDead;
            VictoryTrigger.OnVictoryTriggered -= SetTheVictory;
            VictoryLogic.OnRestartButtonPressed -= RestartTheLevel;
        }

        private void OnDestroy()
        {
            _levelRegistrySo.Unregister(this);
        }

        #region Triggers Interfaces
        public void StartTheGame()
        {
            _levelRegistrySo.TriggerStartILevelState();
        }
        
        private void StopTheGame()
        {
            _levelRegistrySo.TriggerStopILevelState();
        }

        public void RestartTheLevel()
        {
            _levelRegistrySo.TriggerOnRestart();
        }

        public void CheckPointTheLevel()
        {
            _levelRegistrySo.TriggerOnCheckPoint();
        }
        
        private void SetTheVictory()
        {
            _levelRegistrySo.TriggerOnVictory();
        }

        private void PlayerDead()
        {
            if (_isVictory) return;
            _levelRegistrySo.TriggerOnDead();
        }
        
        #endregion
        
        public void OnLevelStart()
        {
            levelBeginButton.SetActive(false);
        }

        public void OnLevelStop() {/*It will be empty*/}
        
        public void OnLevelRestart()
        {
            _isVictory = false;
            Reset();
        }

        public void OnLevelCheckPoint()
        {
            Reset();
        }
        
        /// <summary>
        /// Functions are usually require same modify for both <see cref="IOnRestart"/> and <see cref="IOnCheckPoint"/>>
        /// </summary>
        private void Reset()
        {
            levelBeginButton.SetActive(true);
        }

        public void OnVictory()
        {
            _isVictory = true;
        }
        
        public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
        {
            _levelRegistrySo = levelRegistrySo;
        }
    }
}