using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Core
{
    /// <summary>
    /// Central Registry point for <see cref="ILevelEntity"/>. This script server for prevent to find <see cref="ILevelEntity"/>
    /// by using FindAnyObjectType which is CPU hungry operation. When game first begin, all scripts with <see cref="ILevelEntity"/>
    /// <see cref="Register"/> itself and this helps to only looking <see cref="_entities"/> will be enough to see all scripts are
    /// currently using <see cref="ILevelEntity"/>
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelRegistry")]
    public class LevelRegistrySo : ScriptableObject
    {
        private List<ILevelEntity> _entities = new();
        
        public void Register(ILevelEntity entity)
        {
            if (!_entities.Contains(entity))
                _entities.Add(entity);
        }

        public void Unregister(ILevelEntity entity)
        {
            if (_entities.Contains(entity))
                _entities.Remove(entity);
        }

        public void TriggerStart()
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].OnLevelStart();
        }

        public void TriggerStop()
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].OnLevelStop();
        }
        
        public void TriggerRestart()
        {
            for (int i = 0; i < _entities.Count; i++)
                _entities[i].OnLevelRestart();
        }
    }
}

