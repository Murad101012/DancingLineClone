using Interfaces;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// When a checkpoint triggered, current transform of player will send to <see cref="Core.LevelStateManager"/> to process.
    /// </summary>
    public class CheckpointSender : MonoBehaviour, ICheckPoint
    {
        public void CheckPoint()
        {
            Core.LevelStateManager.CheckPointEvent?.Invoke(transform);
        }
    }
}