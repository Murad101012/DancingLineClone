using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    /// <summary>
    /// Useful when chancing priority of a camera and make switch between Cinemachine Cameras
    /// Can be use with CameraPriorityChanger.prefab
    /// </summary>
    public class CameraPriorityChanger : MonoBehaviour
    {
        /// <summary>
        /// Drag the camera you want to change its priority value
        /// </summary>
        [SerializeField] private CinemachineCamera zoneCamera;
        [SerializeField] private int activePriority = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                zoneCamera.Priority = activePriority;
            }
        }
    }
}