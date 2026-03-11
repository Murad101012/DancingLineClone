using System;
using Animation;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class VictoryUiController : MonoBehaviour, IVictory, IOnRestart
    {
        [Header("UI References")]
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private Button restartButton;
        private VictoryUiAnimation _victoryUiAnimation;
        
        private void OnEnable()
        {
            LevelRegistrySo.Instance.Register(this);
            
            //It's require for smooth VictoryScreen disabling, without preventing Scaling down animation
            TryGetComponent(out _victoryUiAnimation);
            if(_victoryUiAnimation != null) _victoryUiAnimation.OnVictoryAnimationBackwardEnd += Reset;
        }

        private void OnDisable()
        {
            LevelRegistrySo.Instance.Unregister(this);
            if(_victoryUiAnimation != null) _victoryUiAnimation.OnVictoryAnimationBackwardEnd -= Reset;
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
    }
}