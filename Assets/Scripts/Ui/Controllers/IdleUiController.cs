using Interfaces;
using Ui.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Controllers
{
    /// <summary>
    /// Control the lifecycle of Idle Ui's element
    /// </summary>
    public class IdleUiController : MonoBehaviour, IOnRestart, IOnCheckPoint, ILevelRegistryUser, ILevelState
    {
        private ILevelRegistry _levelRegistry;
        [SerializeField] private GameObject idleCanvas;
        [SerializeField] private Button backToMenuButton;
        private IdleUiAnimation _idleUiAnimation;
        private bool _onIdle = true;

        private void Awake()
        {
            _levelRegistry.Register(this);
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
            _levelRegistry.Unregister(this);
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
            ActivateIdleCanvas();
        }
        
        public void OnLevelCheckPoint()
        {
            ActivateIdleCanvas();
        }

        private void ActivateIdleCanvas()
        {
            backToMenuButton.interactable = true;
            _onIdle = true;
            idleCanvas.SetActive(true);
        }

        public void LevelRegistrySoSetter(ILevelRegistry levelRegistry)
        {
            _levelRegistry = levelRegistry;
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

        public void OnLevelStop(){/*It will be empty*/ }
    }
}