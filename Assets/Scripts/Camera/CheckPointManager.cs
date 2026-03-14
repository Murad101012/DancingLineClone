using System;
using Core;
using Interfaces;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    /// <summary>
    /// When player begin to level at last checkpoint, CineMachine Brain's active virtual camera
    /// will switch to the last CineMachine Camera component at CheckPoint cause by <see cref="Gameplay.CheckpointTrigger"/>
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    public class CheckPointManager : MonoBehaviour, IOnCheckPoint, IOnRestart
    {
        private CinemachineBrain _cineMachineBrain;
        private CinemachineCamera _cameraAtCheckPoint;
        private bool _playerCheckPointHappen;

        private void OnEnable()
        {
            Player.CheckPointManager.OnCheckpointUpdated += OnCheckPointUpdated;
        }

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
            _cineMachineBrain = GetComponent<CinemachineBrain>();
        }

        private void OnDisable()
        {
            Player.CheckPointManager.OnCheckpointUpdated -= OnCheckPointUpdated;
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        private void OnCheckPointUpdated()
        {
            //TODO: GEMINI: The Risk: If you ever use a CinemachineFreeLook or a different camera type in the future, this code will crash with an InvalidCastException.
            _cameraAtCheckPoint = (CinemachineCamera)_cineMachineBrain.ActiveVirtualCamera;

            _playerCheckPointHappen = true;
        }

        public void OnLevelCheckPoint()
        {
            if (!_playerCheckPointHappen) return;
            //We check if the current camera at CineMachine brain using is same when at checkpoint
            CinemachineCamera currentCamera = (CinemachineCamera)_cineMachineBrain.ActiveVirtualCamera;
            if (currentCamera == _cameraAtCheckPoint) return;
            
            /*If cameras are not same, then current one will be Non-priortized (0)
             and Camera at checkPoint will be priortized instead
            */
            currentCamera.Priority = 0;
            _cameraAtCheckPoint.Priority = 1;
        }

        //If player Restart the level, latest Checkpoint Camera will be reset
        public void OnLevelRestart()
        {
            _cameraAtCheckPoint = null;
            _playerCheckPointHappen = false;
        }
    }
}