using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// It's responsible for loading a Level.
    /// </summary>
    /// <remarks>If a level working properly by playing directly, but throw null errors
    /// when loading that same level with LevelLoader, please check <see cref="IReady"/></remarks>
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance;
        [SerializeField] private string sceneName;
        public Action LevelLoaded;
        public Action LevelUnloaded;
        private List<IReady> _iReadyList = new();
        
        [SerializeField] private LevelLoadEventSo levelLoadEventSo;

        private void OnEnable()
        {
            if (levelLoadEventSo != null)
            {
                levelLoadEventSo.OnLevelNameLoad += LoadLevelAsync;
            }
            else
            {
                string updateNameOf = nameof(LevelLoadEventSo);
                Debug.LogWarning($"{name}: {updateNameOf} isn't assigned, can't listen player's \"level load\" input." +
                                 $"Loading a level not possible.");
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDisable()
        {
            levelLoadEventSo.OnLevelNameLoad -= LoadLevelAsync;
        }


        private async void LoadLevelAsync(string sceneName)
        {
            // 1. Start the Unity Async Operation
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

            // 2. Instead of 'yield return', we 'await'
            // We turn the Unity AsyncOperation into something awaitable
            while (!op.isDone)
            {
                // This replaces 'yield return null'
                await Task.Yield(); 
            }
            
            LevelLoaded?.Invoke();
            
            Debug.Log("Load Complete!");
            AfterSceneLoad();
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
    }
}