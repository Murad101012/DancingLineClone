using Interfaces;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-10)]
    public class SceneILevelRegistryUserBootstrapper : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] gameObjectUsesLevelRegistryUser;
        
        private void Awake()
        {
            LevelRegistrySo levelRegistrySo = ScriptableObject.CreateInstance<LevelRegistrySo>();
            for (int i = 0; i < gameObjectUsesLevelRegistryUser.Length; i++)
            {
                if (gameObjectUsesLevelRegistryUser[i] is ILevelRegistryUser user)
                {
                    user.LevelRegistrySoSetter(levelRegistrySo);
                }
                else
                {
                    Debug.LogWarning($"{gameObjectUsesLevelRegistryUser[i]} is not implement ILevelRegistryUser interface, skipping");
                }
            }
        }
    }
}