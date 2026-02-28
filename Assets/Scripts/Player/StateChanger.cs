using UnityEngine;

namespace Player
{
    /// <summary>
    /// Chancing state based on what set at moveStates array <see cref="moveStates"/>
    /// </summary>
    /// <remarks>
    /// Place the "StateChangerPrefab" in front of the player. 
    /// The transition occurs via <see cref="OnTriggerEnter"/>.
    /// </remarks>
    public class StateChanger : MonoBehaviour
    {
        [SerializeField] private MoveState.States[] moveStates = new MoveState.States[2];
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                MoveState.TriggerStateChange(moveStates);
            }
        }
    }
}