using System;
using Core;
using Interfaces;
using UnityEngine;

namespace Gameplay
{
    /// <remarks>
    /// For use this class, you must use CheckPoint.prefab and add prefab on level where checkpoint must be happen
    /// </remarks>
    public class CheckpointTrigger : MonoBehaviour, IOnRestart
    {
        private bool _triggered;
        
        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        //Checkpoint check happen when player trigger CheckPoint.prefab
        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;

            if (other.TryGetComponent(out ICheckPointReceiver checkpoint))
            {
                _triggered = true;
                checkpoint.CheckPointReceive(transform);
            }
        }

        public void OnLevelRestart()
        {
            _triggered = false;
        }
    }
}