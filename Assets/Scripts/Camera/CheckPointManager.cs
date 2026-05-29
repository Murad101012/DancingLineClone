using Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using DataContainer;

namespace Camera
{
    /// <summary>
    /// When player begin to level at last checkpoint, CineMachine Brain's active virtual camera
    /// will switch to the last CineMachine Camera component at CheckPoint cause by <see cref="Gameplay.CheckpointTrigger"/>
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    public class CheckPointManager : MonoBehaviour, IOnCheckPoint, IOnRestart, ILevelRegistryUser
    {
        [SerializeField] private Transform cineMachineCamerasParent;
        private CinemachineBrain _cineMachineBrain;
        private CinemachineCamera _cameraAtCheckPoint;
        private CinemachineCamera[] _cameras;
        private int[] _cinemachineCamerasSnapshotIndexs;
        private bool _playerCheckPointHappen;
        private ILevelRegistry _levelRegistry;
        [SerializeField] private ProgressInCurrentLoadedLevelSo progressInCurrentLoadedLevelSo;
        [SerializeField] private SceneLoadStateEventSo sceneLoadStateEventSo;


        private void OnEnable()
        {
            progressInCurrentLoadedLevelSo.OnCheckPointTrigger += OnCheckPointUpdated;
        }

        private void Awake()
        {
            _levelRegistry.Register(this);
            _cineMachineBrain = GetComponent<CinemachineBrain>();
            sceneLoadStateEventSo.OnSceneFullyLoaded += Initialization;
        }

        private void OnDisable()
        {
            progressInCurrentLoadedLevelSo.OnCheckPointTrigger -= OnCheckPointUpdated;
        }

        private void OnDestroy()
        {
            _levelRegistry.Unregister(this);
            sceneLoadStateEventSo.OnSceneFullyLoaded -= Initialization;
        }
        
        public void Initialization()
        {
            //Getting all CineMachine cameras under parent and loading to _cameras variable
            _cameras = cineMachineCamerasParent.GetComponentsInChildren<CinemachineCamera>(true);
            if (_cameras.Length == 0)
            {
                Debug.LogWarning($"{name}: cineMachineCamerasParent doesn't have children" +
                                 " with CineMachineCamera component,  disabling the CheckPoint feature for Camera");
                enabled = false;
                return;
            }
            
            //Taking all cinemachine cameras priorty at the checkpoint
            _cinemachineCamerasSnapshotIndexs = new int[_cameras.Length];
        }

        private void OnCheckPointUpdated()
        {
            for (int i = 0; i < _cameras.Length; i++)
            {
                _cinemachineCamerasSnapshotIndexs[i] = _cameras[i].Priority;
            }

            _playerCheckPointHappen = true;
        }

        public void OnLevelCheckPoint()
        {
            if (!_playerCheckPointHappen) return;
            
            _cineMachineBrain.enabled = false;
            for (int i = 0; i < _cameras.Length; i++)
            {
                _cameras[i].Priority = _cinemachineCamerasSnapshotIndexs[i];
            }
            _cineMachineBrain.enabled = true;
        }

        //If player Restart the level, checkpoint happens remove since player begins fresh level without checkpoint
        public void OnLevelRestart()
        {
            _playerCheckPointHappen = false;
        }
        
        public void LevelRegistrySoSetter(ILevelRegistry levelRegistry)
        {
            _levelRegistry = levelRegistry;
        }
    }
}