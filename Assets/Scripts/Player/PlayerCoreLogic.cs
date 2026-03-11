using System;
using Core;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// It's central point for ObjectSo, tracking Dead and
    /// extending capabilities to <see cref="Interfaces.IVictory"/>
    /// </summary>
    [RequireComponent(typeof(StateMachine))]
    public class PlayerCoreLogic : MonoBehaviour, IVictory
    {
        [field: SerializeField] public ObjectStatsSo ObjectStatsSo { get; private set; }
        
        public static event Action Dead;

        private void OnEnable()
        {
            GroundStateChecker.OnNonGroundChange += OnNonGroundStateChangeUpdater;
        }

        private void Awake()
        {
            if (ObjectStatsSo == null)
            {
                Debug.LogWarning(
                    $"ObjectStatsSo is not assigned, using dummy ObjectStatsSo with default values for {name}");
                ObjectStatsSo = ScriptableObject.CreateInstance<ObjectStatsSo>();
                ObjectStatsSo.speed = 3;
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
                Dead?.Invoke();
            }
        }

        public void OnVictory(){/*It will be empty*/}
    }
}