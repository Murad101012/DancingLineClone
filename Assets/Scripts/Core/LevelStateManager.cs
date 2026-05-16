using DataContainer;
using Interfaces;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// It helps to change States of Level with Interfaces using <see cref="LevelRegistrySo"/>
    /// </summary>
    public class LevelStateManager : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint, IVictory
    {
        /// <summary>
        /// LevelRegistrySo must manually to this script
        /// </summary>
        [SerializeField] private LevelRegistrySo levelRegistrySo;
        [SerializeField] private LevelPropertiesSo levelPropertiesSo;
        [SerializeField] private GameObject levelBeginButton; //:TODO Find a better location for this 
        [SerializeField] private LevelEventHubSo levelEventHubSo;
        private bool _isVictory;
        
        private void OnEnable()
        {
            levelEventHubSo.OnPlayerDead += PlayerOnPlayerDead;
            levelEventHubSo.OnVictoryTriggered += SetTheVictory;
            levelEventHubSo.OnCheckpointBeginAnimationEnd += CheckpointTheLevel;
            levelEventHubSo.OnRestartBeginAnimationEnd += RestartTheLevel;
        }

        private void Awake()
        {
            levelRegistrySo.Register(this);
        }

        private void OnDisable()
        {
            levelEventHubSo.OnPlayerDead -= PlayerOnPlayerDead;
            levelEventHubSo.OnVictoryTriggered -= SetTheVictory;
            levelEventHubSo.OnCheckpointBeginAnimationEnd -= CheckpointTheLevel;
            levelEventHubSo.OnRestartBeginAnimationEnd -= RestartTheLevel;
        }

        private void OnDestroy()
        {
            levelRegistrySo.Unregister(this);
        }

        #region Triggers Interfaces
        public void StartTheGame()
        {
            levelRegistrySo.TriggerStartILevelState();
        }
        
        private void StopTheGame()
        {
            levelRegistrySo.TriggerStopILevelState();
        }

        private void CheckpointTheLevel()
        { 
            levelRegistrySo.TriggerOnCheckPoint();
        }

        private void RestartTheLevel()
        {
            levelRegistrySo.TriggerOnRestart();
        }

        public void CheckPointTheLevel()
        {
            levelRegistrySo.TriggerOnCheckPoint();
        }
        
        private void SetTheVictory()
        {
            levelRegistrySo.TriggerOnVictory();
        }

        private void PlayerOnPlayerDead()
        {
            if (_isVictory) return;
            levelRegistrySo.TriggerOnDead();
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