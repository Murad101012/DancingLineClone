using System;
using Gameplay;
using Ui.Menu;
using UnityEngine;

namespace Ui.Core
{
    /// <summary>
    /// Triggered when the player cycles through level selections to update preview elements.
    /// </summary>
    /// <remarks><see cref="MenuUiElementReference"/> is require for getting references to
    /// <see cref="MenuUiElementReference.LevelChangeNextLevelButtonReference"/> and
    /// <see cref="MenuUiElementReference.LevelChangePreviousLevelButtonReference"/></remarks>
    [RequireComponent(typeof(MenuUiElementReference))]
    public class LevelPreviewChangeSender : MonoBehaviour
    {
        [SerializeField] LevelPropertiesSo[] levelPropertiesSo;
        private int _index = 0;
        public static event Action<LevelPropertiesSo> OnLevelPreviewChange;
        //TODO: Change this reference to Dependency Injection,
        // and eliminate RequireComponent and dependency
        private MenuUiElementReference _menuUiElementReference;

        private void Awake()
        {
            _menuUiElementReference = GetComponent<MenuUiElementReference>();

            //Adding awareness to each LevelPropertiesSo for it's alignment
            for (int i = 0; i < levelPropertiesSo.Length; i++)
            {
                levelPropertiesSo[i].totalLevels = levelPropertiesSo.Length;
                levelPropertiesSo[i].levelIndex = i;
            }
        }
        
        private void Start()
        {
            if (!_menuUiElementReference.CheckFinished)
            {
                string updateTextOfClass = nameof(MenuUiElementReference);
                Debug.LogWarning($"{name}: checkFinished is false, can't get reference to UI elements. So, " +
                                 $"disabling the {name}. (Tip: check if race-condition happen (Meaning if {name} begin" +
                                 $" first before {updateTextOfClass} finish it's Initialization(). " +
                                 "If problem is race-condition then proceed to use IReady interface if it suits to current situation.)");
                enabled = false;
                return;
            }

            if (!_menuUiElementReference.CheckResult)
            {
                Debug.LogWarning(
                    $"{name}: checkResult is false, can't get reference to UI elements. So, disabling the {name}.");
                enabled = false;
                return;
            }
            
            Initialization();
        }

        private void Initialization()
        {
            _menuUiElementReference.LevelChangeNextLevelButtonReference.clicked +=
                LevelChangeNextLevelButtonReferenceOnClicked;
            _menuUiElementReference.LevelChangePreviousLevelButtonReference.clicked +=
                LevelChangePreviousLevelButtonReferenceOnClicked;
            
            InvokeOnLevelPreviewChange(levelPropertiesSo[_index]);
        }

        private void LevelChangePreviousLevelButtonReferenceOnClicked()
        {
            if (_index <= 0) return;
            _index--;
            InvokeOnLevelPreviewChange(levelPropertiesSo[_index]);
        }

        private void LevelChangeNextLevelButtonReferenceOnClicked()
        {
            if (_index >= levelPropertiesSo.Length - 1) return;
            _index++;
            InvokeOnLevelPreviewChange(levelPropertiesSo[_index]);
        }

        private void OnDestroy()
        {
            _menuUiElementReference.LevelChangeNextLevelButtonReference.clicked -=
                LevelChangeNextLevelButtonReferenceOnClicked;
            _menuUiElementReference.LevelChangePreviousLevelButtonReference.clicked -=
                LevelChangePreviousLevelButtonReferenceOnClicked;
        }


        private void InvokeOnLevelPreviewChange(LevelPropertiesSo obj)
        {
            OnLevelPreviewChange?.Invoke(obj);
        }
    }
}