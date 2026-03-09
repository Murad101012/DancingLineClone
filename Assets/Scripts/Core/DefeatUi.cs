using System;
using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DefeatUi : MonoBehaviour, ILevelState, IOnRestart, IOnCheckPoint
    {
        [Header("UI References")]
        [SerializeField] private GameObject defeatScreen;
        [SerializeField] private Button checkPointButton;
        /// <remarks>
        /// Duplicates from <see cref="CheckPointManager._checkPointTriggered"/>
        /// </remarks>
        private bool _checkPointTriggered;
        
        private void OnEnable()
        {
            PlayerCoreLogic.Dead += Defeated;
            CheckPointManager.OnCheckpointUpdated += RefreshCheckPointButtonState;
        }

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
        }

        private void OnDisable()
        {
            PlayerCoreLogic.Dead -= Defeated;
            CheckPointManager.OnCheckpointUpdated -= RefreshCheckPointButtonState;
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        private void RefreshCheckPointButtonState() 
        {
            if (checkPointButton != null)
                checkPointButton.interactable = true;
            else Debug.LogWarning($"{name}: No checkpoint button found");
        }

        private void Defeated()
        {
            defeatScreen.SetActive(true);
        }

        public void Reset()
        {
            defeatScreen.SetActive(false);
        }

        public void OnLevelRestart()
        {
            _checkPointTriggered = false;
            checkPointButton.interactable = _checkPointTriggered;
            Reset();
        }

        public void OnLevelCheckPoint()
        {
            Reset();
        }
        
        #region Interfaces will be empty
        public void OnLevelStart()
        {
            //It will be empty
        }

        public void OnLevelStop()
        {
            //It will be empty
        }
        #endregion
    }
}