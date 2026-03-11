using System;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Core
{
    //TODO: Separate logic where Registers stay here but Triggers must have their own class
    //TODO: Make this summary not exclusive to ILevelState description
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
        private List<IVictory> _victories = new();
        private List<IOnDead> _deads = new();
        
        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _levelStates.Clear();
            _onCheckPoints.Clear();
            _onRestarts.Clear();
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        //TODO: Add Safe check to be sure not a same script register/unregister itself more than once
        public void Register<T>(T entity)
        {
            if (entity is ILevelState state) _levelStates.Add(state);
            if (entity is IOnCheckPoint checkPoint) _onCheckPoints.Add(checkPoint);
            if (entity is IOnRestart restart) _onRestarts.Add(restart);
            if (entity is IVictory victory) _victories.Add(victory);
            if (entity is IOnDead dead) _deads.Add(dead);
        }

        public void Unregister<T>(T entity)
        {
            if (entity is ILevelState state) _levelStates.Remove(state);
            if (entity is IOnCheckPoint checkPoint) _onCheckPoints.Remove(checkPoint);
            if (entity is IOnRestart restart) _onRestarts.Remove(restart);
            if (entity is IVictory victory) _victories.Remove(victory);
            if (entity is IOnDead dead) _deads.Remove(dead);
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

        #region IVictory Methods

        public void TriggerOnVictory()
        {
            for (int i = 0; i < _victories.Count; i++)
            {
                _victories[i].OnVictory();
            }
        }
        #endregion
        
        #region IVictory Methods

        public void TriggerOnDead()
        {
            for (int i = 0; i < _deads.Count; i++)
            {
                _deads[i].OnDead();
            }
        }
        #endregion
        
    }
}

