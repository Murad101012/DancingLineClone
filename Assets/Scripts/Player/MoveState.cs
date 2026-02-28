using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Current move directions update in <see cref="currentStates"/>
    /// </summary>
    /// <remarks>
    /// For get the latest directions, <see cref="Movement"/> script must take current directions from <see cref="currentStates"/>
    /// </remarks>
    public class MoveState : MonoBehaviour
    {
        public static MoveState Instance;
        public enum States
        {
            Forward,
            Backward,
            Left,
            Right,
        }
        
        public States[] currentStates = new States[2];
        public Dictionary<States, Quaternion> StateDirectionDictionary = new(4);
        
        public static event Action<States[]> OnStateChange;
        public static void TriggerStateChange(States[] newStates)
        {
            OnStateChange?.Invoke(newStates);
        }

        private void OnEnable()
        {
            OnStateChange += ChangeState;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
            }
            
            FirstInitialize();
        }

        private void FirstInitialize()
        {
            // Define the rotation based on state
            StateDirectionDictionary.Add(States.Forward, Quaternion.identity);
            StateDirectionDictionary.Add(States.Backward, Quaternion.Euler(0, 180, 0));
            StateDirectionDictionary.Add(States.Left, Quaternion.Euler(0, -90, 0));
            StateDirectionDictionary.Add(States.Right, Quaternion.Euler(0, 90, 0));
        }

        private void OnDisable()
        {
            OnStateChange -= ChangeState;
        }
        /// <summary>
        /// Current directions are change here by <see cref="Player.StateChanger.OnTriggerEnter"/> firing
        /// </summary>
        private void ChangeState(States[] newStates)
        {
            currentStates[0] = newStates[0];
            currentStates[1] = newStates[1];
        }
    }
}