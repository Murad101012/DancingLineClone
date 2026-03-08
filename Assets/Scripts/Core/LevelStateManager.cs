using Gameplay;
using Interfaces;
using Player;
using UnityEngine;

namespace Core
{
    public class LevelStateManager : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint
    {
        private LevelRegistrySo _levelRegistrySo;
        [SerializeField] private LevelPropertiesSo levelPropertiesSo;
        [SerializeField] private GameObject levelBeginButton; //:TODO Find a better location for this button
        
        private void OnEnable()
        {
            PlayerCoreLogic.Dead += StopTheGame;
            _levelRegistrySo = ScriptableObject.CreateInstance<LevelRegistrySo>();
            LevelRegistrySo.Instance.Register(this);
        }
        
        private void OnDisable()
        {
            PlayerCoreLogic.Dead -= StopTheGame;
            Destroy(_levelRegistrySo);
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
        #endregion
        
        public void OnLevelStart()
        {
            levelBeginButton.SetActive(false);
        }

        public void OnLevelStop() {/*It will be empty*/}
        
        public void OnLevelRestart()
        {
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
    }
}