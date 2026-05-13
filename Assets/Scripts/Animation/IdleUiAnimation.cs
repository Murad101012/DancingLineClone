using System;
using Core;
using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Animation
{
    /// <summary>
    /// Animation sequences collection for Idle screen (Screen appear that waiting for player to click the screen and begin to play)
    /// </summary>
    public class IdleUiAnimation : MonoBehaviour, ILevelRegistryUser, IOnRestart, IOnCheckPoint, ILevelState
    {
        private LevelRegistrySo _levelRegistrySo;
        [SerializeField] private CanvasGroup canvasGroup;
        private Sequence _canvasGroupOpacitySequence;
        public event Action OnCanvasAnimationEnd;

        private void Awake()
        {
            _levelRegistrySo.Register(this);
            
            _canvasGroupOpacitySequence = DOTween.Sequence();
            
            _canvasGroupOpacitySequence.Append(canvasGroup.DOFade(0, 0.5f).From(1f).
                OnComplete(() => OnCanvasAnimationEnd?.Invoke()));
            _canvasGroupOpacitySequence.SetAutoKill(false);
            _canvasGroupOpacitySequence.Pause();
        }

        private void OnDestroy()
        {
            _levelRegistrySo.Unregister(this);
        }

        public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
        {
            _levelRegistrySo = levelRegistrySo;
        }

        public void OnLevelRestart()
        {
            _canvasGroupOpacitySequence.PlayBackwards();
        }
        
        public void OnLevelCheckPoint()
        {
            _canvasGroupOpacitySequence.PlayBackwards();
        }

        public void OnLevelStart()
        {
            _canvasGroupOpacitySequence.PlayForward();
        }

        public void OnLevelStop(){/*IT WILL BE EMPTY*/}
    }
}