using Gameplay;
using UnityEngine;

namespace Ui.Menu
{
    /// <summary> Add/Remove Levels </summary>
    /// <remarks>
    /// It's make easier to modify Levels inside the list without directly modifying UXML
    /// </remarks>
    public class MenuUiLevelController : MonoBehaviour
    {
        public LevelPropertiesSo[] levelPropertiesSo;
    }
}