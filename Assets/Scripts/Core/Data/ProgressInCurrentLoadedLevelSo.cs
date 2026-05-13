using UnityEngine;

namespace Core.Data
{
    /// <summary>
    /// Source of truth for information about what achieved and how much progressed
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/ProgressInCurrentLoadedLevel")]
    public class ProgressInCurrentLoadedLevelSo : ScriptableObject
    {
        public float audioDuration;
        public float playbackInAudioWhenPlayerDead;
        
        //For future reference
        public int collectedCoins;
        public int collectedCrowns;
    }
}