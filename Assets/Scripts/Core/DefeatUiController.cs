using Animation;
using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Responsible for managing life cycle of Defeat.prefab's gameobjects
    /// </summary>
    public class DefeatUiController : MonoBehaviour, IOnRestart, IOnCheckPoint, IOnDead
    {
        [Header("UI References")]
        [SerializeField] private GameObject defeatScreen;
        [SerializeField] private Button checkPointButton;
        /// <remarks>
        /// Duplicates from <see cref="CheckPointManager._checkPointTriggered"/>
        /// </remarks>
        private bool _checkPointTriggered;
        private DefeatUiAnimation _defeatUiAnimation;
        
        private void OnEnable()
        {
            CheckPointManager.OnCheckpointUpdated += RefreshCheckPointButtonState;

            //It's require for smooth DefeatScreen disabling, without preventing Scaling down animation
            TryGetComponent(out _defeatUiAnimation);
            if(_defeatUiAnimation != null) _defeatUiAnimation.OnDefeatAnimationBackwardEnd += Reset;
        }

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
        }
        
        private void OnDisable()
        {
            CheckPointManager.OnCheckpointUpdated -= RefreshCheckPointButtonState;
            if(_defeatUiAnimation != null) _defeatUiAnimation.OnDefeatAnimationBackwardEnd -= Reset;
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

        public void OnDead()
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
            NullCheckDefeatUiAnimationRewindEvent();
        }

        public void OnLevelCheckPoint()
        {
            NullCheckDefeatUiAnimationRewindEvent();
        }

        private void NullCheckDefeatUiAnimationRewindEvent()
        {
            if (_defeatUiAnimation == null)
            {
                Reset();
                Debug.LogWarning($"{name}: DefeatUiController: _defeatUiAnimation is null, " +
                                 $"bypassing animation");
            }
        }
    }
}