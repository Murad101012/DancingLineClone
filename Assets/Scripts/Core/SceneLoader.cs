using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gameplay;
using Interfaces;
using Ui.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// It's responsible for loading a Scene.
    /// </summary>
    /// <remarks>If a level working properly by playing directly, but throw null errors
    /// when loading that same level with LevelLoader, please check <see cref="IReady"/></remarks>
    public class SceneLoader : MonoBehaviour
    {
        //TODO: Replace Instance with Zenject
        public static SceneLoader Instance;
        private string _sceneNameInPreview;
        [SerializeField] private MenuOnLevelInPreviewChangeSo menuOnLevelInPreviewChange;
        public event Action LevelLoaded;
        public event Action LevelUnloaded;
        [SerializeField] private SceneFullyLoadedEventSo sceneFullyLoadedEvent;
        

        private void OnEnable()
        {
            MenuUiLevelCarousel.OnLoadLevelButtonClicked += LoadLevelAsync;
            menuOnLevelInPreviewChange.LevelPreviewChangeEvent += OnLevelPreviewChange;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            MenuUiLevelCarousel.OnLoadLevelButtonClicked -= LoadLevelAsync;
            menuOnLevelInPreviewChange.LevelPreviewChangeEvent -= OnLevelPreviewChange;
        }


        private async void LoadLevelAsync()
        {
            // Basic string check
            if (string.IsNullOrEmpty(_sceneNameInPreview)) 
            {
                Debug.LogWarning($"{name}: No scene name provided in preview");
                return;
            }

            // Build Settings check
            if (!Application.CanStreamedLevelBeLoaded(_sceneNameInPreview))
            {
                Debug.LogError($"{name}: Scene '{_sceneNameInPreview}' is not in Build Settings or doesn't exist");
                return;
            }
            
            if (_sceneNameInPreview == "") return;
            // 1. Start the Unity Async Operation
            AsyncOperation op = SceneManager.LoadSceneAsync(_sceneNameInPreview);

            // 2. Instead of 'yield return', we 'await'
            // We turn the Unity AsyncOperation into something awaitable
            while (!op.isDone)
            {
                // This replaces 'yield return null'
                await Task.Yield(); 
            }

            if (_sceneNameInPreview != "Menu")
            {
                LevelLoaded?.Invoke();
                AfterSceneCompletelyLoad();
            }
            else
            {
                LevelUnloaded?.Invoke();
            }
        }

        private void AfterSceneCompletelyLoad()
        {
            sceneFullyLoadedEvent.InvokeOnSceneFullyLoaded();
        }

        private void OnLevelPreviewChange()
        {
            _sceneNameInPreview = menuOnLevelInPreviewChange.levelInPreview.levelName;
        }

        public void ReturnToMenu()
        {
            _sceneNameInPreview = "Menu";
            LoadLevelAsync();
        }
    }
}