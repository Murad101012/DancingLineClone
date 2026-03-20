using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Menu
{
    /// <summary>
    /// Responsible to control Menu's UI (e.g: changing behavior of an element (Interactable, Opacity, Enabled)) at level selection
    /// </summary>
    /// <remarks> To prevent using direct string references, you must first add reference to <see cref="MenuUiElementReference"/>
    /// and get reference to string variable instead (Check MenuUiName for further information)</remarks>
    [RequireComponent(typeof(MenuUiElementReference))]
    [RequireComponent(typeof(UIDocument))]
    public class MenuUiController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private MenuUiElementReference _menuUiElementReference;
        
        private void Start()
        {
            _menuUiElementReference = GetComponent<MenuUiElementReference>();

            if (!_menuUiElementReference.CheckFinished)
            {
                Debug.LogWarning($"{name}: checkFinished is false, can't get reference to UI elements. So, disabling the MenuUiController." +
                                 $"(Tip: check if race-condition happen (Meaning if {name} begin" +
                                 " first before MenuUiElementReference finish it's Initialization(). " +
                                 "If problem is race-condition then proceed to use IReady interface if it suits to current situation.)");
                enabled = false;
                return;
            }

            if (!_menuUiElementReference.CheckResult)
            {
                Debug.LogWarning(
                    $"{name}: checkResult is false, can't get reference to UI elements. So, disabling the MenuUiController.");
                enabled = false;
                return;
            }
            
            Debug.Log(_menuUiElementReference.TestButtonReference.text);
        }
        
    }
}