using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ProgressInCurrentLoadedLevel")]
    public class ProgressInCurrentLoadedLevel : ScriptableObject
    {
        public float audioDuration;
        public float playbackInAudioWhenPlayerDead;
        
        //For future reference
        public int collectedCoins;
        public int collectedCrowns;
    }
}