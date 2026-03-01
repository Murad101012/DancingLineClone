using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main script where actual moving happen and controls Player.prefab
    /// </summary>
    public class Movement : MonoBehaviour
    {
        private Transform _playerTransform;
        private Rigidbody _playerRigidbody;
        [SerializeField] private int speed = 3;
        private bool _switchOrder;
        
        [SerializeField] private float raycastDistance = 2;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask nonGroundLayer;
        [SerializeField] private float checkInterval = 0.01f;
        private Coroutine _groundCheckCoroutine;
        private WaitForSeconds _waitForSecondsGroundCheckInterval;
        private bool _onGround = true;
            
        public static event Action PlayerPressed;
        public static event Action<bool> GroundChange;

        private void Awake()
        {
            _playerTransform = transform;
            _playerRigidbody = _playerTransform.GetComponent<Rigidbody>();
            _waitForSecondsGroundCheckInterval = new WaitForSeconds(checkInterval);
            BeginGroundCheck();
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

        /// <summary>
        /// Making player only can move, change direction on Ground Layer with Raycast and <see cref="groundLayer"/> by masking
        /// </summary>
        private IEnumerator GroundCheckIEnumerator()
        {
            //Debug.Log(combinedMask.value);
            while (true)
            {
                yield return _waitForSecondsGroundCheckInterval;
                //BoxCast recommend by AI. Explore this if this is works and how it works.
                if (Physics.BoxCast(_playerTransform.position, new Vector3(0.45f, 0.1f, 0.45f), Vector3.down, _playerTransform.rotation, raycastDistance, groundLayer))
                {
                    if (!_onGround)
                    {
                        _onGround = true;
                        GravityDisable();
                        GroundChange?.Invoke(_onGround);
                    }
                }
                else
                {
                    if (_onGround)
                    {
                        _onGround = false;
                        GravityEnable();
                        GroundChange?.Invoke(_onGround);
                    }
                }
            }
            
        }
        
        private void BeginGroundCheck()
        {
            _groundCheckCoroutine = StartCoroutine(GroundCheckIEnumerator());
        }

        private void StopGroundCheck()
        {
            if (_groundCheckCoroutine != null)
            {
                StopCoroutine(_groundCheckCoroutine);
            }
        }

        private void GravityEnable()
        {
            _playerRigidbody.useGravity = true;
        }

        private void GravityDisable()
        {
            _playerRigidbody.useGravity = false;
        }
    }
}