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
        private AudioSource _audioSource;
        [SerializeField] public AudioClip _clip; //Remove SerializeField after fully implementation
        private LevelLoader _levelLoader;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (TryGetComponent(out _levelLoader))
            {
                _levelLoader.LevelLoaded += RegisterToLevel;
                _levelLoader.LevelUnloaded += UnregisterFromLevel;
            }
            else
            {
                Debug.LogWarning("GlobalAudioPlayer: LevelLoader not found, can't register");
            }
            
            InsertClip(_clip);
        }
        

        private void OnDestroy()
        {
            if (_levelLoader != null)
            {
                _levelLoader.LevelLoaded -= RegisterToLevel;
                _levelLoader.LevelUnloaded -= UnregisterFromLevel;
            }
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