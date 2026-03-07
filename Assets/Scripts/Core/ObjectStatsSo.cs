using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ObjectStats")]
    public class ObjectStatsSo : ScriptableObject
    {
        [Header("Movement")]
        public int speed;
        
        [Header("Level Initialization - First Beginning")]
        public Vector3 firstLevelBeginPosition;
        public Quaternion firstLevelBeginRotation;
        
        [Header("Level Initialization - Current Checkpoint")]
        public Vector3 currentCheckpointPosition;
        public Quaternion currentCheckpointRotation;
    }
}