using System;
using Core;
using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// All sounds in the game going through this script
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudioPlayer : MonoBehaviour, ILevelState, IOnDead
    {
        public static event Action<AudioClip> OnLevelClip;
        private AudioSource _audioSource;
        [SerializeField] public AudioClip _clip;
        
        //TODO: After make a class for LevelLoading, move these events to there.
        ///<remarks>These events temporary settled in here, it will move to another class when it's created</remarks>
        public static Action LevelLoaded;
        public static Action LevelUnloaded;

        private void OnEnable()
        {
            LevelLoaded += RegisterToLevel;
            LevelUnloaded += UnregisterFromLevel;
            OnLevelClip += InsertClip;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            
            _audioSource = GetComponent<AudioSource>();
            
            //This is temporary. After Move LeveLoaded and LevelUnloaded events, remove it:
            LevelLoaded?.Invoke();
            OnLevelClip?.Invoke(_clip);
        }

        private void OnDisable()
        {
            LevelLoaded -= RegisterToLevel;
            LevelUnloaded -= UnregisterFromLevel;
            OnLevelClip -= InsertClip;
        }

        private void RegisterToLevel()
        {
            if(LevelRegistrySo.Instance) LevelRegistrySo.Instance.Register(this);
        }

        private void UnregisterFromLevel()
        {
            if(LevelRegistrySo.Instance) LevelRegistrySo.Instance.Unregister(this);
        }

        private void InsertClip(AudioClip clip)
        {
            _clip = clip;
            _audioSource.clip = clip;
        }

        private void PlaySound()
        {
            _audioSource.DOKill();
            
            _audioSource.volume = 1;
            if(_audioSource.clip != null) _audioSource.Play();
            else{Debug.LogWarning("GlobalAudioPlayer: No clip found to play");}
        }

        private void StopSound(bool fading = true)
        {
            _audioSource.DOKill();
            
            if (!fading) _audioSource.Stop();
            else _audioSource.DOFade(0, 5.0f).onComplete +=
                () => _audioSource.Stop();
        }

        public void OnLevelStart()
        {
            PlaySound();
        }

        public void OnLevelStop()
        {
            StopSound();
        }

        public void OnDead()
        {
            StopSound();
        }
    }
}