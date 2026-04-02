using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core;
using Gameplay;
using Interfaces;
using Ui.Core;
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
    public class MenuUiController : MonoBehaviour, ILevelPreviewChange
    {
        private UIDocument _uiDocument;
        private MenuUiElementReference _menuUiElementReference;
        
        private bool _holdingTheMouseOnWheel;
        Translate _wheelTranslateOnCursor;
        private VisualElement _targetWhenClicked;
        private float _sizeOfWheelXMin;
        private float _sizeOfWheelXMax;
        
        private Vector2 _startPos;
        private bool _hasMovedSignificantly;
        
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
            //_menuUiElementReference.LevelLoadButtonReference.clicked += LoadLevelButton;
            _menuUiElementReference.FilmLevelReference.RegisterCallback<PointerDownEvent>(ClickingTheWheel, TrickleDown.TrickleDown);
            _menuUiElementReference.FilmLevelReference.RegisterCallback<PointerUpEvent>(LeftTheWheelToHold);
            _menuUiElementReference.FilmLevelReference.RegisterCallback<PointerMoveEvent>(MovingTheWheel);

            
            _menuUiElementReference.FilmLevelReference.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _sizeOfWheelXMin = -_menuUiElementReference.FilmLevelReference.layout.width + _menuUiElementReference.Root.layout.width / 2;
                _sizeOfWheelXMax = _menuUiElementReference.Root.layout.width / 2;
            });
        }

        private void ClickingTheWheel(PointerDownEvent evt)
        {
            _targetWhenClicked = evt.target as VisualElement;
            _holdingTheMouseOnWheel = true;
            _startPos = evt.position; 
            _hasMovedSignificantly = false; //Resetting the flag
            //Allows to move wheel even outside the wheel
            _menuUiElementReference.FilmLevelReference.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void MovingTheWheel(PointerMoveEvent evt)
        {
            if (!_holdingTheMouseOnWheel) return;

            /*This where actual moving happen calculate
            We take the current translate.x position of wheel*/
            float currentX = _menuUiElementReference.FilmLevelReference.style.translate.value.x.value;
            
            /*Modify cached Translate variable by adding current wheel translate.x and add evt.deltaPosition.x which shows how much the finger moved from the last time
            Thankfully, deltaPosition return directly distance between current and last time so, we just add those both to each other.*/
            _wheelTranslateOnCursor.x = Mathf.Clamp(currentX + evt.deltaPosition.x, _sizeOfWheelXMin, _sizeOfWheelXMax); //Since we're only moving wheel in x position, we make other's 0
            _wheelTranslateOnCursor.y = 0;
            _wheelTranslateOnCursor.z = 0;
            _menuUiElementReference.FilmLevelReference.style.translate = _wheelTranslateOnCursor;

            /*At the last we check distance of mouse's x position between the first time player clicked the screen and second time its drag.
              If distance is more than 10, meaning player drag significant distance and intend this as a drag on wheel for swap between levels
              and making _hasMovedSignificantly true to inform the LeftTheWheelToHold() this is not a click*/
            if (math.abs(_startPos.x - evt.position.x) + math.abs(_startPos.y - evt.position.y) > 10f)
            {
                _hasMovedSignificantly = true;
            }
        }

        private void LeftTheWheelToHold(PointerUpEvent evt)
        {
            if (!_holdingTheMouseOnWheel) return;

            /*This boolean depends on how much player drag the wheel. If drag distance less than 10,
              we assume player tried to click on a level, so we send load level signal*/
            if (!_hasMovedSignificantly)
            {
                if (_targetWhenClicked is Button btn)
                {
                    Debug.Log($"Level Selected: {btn.name}");
                }
            }

            _holdingTheMouseOnWheel = false;
            _hasMovedSignificantly = false;
            _menuUiElementReference.FilmLevelReference.ReleasePointer(evt.pointerId);
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
            //_menuUiElementReference.LevelLoadButtonReference.clicked -= LoadLevelButton;
            _menuUiElementReference.FilmLevelReference.UnregisterCallback<PointerDownEvent>(ClickingTheWheel);
            _menuUiElementReference.FilmLevelReference.UnregisterCallback<PointerUpEvent>(LeftTheWheelToHold);
            _menuUiElementReference.FilmLevelReference.UnregisterCallback<PointerMoveEvent>(MovingTheWheel);

        }

        public void OnLevelPreviewChange(LevelPropertiesSo levelPropertiesSo)
        {
            //Chancing name
            _menuUiElementReference.LevelLabelNameReference.text = levelPropertiesSo.levelName;
            
            //Disabling/enabling the buttons interactable if player first/last or between levels
            if (levelPropertiesSo.levelIndex == 0)
            {
                _menuUiElementReference.LevelChangePreviousLevelButtonReference.SetEnabled(false);
                _menuUiElementReference.LevelChangeNextLevelButtonReference.SetEnabled(true);
            }
            else if (levelPropertiesSo.levelIndex == levelPropertiesSo.totalLevels - 1)
            {
                _menuUiElementReference.LevelChangeNextLevelButtonReference.SetEnabled(false);
                _menuUiElementReference.LevelChangePreviousLevelButtonReference.SetEnabled(true);
            }
            else
            {
                _menuUiElementReference.LevelChangePreviousLevelButtonReference.SetEnabled(true);
                _menuUiElementReference.LevelChangeNextLevelButtonReference.SetEnabled(true);
            }
        }
    }
}