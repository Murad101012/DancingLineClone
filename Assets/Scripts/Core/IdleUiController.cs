using System;
using Animation;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class IdleUiController : MonoBehaviour, IOnRestart, ILevelRegistryUser, ILevelState
    {
        private LevelRegistrySo _levelRegistrySo;
        [SerializeField] private GameObject idleCanvas;
        [SerializeField] private Button backToMenuButton;
        private IdleUiAnimation _idleUiAnimation;
        private bool _onIdle = true;

        private void Awake()
        {
            _levelRegistrySo.Register(this);
            TryGetComponent(out _idleUiAnimation);
        }

        private void OnEnable()
        {
            if (_idleUiAnimation != null)
            {
                _idleUiAnimation.OnCanvasAnimationEnd += DisableIdleCanvas;
            }
        }

        private void OnDisable()
        {
            _idleUiAnimation.OnCanvasAnimationEnd -= DisableIdleCanvas;
        }

        private void OnDestroy()
        {
            _levelRegistrySo.Unregister(this);
        }
        
        private void DisableIdleCanvas()
        {
            if (!_onIdle)
            {
                idleCanvas.SetActive(false);
            }
        }

        public void OnLevelRestart()
        {
            backToMenuButton.interactable = true;
            _onIdle = true;
            idleCanvas.SetActive(true);
        }

        public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
        {
            _levelRegistrySo = levelRegistrySo;
        }

        public void OnLevelStart()
        {
            backToMenuButton.interactable = false;
            _onIdle = false;
            if (_idleUiAnimation == null)
            {
                Debug.LogWarning($"{name}: {nameof(_idleUiAnimation)} is null. " +
                                 $"Disabling the IdleUiCanvas without waiting idle animation complete when {nameof(OnLevelStart)}");
                idleCanvas.SetActive(false);
            }
        }

        public void OnLevelStop()
        {
            /*It will be empty*/
        }
    }
}