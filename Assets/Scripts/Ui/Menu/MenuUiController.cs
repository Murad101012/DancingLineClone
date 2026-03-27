using System;
using Core;
using Gameplay;
using Interfaces;
using Ui.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Menu
{
    /// <summary>
    /// Responsible to control Menu's UI (e.g: changing behavior of an element (Interactable, Opacity, Enabled)) at level selection,
    /// acting as centralized hub for connecting buttons)
    /// </summary>
    /// <remarks> To prevent using direct string references to find a Visual Element in UIToolkit hierarchy,
    /// you must get references from <see cref="MenuUiElementReference"/> to prevent typos and get latest Visual Element
    /// names if their name changed</remarks>
    [RequireComponent(typeof(MenuUiElementReference))]
    [RequireComponent(typeof(UIDocument))]
    public class MenuUiController : MonoBehaviour, ILevelPreviewChange
    {
        private UIDocument _uiDocument;
        private MenuUiElementReference _menuUiElementReference;
        private readonly StyleBackground _nullStyleBackground = StyleKeyword.Null;
        public static event Action OnLoadLevelButtonClicked;

        private void OnEnable()
        {
            LevelPreviewChangeSender.OnLevelPreviewChange += OnLevelPreviewChange;
        }

        private void Awake()
        {
            _menuUiElementReference = GetComponent<MenuUiElementReference>();
        }
        
        private void Start()
        {
            if (!_menuUiElementReference.CheckFinished)
            {
                string updateTextOfClass = nameof(MenuUiElementReference);
                Debug.LogWarning($"{name}: checkFinished is false, can't get reference to UI elements. So, " +
                                 $"disabling the MenuUiController. (Tip: check if race-condition happen (Meaning if {name} begin" +
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
            _menuUiElementReference.LevelLoadButtonReference.clicked += LoadLevelButton;
        }
        
        private void OnDisable()
        {
            LevelPreviewChangeSender.OnLevelPreviewChange -= OnLevelPreviewChange;
        }
        
        private void LoadLevelButton()
        {
            OnLoadLevelButtonClicked?.Invoke();
        }

        private void OnDestroy()
        {
            _menuUiElementReference.LevelLoadButtonReference.clicked -= LoadLevelButton;
        }

        public void OnLevelPreviewChange(LevelPropertiesSo levelPropertiesSo)
        {
            //Chancing name
            _menuUiElementReference.LevelLabelNameReference.text = levelPropertiesSo.levelName;
            
            //Chancing image
            _menuUiElementReference.LevelPreviewImageReference.style.backgroundImage = 
                levelPropertiesSo.styleBackgroundLevelImage != null
                    ? levelPropertiesSo.styleBackgroundLevelImage : _nullStyleBackground;
            
            //Disabling/enabling the buttons interactable if player first/last or between levels
            if (levelPropertiesSo.levelIndex == 0)
            {
                _menuUiElementReference.LevelChangePreviousLevelButtonReference.SetEnabled(false);
            }
            if (levelPropertiesSo.levelIndex == levelPropertiesSo.totalLevels - 1)
            {
                _menuUiElementReference.LevelChangeNextLevelButtonReference.SetEnabled(false);
                return;
            }
            _menuUiElementReference.LevelChangePreviousLevelButtonReference.SetEnabled(true);
            _menuUiElementReference.LevelChangeNextLevelButtonReference.SetEnabled(true);
        }
    }
}