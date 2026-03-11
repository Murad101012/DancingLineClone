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
    public class LevelStateManager : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint, IVictory
    {
        [SerializeField] private LevelRegistrySo levelRegistrySo;
        [SerializeField] private LevelPropertiesSo levelPropertiesSo;
        [SerializeField] private GameObject levelBeginButton; //:TODO Find a better location for this 
        private bool _isVictory;
        
        private void OnEnable()
        {
            PlayerCoreLogic.Dead += PlayerDead;
            VictoryTrigger.OnVictoryTriggered += SetTheVictory;
            VictoryLogic.OnRestartButtonPressed += RestartTheLevel;
        }

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
        }

        private void OnDisable()
        {
            PlayerCoreLogic.Dead -= PlayerDead;
            VictoryTrigger.OnVictoryTriggered -= SetTheVictory;
            VictoryLogic.OnRestartButtonPressed -= RestartTheLevel;
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        #region Triggers Interfaces
        public void StartTheGame()
        {
            LevelRegistrySo.Instance.TriggerStartILevelState();
        }
        
        private void StopTheGame()
        {
            LevelRegistrySo.Instance.TriggerStopILevelState();
        }

        public void RestartTheLevel()
        {
            LevelRegistrySo.Instance.TriggerOnRestart();
        }

        public void CheckPointTheLevel()
        {
            LevelRegistrySo.Instance.TriggerOnCheckPoint();
        }
        
        private void SetTheVictory()
        {
            LevelRegistrySo.Instance.TriggerOnVictory();
        }

        private void PlayerDead()
        {
            if (_isVictory) return;
            LevelRegistrySo.Instance.TriggerOnDead();
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
    }
}