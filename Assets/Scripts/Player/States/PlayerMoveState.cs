using System;
using Interfaces;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Handles the player's movement logic, input-based rotation, and ground detection
    /// while the player is in the active 'Moving' state.
    /// </summary>
    /// <remarks> It's belong to </remarks>
    public class PlayerMoveState : IPlayerState
    {
        private readonly PlayerCoreLogic _playerCoreLogic;
        private Transform _movementTransform;
        private bool _switchOrder;
        private DirectionController _directionController;

        /// <remarks> This boolean duplicates from
        /// <see cref="DirectionController.CurrentDirectionsAsQuaternions"/></remarks>
        private Quaternion[] _currentDirectionsAsQuaternions = new Quaternion[2];
        /// <remarks> This boolean duplicates from <see cref="GroundStateChecker._onGround"/></remarks>
        private bool _onGround = true;

        public PlayerMoveState(PlayerCoreLogic playerCoreLogic) => _playerCoreLogic = playerCoreLogic;
        
        public static event Action PlayerPressed;

        
        public void StateBegin()
        {
            _movementTransform = _playerCoreLogic.transform;
            
            /*As soon as game begin script get the first Direction information since, with OnGroundChange event,
            it's miss the very first event since PlayerMoveState not MonoBehaviour and can't use OnEnable*/
            _directionController = _playerCoreLogic.GetComponent<DirectionController>();
            _currentDirectionsAsQuaternions = _directionController.CurrentDirectionsAsQuaternions;
            
            _directionController.OnDirectionChange += OnDirectionChange;
            GroundStateChecker.OnGroundChange += OnGroundStateChangeUpdater;
        }
        
        public void StateTick()
        {
            MovePlayerForwardZIndex();
            SwitchOrder();
        }
        
        /// <remarks>
        /// Moving done by only where Z axis pointing, not by chancing player's rotation.
        /// </remarks>
        //TODO: Change transform.position to RigidBody for prevent teleport and transform to Update()
        private void MovePlayerForwardZIndex()
        {
            _movementTransform.position += _movementTransform.forward * _playerCoreLogic.ObjectStatsSo.speed * Time.deltaTime;
        }
        
        /// <summary>
        /// Player cube rotation change in runtime based on Space button and state of <see cref="_onGround"/>
        /// </summary>
        private void SwitchOrder()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _onGround)
            {
                //Switch between 0 and 1 currentStates with _switchOrder boolean
                if (!_switchOrder)
                {
                    _switchOrder = true;
                    _movementTransform.rotation = _currentDirectionsAsQuaternions[0];
                }
                else
                {
                    _switchOrder = false;
                    _movementTransform.rotation = _currentDirectionsAsQuaternions[1];
                }
                PlayerPressed?.Invoke();
            }
        }
        
        private void OnDirectionChange(Quaternion[] newStatesAsQuaternions)
        {
            _currentDirectionsAsQuaternions[0] = newStatesAsQuaternions[0];
            _currentDirectionsAsQuaternions[1] = newStatesAsQuaternions[1];
        }

        private void OnGroundStateChangeUpdater(bool currentState)
        {
            _onGround = currentState;
        }
        
 
        public void StateEnd()
        {
            _directionController.OnDirectionChange -= OnDirectionChange;
            GroundStateChecker.OnGroundChange -= OnGroundStateChangeUpdater;

            /*Chancing _onGround = true, because when _onGround fired true when game begins,
            PlayerMoveState didn't subs to GroundStateChecker.OnGroundChange yet and since this
            script not destroyed but cached, _onGround must be begin as true when this state
            call to use again*/
            _onGround = true;
        }
    }
}