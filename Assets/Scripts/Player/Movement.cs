using System;
using System.Collections;
using Core;
using Gameplay;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main script where actual moving happen and controls Player.prefab
    /// </summary>
    public class Movement : MonoBehaviour, ILevelState
    {
        [SerializeField] private ObjectStatsSo objectStatsSo;
        private Transform _playerTransform;
        private bool _switchOrder;
        
        /// <remarks> This boolean duplicates from <see cref="MoveState._currentStates"/></remarks>
        private Quaternion[] _currentStatesAsQuaternions = new Quaternion[2];
        
        /// <remarks> This boolean duplicates from <see cref="GroundStateChecker._onGround"/></remarks>
        private bool _onGround = true;
        /// <remarks> This boolean duplicates from <see cref="GroundStateChecker._onNonGround"/></remarks>
        private bool _onNonGround;

        
        private Coroutine _movePlayerToZIndexCoroutine;
            
        public static event Action PlayerPressed;
        public static event Action Dead;

        private void OnEnable()
        {
            GroundStateChecker.OnGroundChange += OnGroundStateChangeUpdater;
            GroundStateChecker.OnNonGroundChange += OnNonGroundStateChangeUpdater;
            MoveState.OnStateChange += OnStateChange;
            
            if (LevelRegistrySo.Instance == null) return;
            LevelRegistrySo.Instance.Register(this);
        }

        private void Awake()
        {
            _playerTransform = transform;
            if (objectStatsSo == null)
            {
                Debug.LogWarning($"objectStatsSo is not assigned, using dummy objectStatsSo with default values for {name}");
                objectStatsSo = ScriptableObject.CreateInstance<ObjectStatsSo>();
                objectStatsSo.speed = 3;
            }
        }

        private void Update()
        {
            SwitchOrder();
        }

        private void OnDisable()
        {
            GroundStateChecker.OnGroundChange -= OnGroundStateChangeUpdater;
            GroundStateChecker.OnNonGroundChange -= OnNonGroundStateChangeUpdater;
            MoveState.OnStateChange -= OnStateChange;
            
            if (LevelRegistrySo.Instance == null) return;
            LevelRegistrySo.Instance.Unregister(this);
        }
        
        public void OnLevelStart()
        {
            _movePlayerToZIndexCoroutine = StartCoroutine(MovePlayerToZIndex());
        }
        
        public void OnLevelStop()
        {
            if (_movePlayerToZIndexCoroutine != null)
            {
                StopCoroutine(_movePlayerToZIndexCoroutine);
            }
        }

        private void OnStateChange(Quaternion[] newStatesAsQuaternions)
        {
            _currentStatesAsQuaternions[0] = newStatesAsQuaternions[0];
            _currentStatesAsQuaternions[1] = newStatesAsQuaternions[1];
        }
        
        
        /// <remarks>
        /// Moving done by only where Z axis pointing, not by chancing player's rotation.
        /// </remarks>
        //TODO: Change transform.position to RigidBody for prevent teleport and transform to Update()
        private IEnumerator MovePlayerToZIndex()
        {
            //Player can only walk if it's on Ground Layer.
            while (!_onNonGround)
            {
                _playerTransform.position += _playerTransform.forward * objectStatsSo.speed * Time.deltaTime;
                yield return null;
            }
        }
        
        /// <summary>
        /// Player cube rotation change in runtime based on Space button and state of <see cref="_onGround"/>
        /// </summary>
        private void SwitchOrder()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _onGround)
            {
                //Switch between 0 and 1 currentStates with _switchOrder boolean
                //TODO: Learn States and replace if with state logic
                if (!_switchOrder)
                {
                    _switchOrder = true;
                    _playerTransform.rotation = _currentStatesAsQuaternions[0];
                }
                else
                {
                    _switchOrder = false;
                    _playerTransform.rotation = _currentStatesAsQuaternions[1];
                }
                PlayerPressed?.Invoke();
            }
        }

        private void OnGroundStateChangeUpdater(bool currentState)
        {
            _onGround = currentState;
        }
        
        private void OnNonGroundStateChangeUpdater(bool currentState)
        {
            _onNonGround = currentState;
            
            //TODO: Move Dead section to own separate function in future
            if (currentState)
            {
                Dead?.Invoke();
            }
        }
    }
}