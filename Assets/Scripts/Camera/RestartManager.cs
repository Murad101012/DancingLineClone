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
    public class RestartManager : MonoBehaviour, IOnRestart, ILevelState, ILevelRegistryUser
    {
        private CinemachineBrain _cineMachineBrain;
        private CinemachineCamera[] _cameras;
        [SerializeField] private Transform cineMachineCamerasParent;
        [SerializeField] private CinemachineCamera cameraAtBeginning;
        private LevelRegistrySo _levelRegistrySo;
        [SerializeField] private SceneFullyLoadedEventSo sceneFullyLoadedEventSo;

        private void Awake()
        {
            _levelRegistrySo.Register(this);
            sceneFullyLoadedEventSo.OnSceneFullyLoaded += Initialization;

            _cineMachineBrain = GetComponent<CinemachineBrain>();

            if (cineMachineCamerasParent != null) return;
            Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent is null, disabling the Restart feature for Camera");
            enabled = false;
        }

        private void OnDestroy()
        {
            _levelRegistrySo.Unregister(this);
            sceneFullyLoadedEventSo.OnSceneFullyLoaded -= Initialization;
        }

        public void OnLevelRestart()
        {
            //Resetting all CineMachine Camera component's priority value to 0
            for (int i = 0; i < _cameras.Length; i++)
            {
                _cameras[i].Priority = 0;
            }
            
            //Making most priortiest the CineMachine Camera that at the beginning of the level
            cameraAtBeginning.Priority = 1;
        }
        
        public void Initialization()
        {
            //Getting all CineMachine cameras under parent and loading to _cameras variable
            _cameras = cineMachineCamerasParent.GetComponentsInChildren<CinemachineCamera>(true);
            if (_cameras.Length != 0) return;
            Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent doesn't have children" +
                             " with CineMachineCamera component,  disabling the Restart feature for Camera");
            enabled = false;
        }

        public void OnLevelStart()
        {
            /*Since ActiveVirtualCamera is not initialized yet In Awake() and Start(), executing this code cause always  return null exception
              error, because CinemachineCamera isn't ready yet. So, instead it gets the camera that active as soon as player begin to
              play the level*/          
            if (cameraAtBeginning == null)
            {
                cameraAtBeginning = (CinemachineCamera)_cineMachineBrain.ActiveVirtualCamera;
                Debug.LogWarning($"{name}: '{nameof(cameraAtBeginning)}' was not assigned in the Inspector. " +
                                 $"Auto-assigned to '{cameraAtBeginning.name}'. " +
                                 "(Tip: Assign this manually to avoid performance overhead of auto-detection.)");
            }
        }

        public void OnLevelStop() {/*It will be empty*/}
        
        public void LevelRegistrySoSetter(LevelRegistrySo levelRegistrySo)
        {
            _levelRegistrySo = levelRegistrySo;
        }
    }
}