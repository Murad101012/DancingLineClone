using System;
using Core;
using DG.Tweening;
using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    /// <summary>
    /// Animations for Defeat.prefab
    /// </summary>
    [RequireComponent(typeof(DefeatUiController))]
    public class DefeatUiAnimation : MonoBehaviour, IOnRestart, IOnCheckPoint, IOnDead
    {
        [SerializeField] private GameObject defeatRoot;
        [SerializeField] private Button button;
        
        //Sequences
        private Sequence _sequenceDefeatRoot;
        
        public event Action OnDefeatAnimationBackwardEnd;
        
        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
            
            /*Scale defeatRoot to Vector3.zero for ease animation where from 0 to 1.
             Since using .From() tween causing show defeatUI scale 1 (Meaning it's being late
             to set up), LocalScale used instead*/
            defeatRoot.transform.localScale = Vector3.zero;
            
            /*As soon as player dead, defeat screen shown with this animation
             It also uses when hiding defeat screen by playing reverse*/
            _sequenceDefeatRoot = DOTween.Sequence();
            _sequenceDefeatRoot.Append(defeatRoot.transform.
                DOScale(Vector3.one, 0.5f).
                SetEase(Ease.OutBack, 2.3f));
            _sequenceDefeatRoot.Pause();
            _sequenceDefeatRoot.SetAutoKill(false);
            
            _sequenceDefeatRoot.OnRewind(() => OnDefeatAnimationBackwardEnd?.Invoke());
        }

        public void OnDead()
        {
            DefeatScreenAnimate();
        }
        
        //It can be uses for both "Showing" and "Hiding" the defeat screen
        private void DefeatScreenAnimate(bool playForward = true)
        {
            if (playForward) _sequenceDefeatRoot.PlayForward();
            else _sequenceDefeatRoot.PlayBackwards();
        }

        public void OnLevelRestart()
        {
            DefeatScreenAnimate(false);
        }

        public void OnLevelCheckPoint()
        {
            DefeatScreenAnimate(false);
        }
    }
}