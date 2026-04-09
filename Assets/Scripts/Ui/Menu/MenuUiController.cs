using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core;
using Gameplay;
using Interfaces;
using Unity.Mathematics;
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
        [SerializeField] MenuOnLevelInPreviewChangeSo menuOnLevelInPreviewChangeSo;
        
        //Cache
        private readonly BackgroundSize _backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
        private readonly BackgroundPosition _backgroundPosition = new BackgroundPosition(BackgroundPositionKeyword.Center);

        
        private void OnEnable()
        {
            if (menuOnLevelInPreviewChangeSo == null)
            {
                Debug.LogWarning($"{name}: {nameof(menuOnLevelInPreviewChangeSo)} is null. Chancing name and background not possible");
                return;
            }
            menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent += OnLevelPreviewChange;
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
            //_menuUiElementReference.LevelLoadButtonReference.clicked += LoadLevelButton;
        }
        
        

        private void OnDisable()
        {
            menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent -= OnLevelPreviewChange;
        }

        private void OnLevelPreviewChange()
        {
            //Chancing name
            _menuUiElementReference.LevelLabelNameReference.text =
                menuOnLevelInPreviewChangeSo.levelInPreview.levelName;
            
            //Chancing background-image
            _menuUiElementReference.Root.style.backgroundImage =
                menuOnLevelInPreviewChangeSo.levelInPreview.styleBackgroundLevelImage;
            
            //Since setting this in root class container ignored when we set background-image as style, we need to implement in C# manually
            /*By Gemini: Modern replacement for -unity-background-scale-mode: scale-and-crop */
            // 1. Force 'Cover' (Scale and Crop) via C#
            _menuUiElementReference.Root.style.backgroundSize = _backgroundSize;

            // 2. Force 'Center' alignment
            _menuUiElementReference.Root.style.backgroundPositionX = _backgroundPosition;
            _menuUiElementReference.Root.style.backgroundPositionY = _backgroundPosition;
        }
    }
}