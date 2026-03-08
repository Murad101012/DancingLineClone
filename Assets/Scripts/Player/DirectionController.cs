using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Current move directions update in <see cref="_currentDirections"/>
    /// </summary>
    /// <remarks>
    /// For get the latest directions, <see cref="PlayerCoreLogic"/> script must take current directions from <see cref="_currentDirections"/>
    /// </remarks>
    public class DirectionController : MonoBehaviour, IDirectionSwitchable // Signing the contract
    {
        public enum Directions
        {
            Forward,
            Backward,
            Left,
            Right,
        }
        
        private Directions[] _currentDirections = new Directions[2];
        public Quaternion[] CurrentDirectionsAsQuaternions { get; private set; } = new Quaternion[2];
        private Dictionary<Directions, Quaternion> _directionDictionary = new(4);
        
        public event Action<Quaternion[]> OnDirectionChange;
        private void PassStageChangePlayer()
        {
            //1.Transform Directions enum to Quaternions to find where to player must rotate
            CurrentDirectionsAsQuaternions[0] = _directionDictionary[_currentDirections[0]];
            CurrentDirectionsAsQuaternions[1] = _directionDictionary[_currentDirections[1]];
            
            //2.Fire event and send new directions to player as quaternion
            OnDirectionChange?.Invoke(CurrentDirectionsAsQuaternions);
        }
        

        private void Awake()
        {
            FirstInitialize();
        }

        private void FirstInitialize()
        {
            // Define the rotation based on state
            _directionDictionary.Add(Directions.Forward, Quaternion.identity);
            _directionDictionary.Add(Directions.Backward, Quaternion.Euler(0, 180, 0));
            _directionDictionary.Add(Directions.Left, Quaternion.Euler(0, -90, 0));
            _directionDictionary.Add(Directions.Right, Quaternion.Euler(0, 90, 0));
        }
        
        /// <summary>
        /// Current directions are change here by <see cref="DirectionChanger.OnTriggerEnter"/> firing
        /// </summary>
        public void ChangeDirection(Directions[] newStates)
        {
            _currentDirections[0] = newStates[0];
            _currentDirections[1] = newStates[1];
            
            //Passing new direction changes to Movement.cs
            PassStageChangePlayer();
        }
    }
}