using System;
using Core;
using Interfaces;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    /// <summary>
    /// It will reset all cameras priority to zero except the CineMachine Camera at the beginning of the level
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    public class RestartManager : MonoBehaviour, IOnRestart
    {
        private CinemachineBrain _cineMachineBrain;
        private CinemachineCamera[] _cameras;
        [SerializeField] private Transform cineMachineCamerasParent;
        private CinemachineCamera _cameraAtBeginning;

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);

            _cineMachineBrain = GetComponent<CinemachineBrain>();
            
            if (cineMachineCamerasParent == null)
            {
                Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent is null, disabling the Restart feature for Camera");
                enabled = false;
                return;
            }
            _cameras = cineMachineCamerasParent.GetComponentsInChildren<CinemachineCamera>();
            
            if (_cameras != null) return;
            Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent doesn't have children" +
                             " with CineMachineCamera component,  disabling the Restart feature for Camera");
            enabled = false;
        }

        private void Start()
        {
            //Delaying the code, potentially brain hasn't assigned the virtualCamera yet.
            _cameraAtBeginning = (CinemachineCamera)_cineMachineBrain.ActiveVirtualCamera;
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        public void OnLevelRestart()
        {
            //Resetting all CineMachine Camera component's priority value to 0
            for (int i = 0; i < _cameras.Length; i++)
            {
                _cameras[i].Priority = 0;
            }
            
            //Making most priortiest the CineMachine Camera that at the beginning of the level
            _cameraAtBeginning.Priority = 1;
        }
    }
}