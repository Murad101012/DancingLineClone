using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gameplay;
using Interfaces;
using Ui.Menu;
using Ui.SceneTransformation;
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
        private SceneTransformationUiController _sceneTransformationUiController;
        private string _sceneNameInPreview;
        [SerializeField] private MenuOnLevelInPreviewChangeSo menuOnLevelInPreviewChange;

        [SerializeField] private SceneFullyLoadedEventSo sceneFullyLoadedEvent;
        

        private void OnEnable()
        {
            MenuUiLevelCarousel.OnLoadLevelButtonClicked += LoadLevelAsync;
            menuOnLevelInPreviewChange.LevelPreviewChangeEvent += OnLevelPreviewChange;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            TryGetComponent(out _sceneTransformationUiController);
        }

        private void Start()
        {
            ReturnToMenu();
        }

        private void OnDisable()
        {
            MenuUiLevelCarousel.OnLoadLevelButtonClicked -= LoadLevelAsync;
            menuOnLevelInPreviewChange.LevelPreviewChangeEvent -= OnLevelPreviewChange;
        }


        private async void LoadLevelAsync()
        {
            if (_sceneTransformationUiController == null)
            {
                Debug.LogWarning($"{name}: variable '{_sceneTransformationUiController}' is null. " +
                                 "Skipping blacking out the screen animation for Scene Load");
            }
            else
            {
                _sceneTransformationUiController.LoadScreenAnimation();
            }
            
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

            SceneBeginToLoadEventInvoke();
            AsyncOperation op = SceneManager.LoadSceneAsync(_sceneNameInPreview);
            op.allowSceneActivation = false;
            
            //We're waiting until loadScreenAnimationFullyLoaded = true
            while (!_sceneTransformationUiController.loadScreenAnimationFullyLoaded)
            {
                await Task.Yield(); 
            }

            //When player only see loading screen, we're then allowing scene to load
            op.allowSceneActivation = true;

            //We check if level fully load
            while (!op.isDone)
            {
                await Task.Yield();
            }
            
            //If scene also fully initialized/load/ready, we're removing the loading screen with animation
            if (_sceneTransformationUiController != null) _sceneTransformationUiController.LoadScreenAnimation(false);

            AfterSceneCompletelyLoadEventInvoke();
        }

        private void SceneBeginToLoadEventInvoke()
        {
            sceneFullyLoadedEvent.InvokeOnSceneBeginToLoad();
        }

        private void AfterSceneCompletelyLoadEventInvoke()
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