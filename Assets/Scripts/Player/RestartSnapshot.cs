using UnityEngine;

namespace Player
{
    
    public class RestartSnapshot: MonoBehaviour
    {
        public Vector3 FirstLevelBeginPosition { get; private set; }
        public Quaternion FirstLevelBeginRotation { get; private set; }

        private void Awake()
        {
            FirstLevelBeginPosition = transform.position;
            FirstLevelBeginRotation = transform.rotation;
        }
    }
}