using System;
using Core;
using DG.Tweening;
using Gameplay;
using Interfaces;
using Ui.Core;
using Ui.Menu;
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
        private AudioClip _clip;
        private SceneLoader _sceneLoader;
        [SerializeField] private MenuOnLevelInPreviewChangeSo menuOnLevelInPreviewChangeSo;

        private void OnEnable()
        {
            if (menuOnLevelInPreviewChangeSo == null)
            {
                Debug.LogWarning($"{name}: {nameof(menuOnLevelInPreviewChangeSo)} is null. Can't access audio when level preview change");
                return;
            }
            menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent += OnLevelPreviewChange;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (TryGetComponent(out _sceneLoader))
            {
                _sceneLoader.LevelLoaded += OnSceneLoad;
                _sceneLoader.LevelUnloaded += OnSceneUnload;
            }
            else
            {
                Debug.LogWarning("GlobalAudioPlayer: LevelLoader not found, can't register");
            }
            
            InsertClip(_clip);
        }

        private void OnDisable()
        {
            menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent -= OnLevelPreviewChange;
        }

        private void OnDestroy()
        {
            if (_sceneLoader != null)
            {
                _sceneLoader.LevelLoaded -= OnSceneLoad;
                _sceneLoader.LevelUnloaded -= OnSceneUnload;
            }
        }

        private void OnSceneLoad()
        {
            LevelRegistrySo.Instance?.Register(this);
            StopSound();
        }

        private void OnSceneUnload()
        {
            LevelRegistrySo.Instance?.Unregister(this);
        }

        private void InsertClip(AudioClip clip)
        {
            _clip = clip;
            _audioSource.clip = clip;
        }

        private void PlaySound(bool delay = false)
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
            else _audioSource.DOFade(0, 5.0f).OnComplete(_audioSource.Stop);
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

        private void OnLevelPreviewChange()
        {
            if (menuOnLevelInPreviewChangeSo.levelInPreview.levelSound == null) return;
            InsertClip(menuOnLevelInPreviewChangeSo.levelInPreview.levelSound);
            PlaySound(true);
        }
    }
}