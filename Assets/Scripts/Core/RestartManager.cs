using Interfaces;
using UnityEngine;

namespace Core
{
    //TODO: Do better naming since it's only change player transform not whole game's Restart parameters
    public class RestartManager : MonoBehaviour, IOnRestart
    {
        private ObjectStatsSo _objectStatsSo;

        private void OnEnable()
        {
            if (LevelRegistrySo.Instance == null) return;
            LevelRegistrySo.Instance.Register(this);
            
            //Creating run-time ObjectStatsSO
            _objectStatsSo = ScriptableObject.CreateInstance<ObjectStatsSo>();
            _objectStatsSo.firstLevelBeginPosition = transform.position;
            _objectStatsSo.firstLevelBeginRotation = transform.rotation;
        }

        private void OnDisable()
        {
            if (LevelRegistrySo.Instance == null) return;
            LevelRegistrySo.Instance.Unregister(this);
            
            //Creating run-time ObjectStatsSO
            Destroy(_objectStatsSo);
        }

        /// <summary>
        /// If Object triggered a checkpoint it will be removed if object restart the level
        /// </summary>
        public void OnLevelRestart()
        {
            transform.position = _objectStatsSo.firstLevelBeginPosition;
            transform.rotation = _objectStatsSo.firstLevelBeginRotation;
        }
    }
}