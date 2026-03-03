using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PlayerStats")]
    public class PlayerStatsSo : ScriptableObject
    {
        [Header("Movement")]
        public int speed;
    }
}