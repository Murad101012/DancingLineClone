using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main script where actual moving happen and controls Player.prefab
    /// </summary>
    public class Movement : MonoBehaviour
    {
        private Transform _playerTransform;
        [SerializeField] private int speed = 3;
        private bool _switchOrder;
        public static event Action PlayerPressed;

        private void Awake()
        {
            _playerTransform = transform;
        }

        private void Update()
        {
            MovePlayerToZIndex();
            SwitchOrder();
        }
        
        /// <remarks>
        /// Moving done by only where Z axis pointing, not by chancing player's rotation.
        /// </remarks>
        private void MovePlayerToZIndex()
        {
            _playerTransform.position += _playerTransform.forward * speed * Time.deltaTime;
        }
        
        /// <summary>
        /// Player cube rotation change in runtime based on Space button
        /// </summary>
        private void SwitchOrder()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Switch between 0 and 1 currentStates with _switchOrder boolean
                if (!_switchOrder)
                {
                    _switchOrder = true;
                    _playerTransform.rotation = MoveState.Instance.StateDirectionDictionary[MoveState.Instance.currentStates[0]];
                }
                else
                {
                    _switchOrder = false;
                    _playerTransform.rotation = MoveState.Instance.StateDirectionDictionary[MoveState.Instance.currentStates[1]];
                }
                PlayerPressed?.Invoke();
            }
        }
    }
}