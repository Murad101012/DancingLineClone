using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// When player get into collision that have <see cref="CheckpointTrigger"/> script on it, it will get checkpoint information
    /// </summary>
    public interface ICheckPointReceiver
    {
        void CheckPointReceive(Transform transformPlayer);
    }
}