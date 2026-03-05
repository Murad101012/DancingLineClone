using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerStats")]
    public class PlayerStatsSo : ScriptableObject
    {
        [Header("Movement")]
        public int speed;
        
        [Header("Level Initialization")]
        public Vector3 firstLevelBeginPosition;
        public Quaternion firstLevelBeginRotation;
    }
}