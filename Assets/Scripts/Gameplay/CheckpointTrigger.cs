using Interfaces;
using UnityEngine;

namespace Gameplay
{
    /// <remarks>
    /// For use this class, you must use CheckPoint.prefab and add prefab on level where checkpoint must be happen
    /// </remarks>
    public class CheckpointTrigger : MonoBehaviour
    {
        private bool _triggered;
        
        //Checkpoint check happen when player trigger CheckPoint.prefab
        private void OnTriggerEnter(Collider other)
        {
            if (!_triggered)
            {
                ICheckPoint checkpoint = other.gameObject.GetComponent<ICheckPoint>();
                if (checkpoint != null)
                {
                    _triggered = true;
                    checkpoint.CheckPoint();
                }
            }
        }
    }
}