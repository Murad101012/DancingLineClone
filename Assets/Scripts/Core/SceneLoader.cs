using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gameplay;
using Interfaces;
using Ui.Core;
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
    public class SceneLoader : MonoBehaviour, ILevelPreviewChange
    {
        public static SceneLoader Instance;
        private string _sceneNameInPreview;
        public event Action LevelLoaded;
        public event Action LevelUnloaded;
        private List<IReady> _iReadyList = new();
        

        private void OnEnable()
        {
            LevelPreviewChangeSender.OnLevelPreviewChange += OnLevelPreviewChange;
            MenuUiController.OnLoadLevelButtonClicked += LoadLevelAsync;
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
            LevelPreviewChangeSender.OnLevelPreviewChange -= OnLevelPreviewChange;
            MenuUiController.OnLoadLevelButtonClicked -= LoadLevelAsync;
        }


        private async void LoadLevelAsync()
        {
            // Basic string check
            if (string.IsNullOrEmpty(_sceneNameInPreview)) 
            {
                Debug.LogWarning($"{name}: No scene name provided in preview!");
                return;
            }

            // Build Settings check
            if (!Application.CanStreamedLevelBeLoaded(_sceneNameInPreview))
            {
                Debug.LogError($"{name}: Scene '{_sceneNameInPreview}' is not in Build Settings or doesn't exist!");
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
                AfterSceneLoad();
            }
            else
            {
                LevelUnloaded?.Invoke();
            }
        }

        public void RegisterIReady(IReady iReady)
        {
            _iReadyList.Add(iReady);
        }

        private void AfterSceneLoad()
        {
            /*It will call all the Initialization scripts those are
            using IReady to waiting scene fully loaded*/
            for (int i = 0; i < _iReadyList.Count; i++)
            {
                _iReadyList[i].Initialization();
            }
            
            //After call all Initialization scripts, list removed
            _iReadyList.Clear();
        }

        public void OnLevelPreviewChange(LevelPropertiesSo levelPropertiesSo)
        {
            _sceneNameInPreview = levelPropertiesSo.levelName;
        }

        public void ReturnToMenu()
        {
            _sceneNameInPreview = "Menu";
            LoadLevelAsync();
        }
    }
}