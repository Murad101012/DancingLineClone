using System;
using Core;
using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Animation
{
    /// <summary>
    /// Animations for Victory.prefab
    /// </summary>
    [RequireComponent(typeof(VictoryUiController))]
    public class VictoryUiAnimation : MonoBehaviour, IVictory, IOnRestart
    {
        [SerializeField] private GameObject victoryRoot;
        private Sequence _sequenceVictoryRoot;
        public event Action OnVictoryAnimationBackwardEnd;

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
            
            /*Scale victoryRoot to Vector3.zero for ease animation where from 0 to 1.
             Since using .From() tween causing show victoryUI scale 1 (Meaning it's being late
             to set up), LocalScale used instead*/
            victoryRoot.transform.localScale = Vector3.zero;
            
            /*As soon as player trigger the VictoryTrigger.prefab, victory screen shown with this animation
             It also uses when hiding victory screen by playing reverse*/
            _sequenceVictoryRoot = DOTween.Sequence();
            _sequenceVictoryRoot.Append(victoryRoot.transform.
                DOScale(Vector3.one, 0.5f).
                SetEase(Ease.OutBack, 2.3f));
            _sequenceVictoryRoot.Pause();
            _sequenceVictoryRoot.SetAutoKill(false);
            
            _sequenceVictoryRoot.OnRewind(() => OnVictoryAnimationBackwardEnd?.Invoke());
        }
        
        private void VictoryScreenAnimate(bool playForward = true)
        {
            if (playForward) _sequenceVictoryRoot.PlayForward();
            else _sequenceVictoryRoot.PlayBackwards();
        }

        public void OnVictory()
        {
            VictoryScreenAnimate();
        }
        
        public void OnLevelRestart()
        {
            VictoryScreenAnimate(false);
        }
    }
}