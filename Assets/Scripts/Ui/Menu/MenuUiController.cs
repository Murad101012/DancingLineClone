using System;
using Core;
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
    public class MenuUiController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private MenuUiElementReference _menuUiElementReference;
        
        
        [SerializeField] private LevelLoadEventSo levelLoadEventSo;
        
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
            _menuUiElementReference.LevelLoadButtonReference.clicked += ClickedButton;
        }
        
        private void ClickedButton()
        {
            levelLoadEventSo.RaiseOnLevelNameLoad("LevelCreateTemplate");
        }

        private void OnDestroy()
        {
            _menuUiElementReference.LevelLoadButtonReference.clicked -= ClickedButton;
        }
    }
}