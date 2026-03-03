using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Current move directions update in <see cref="_currentStates"/>
    /// </summary>
    /// <remarks>
    /// For get the latest directions, <see cref="Movement"/> script must take current directions from <see cref="_currentStates"/>
    /// </remarks>
    public class MoveState : MonoBehaviour, IStateSwitchable // Signing the contract
    {
        public enum States
        {
            Forward,
            Backward,
            Left,
            Right,
        }
        
        private States[] _currentStates = new States[2];
        private Quaternion[] _currentStatesAsQuaternions = new Quaternion[2];
        private Dictionary<States, Quaternion> _stateDirectionDictionary = new(4);
        
        public static event Action<Quaternion[]> OnStateChange;
        private void PassStageChangePlayer()
        {
            //1.Transform States to Quaternions to find where to player must rotate based on directions (states)
            _currentStatesAsQuaternions[0] = _stateDirectionDictionary[_currentStates[0]];
            _currentStatesAsQuaternions[1] = _stateDirectionDictionary[_currentStates[1]];
            
            //2.Fire event and send new directions to player as quaternion
            OnStateChange?.Invoke(_currentStatesAsQuaternions);
        }
        

        private void Awake()
        {
            FirstInitialize();
        }

        private void FirstInitialize()
        {
            // Define the rotation based on state
            _stateDirectionDictionary.Add(States.Forward, Quaternion.identity);
            _stateDirectionDictionary.Add(States.Backward, Quaternion.Euler(0, 180, 0));
            _stateDirectionDictionary.Add(States.Left, Quaternion.Euler(0, -90, 0));
            _stateDirectionDictionary.Add(States.Right, Quaternion.Euler(0, 90, 0));
        }
        
        /// <summary>
        /// Current directions are change here by <see cref="Player.StateChanger.OnTriggerEnter"/> firing
        /// </summary>
        public void ChangeState(States[] newStates)
        {
            _currentStates[0] = newStates[0];
            _currentStates[1] = newStates[1];
            
            //Passing new state changes to Movement.cs
            PassStageChangePlayer();
        }
    }
}