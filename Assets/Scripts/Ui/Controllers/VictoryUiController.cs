using Interfaces;
using Ui.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Controllers
{
    /// <summary>
    /// Controls the lifetime of gameobjects of Victory Ui prefab
    /// </summary>
    public class VictoryUiController : MonoBehaviour, IVictory, IOnRestart, ILevelRegistryUser
    {
        [Header("UI References")]
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private Button restartButton;
        private VictoryUiAnimation _victoryUiAnimation;
        private ILevelRegistry _levelRegistry;
        
        private void OnEnable()
        {
            _levelRegistry.Register(this);
            
            TryGetComponent(out _victoryUiAnimation);
            if(_victoryUiAnimation != null) _victoryUiAnimation.RestartEndAnimationEnd += Reset;
        }

        private void OnDisable()
        {
            _levelRegistry.Unregister(this);
            if(_victoryUiAnimation != null) _victoryUiAnimation.RestartEndAnimationEnd -= Reset;
        }

        public void OnVictory()
        {
            victoryScreen.SetActive(true);
        }

        public void OnLevelRestart()
        {
            NullCheckDefeatUiAnimationRewindEvent();
        }
        public void Reset()
        {
            victoryScreen.SetActive(false);
        }
        
        private void NullCheckDefeatUiAnimationRewindEvent()
        {
            if (_victoryUiAnimation == null)
            {
                Reset();
                Debug.LogWarning($"{name}: VictoryUiController: _victoryUiAnimation is null, " +
                                 $"bypassing animation");
            }
        }
        
        public void OnLevelRestartButton()
        {
            _victoryUiAnimation.PlayRestartBeginAnimation();
        }
        
        public void LevelRegistrySoSetter(ILevelRegistry levelRegistry)
        {
            _levelRegistry = levelRegistry;
        }
    }
}