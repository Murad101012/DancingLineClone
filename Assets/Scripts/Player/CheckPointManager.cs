using System;
using Core;
using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// When a checkpoint triggered, current transform of object will be stored here for resetting
    /// </summary>
    //TODO: Do better naming since it's only change player transform not whole game's CheckPoint parameters
    public class CheckPointManager : MonoBehaviour, ICheckPointReceiver, IOnCheckPoint, IOnRestart
    {
        private ObjectStatsSo _objectStatsSo;
        private bool _checkPointTriggered;
        public static event Action OnCheckpointUpdated;

        private void OnEnable()
        {
            _objectStatsSo = ScriptableObject.CreateInstance<ObjectStatsSo>();
        }

        private void Awake()
        {
            LevelRegistrySo.Instance.Register(this);
        }

        private void OnDisable()
        {
            Destroy(_objectStatsSo);
        }

        private void OnDestroy()
        {
            LevelRegistrySo.Instance.Unregister(this);
        }

        /// <summary>
        /// This function called when <see cref="Gameplay.CheckpointTrigger"/> triggered
        /// </summary>
        public void CheckPointReceive(Transform transformPlayer)
        {
            //Setting only X and Z positions of trigger but keeping Y same as player's it's position
            _objectStatsSo.currentCheckpointPosition = new Vector3(transformPlayer.position.x, transform.position.y, transformPlayer.position.z);
            _objectStatsSo.currentCheckpointRotation = transform.rotation;
            _checkPointTriggered = true;
            OnCheckpointUpdated?.Invoke();
        }

        /// <summary>
        /// Resetting object position to checkpoint position
        /// </summary>
        public void OnLevelCheckPoint()
        {
            if (!_checkPointTriggered) return;
            transform.position = _objectStatsSo.currentCheckpointPosition;
            transform.rotation = _objectStatsSo.currentCheckpointRotation;
        }
        
        /// <summary>
        /// Restart function is here for if player Restart the level instead of continue from checkpoint,
        /// this function remove the checkpoint object got and reset to 0
        /// </summary>
        /// <remarks>Main Restarting happen at <see cref="RestartManager"/></remarks>
        public void OnLevelRestart()
        {
            if (!_checkPointTriggered) return;
            _objectStatsSo.currentCheckpointPosition = Vector3.zero;
            _objectStatsSo.currentCheckpointRotation = Quaternion.identity;
            _checkPointTriggered = false;
        }
    }
}