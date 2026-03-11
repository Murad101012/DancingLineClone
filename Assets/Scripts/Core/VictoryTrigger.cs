using System;
using Interfaces;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// It's for triggering the Victory that put on at the of level.
    /// Use VictoryTrigger.prefab
    /// </summary>
    public class VictoryTrigger: MonoBehaviour
    {
        public static event Action OnVictoryTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IVictory _)) OnVictoryTriggered?.Invoke();
        }
    }
}