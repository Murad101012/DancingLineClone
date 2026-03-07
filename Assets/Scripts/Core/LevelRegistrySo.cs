using System;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Core
{
    //TODO: Make this summary not exclusive to ILevelState descirption
    /// <summary>
    /// Central Registry point for Interfaces as <see cref="ILevelState"/> and etc. This script server for prevent to find <see cref="ILevelState"/>
    /// by using FindAnyObjectType which is CPU hungry operation. When game first begin, all scripts with <see cref="ILevelState"/>
    /// <see cref="RegisterILevelState"/> itself and this helps to only looking <see cref="_levelStates"/> will be enough to see all scripts are
    /// currently using <see cref="ILevelState"/>
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelRegistry")]
    public class LevelRegistrySo : ScriptableObject
    {
        //TODO: Change this to Dependency Injection instead
        public static LevelRegistrySo Instance { get; private set; }
        
        private List<ILevelState> _levelStates = new();
        private List<IOnCheckPoint> _onCheckPoints = new();
        private List<IOnRestart> _onRestarts = new();
        
        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _levelStates.Clear();
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        //TODO: This Register type written by AI, must explore how this Generic <T> works
        public void Register<T>(T entity)
        {
            if (entity is ILevelState state) _levelStates.Add(state);
            if (entity is IOnCheckPoint checkPoint) _onCheckPoints.Add(checkPoint);
            if (entity is IOnRestart restart) _onRestarts.Add(restart);
        }

        public void Unregister<T>(T entity)
        {
            if (entity is ILevelState state) _levelStates.Remove(state);
            if (entity is IOnCheckPoint checkPoint) _onCheckPoints.Remove(checkPoint);
            if (entity is IOnRestart restart) _onRestarts.Remove(restart);
        }

        #region ILevelState Methods
        public void TriggerStartILevelState()
        {
            for (int i = 0; i < _levelStates.Count; i++)
                _levelStates[i].OnLevelStart();
        }

        public void TriggerStopILevelState()
        {
            for (int i = 0; i < _levelStates.Count; i++)
                _levelStates[i].OnLevelStop();
        }
        #endregion

        #region IOnCheckPoint Methods
        public void TriggerOnCheckPoint()
        {
            for (int i = 0; i < _onCheckPoints.Count; i++)
            {
                _onCheckPoints[i].OnLevelCheckPoint();
            }
        }
        #endregion

        #region IOnRestart Methods
        public void TriggerOnRestart()
        {
            for (int i = 0; i < _onRestarts.Count; i++)
            {
                _onRestarts[i].OnLevelRestart();
            }
        }
        #endregion
    }
}

