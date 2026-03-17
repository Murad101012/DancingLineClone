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
    public class RestartManager : MonoBehaviour, IOnRestart, ILevelState, IReady
    {
        private CinemachineBrain _cineMachineBrain;
        private CinemachineCamera[] _cameras;
        [SerializeField] private Transform cineMachineCamerasParent;
        [SerializeField] private CinemachineCamera cameraAtBeginning;

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
            LevelLoader.Instance?.RegisterIReady(this);

            _cineMachineBrain = GetComponent<CinemachineBrain>();

            if (cineMachineCamerasParent != null) return;
            Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent is null, disabling the Restart feature for Camera");
            enabled = false;
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
            cameraAtBeginning.Priority = 1;
        }
        
        public void Initialization()
        {
            //Getting all CineMachine cameras under parent and loading to _cameras variable
            _cameras = cineMachineCamerasParent.GetComponentsInChildren<CinemachineCamera>(true);
            if (_cameras.Length == 0 || _cameras == null)
            {
                Debug.LogWarning("Camera/RestartManager: cineMachineCamerasParent doesn't have children" +
                                 " with CineMachineCamera component,  disabling the Restart feature for Camera");
                enabled = false;
            }
        }

        public void OnLevelStart()
        {
            //Delaying the code, potentially brain hasn't assigned the virtualCamera yet.
            if (cameraAtBeginning == null)
            {
                cameraAtBeginning = (CinemachineCamera)_cineMachineBrain.ActiveVirtualCamera;
                Debug.LogWarning("RestartManager: Beginning Camera isn't set. Taking camera that active when player begin to play.");
            }
        }

        public void OnLevelStop() {/*It will be empty*/}
    }
}