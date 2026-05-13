using System;
using Core;
using Core.Data;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// It's central point for ObjectSo, tracking Dead and
    /// extending capabilities to <see cref="Interfaces.IVictory"/>
    /// </summary>
    [RequireComponent(typeof(StateMachine))]
    public class PlayerCoreLogic : MonoBehaviour
    {
        [field: SerializeField] public PlayerStatsSo PlayerStatsSo { get; private set; }
        [SerializeField] private LevelEventHubSo levelEventHubSo;
        
        private void OnEnable()
        {
            GroundStateChecker.OnNonGroundChange += OnNonGroundStateChangeUpdater;
        }

        private void Awake()
        {
            if (PlayerStatsSo == null)
            {
                Debug.LogWarning(
                    $"ObjectStatsSo is not assigned, using dummy ObjectStatsSo with default values for {name}");
                PlayerStatsSo = ScriptableObject.CreateInstance<PlayerStatsSo>();
                PlayerStatsSo.speed = 10;
            }
        }


        private void OnDisable()
        {
            GroundStateChecker.OnNonGroundChange -= OnNonGroundStateChangeUpdater;
        }

        private void OnNonGroundStateChangeUpdater(bool currentState)
        {
            if (currentState)
            {
                if (levelEventHubSo == null)
                {
                    Debug.LogWarning($"{nameof(levelEventHubSo)} isn't assigned, can't invoke dead");
                    return;
                }
                
                levelEventHubSo.PublishPlayerDead();
            }
        }
    }
}