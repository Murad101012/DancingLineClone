using System;
using Core;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// It's central point for ObjectSo and tracking Dead
    /// </summary>
    [RequireComponent(typeof(StateMachine))]
    public class PlayerCoreLogic : MonoBehaviour
    {
        [field: SerializeField] public ObjectStatsSo ObjectStatsSo { get; private set; }

        //TODO Change Dead to a different script logic
        public static event Action Dead;

        private void OnEnable()
        {
            GroundStateChecker.OnNonGroundChange += OnNonGroundStateChangeUpdater;
        }

        private void Awake()
        {
            if (ObjectStatsSo == null)
            {
                Debug.LogWarning($"ObjectStatsSo is not assigned, using dummy ObjectStatsSo with default values for {name}");
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
            //TODO: Move Dead section to own separate function in future
            if (currentState)
            {
                Dead?.Invoke();
            }
        }
    }
}