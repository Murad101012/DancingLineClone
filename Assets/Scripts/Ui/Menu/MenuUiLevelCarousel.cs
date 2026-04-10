using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Menu
{
    /// <summary>
    /// Carousel drag between Level selections and make event invoke (with <see cref="MenuOnLevelInPreviewChangeSo.ChangeLevelInPreview"/>) when player changes level in preview
    /// </summary>
    /// <remarks>
    /// It's UI Toolkit's Viewport write from scratch to achieve GPU based (With translate) performant by bypassing FlexBox recalculating.
    /// </remarks>
    /// <remarks>Gemini help to apply values to UI Toolkit (e.g for applying float to rotate of UI Toolkit: Angle -> Rotate -> StyleRotate</remarks>
    [RequireComponent(typeof(MenuUiElementReference))]
    public class MenuUiLevelCarousel : MonoBehaviour
    {
        private MenuUiElementReference _menuUiElementReference;
        private MenuUiLevelController _menuUiLevelController;

        private int _levelIndexInPreview;
        private Vector2 _areaWidthOfLevelIndexInPreview;
        
        private bool _holdingTheMouseOnWheel;
        private Translate _wheelTranslateOnCursor;
        private Translate _buttonTranslateOnCursor;
        private float _targetScrollX;
        private float _currentScrollX;
        private float _distanceBetweenTargetAndCurrentScrollX;
        private int _spaceBetweenLevelButtons;
        
        private Vector2 _startPos;
        private bool _hasMovedSignificantly;

        private float _scrollVelocity;
        private const float SmoothTime = 0.08f;
        
        //Cache values for Scale
        private float _distanceBetweenCenterFocusAndButton;
        private float _normalizedReverse;
        private Vector2 _scaleValueForLevelButton;
        private Scale _scaleUIForLevelButton;
        private StyleScale _styleScaleForLevelButton;
        
        //Cache values for Rotation
        private float _currentAngle;
        private IStyle _currentRotatingButtonStyle;
        private Angle _currentRotatingButtonAngle;
        private Rotate _currentRotatingButtonRotate;
        private StyleRotate _currentRotatingButtonStyleRotate;
        
        private event Action OnSpaceBetweenLevelChange;
        public static event Action OnLoadLevelButtonClicked;
        
        [SerializeField] private MenuOnLevelInPreviewChangeSo menuOnLevelInPreviewChangeSo;

        private void OnEnable()
        {
            OnSpaceBetweenLevelChange += ApplySpaceToBetweenOfLevelButtons;
            OnSpaceBetweenLevelChange += UpdateAreaWidthForCurrentButtonInPreview;
        }

        private void Awake()
        {
            _menuUiElementReference = GetComponent<MenuUiElementReference>();
            _menuUiLevelController = GetComponent<MenuUiLevelController>();
        }
        
        private void Start()
        {
            if (menuOnLevelInPreviewChangeSo == null)
            {
                Debug.LogWarning($"{name}: {nameof(menuOnLevelInPreviewChangeSo)} is null. Sending level preview change not possible");
            }
            else
            {
                menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent += OnLevelPreviewChange;
                menuOnLevelInPreviewChangeSo.ChangeLevelInPreview(_menuUiLevelController.levelPropertiesSo[0], 0, true);
            }
            
            if (!_menuUiElementReference.CheckFinished)
            {
                Debug.LogWarning($"{name}: checkFinished is false, can't get reference to UI elements. So, " +
                                 $"disabling the {name}. (Tip: check if race-condition happen (Meaning if {name} begin" +
                                 $" first before {nameof(MenuUiElementReference)} finish it's Initialization(). " +
                                  "If problem is race-condition then proceed to use IReady interface if it suits to current situation.)");
                enabled = false;
                return;
            }

            if (!_menuUiElementReference.CheckResult)
            {
                Debug.LogWarning($"{name}: checkResult is false, can't get reference to UI elements. So, disabling the {name}.");
                enabled = false;
                return;
            }
            
            Initialization();
        }

        private void Initialization()
        {
            _menuUiElementReference.DragZoneReference.RegisterCallback<PointerDownEvent>(ClickingTheWheel, TrickleDown.TrickleDown);
            _menuUiElementReference.DragZoneReference.RegisterCallback<PointerUpEvent>(LeftTheWheelToHold);
            _menuUiElementReference.DragZoneReference.RegisterCallback<PointerMoveEvent>(MovingTheWheel);

            _menuUiElementReference.Root.RegisterCallback<GeometryChangedEvent>(UpdateSpaceBetweenMouseOnWindowWidthChange);
            
            //Chancing buttons position to absolute to be sure they begin from 0,0 and only change their position purely from C#
            StyleEnum<Position> absolutePositionLevelButtons = new StyleEnum<Position>(Position.Absolute); 
            for (int i = 0; _menuUiElementReference.LevelButtonsReferences.Length > i; i++)
            {
                _menuUiElementReference.LevelButtonsReferences[i].style.position = absolutePositionLevelButtons;
            }

            for (int i = 0; _menuUiElementReference.LevelButtonsReferences.Length > i; i++)
            {
                _buttonTranslateOnCursor = _menuUiElementReference.LevelButtonsReferences[i].style.translate.value;
                _buttonTranslateOnCursor.x = i * _spaceBetweenLevelButtons;
                _menuUiElementReference.LevelButtonsReferences[i].style.translate = _buttonTranslateOnCursor;
            }

        }

        private void OnDisable()
        {
            OnSpaceBetweenLevelChange -= ApplySpaceToBetweenOfLevelButtons;
            OnSpaceBetweenLevelChange -= UpdateAreaWidthForCurrentButtonInPreview;
        }

        //Use for make persistant the space between level buttons that match to half of the Root container (Which it's..
        //whole screen with 1.5% padding)
        private void UpdateSpaceBetweenMouseOnWindowWidthChange(GeometryChangedEvent evt)
        {
            //Updating space between buttons
            /*It's specifically chosen divide to 2, because this neighbor buttons stay at the half the outside of left/right screen borders,
              when button that in preview is locked to the center*/

            _spaceBetweenLevelButtons = (int)evt.newRect.width;
            
            //If screen ratio is high, it means user's screen quite bigger than 4:3, so we can fit neighbor buttons
            if (evt.newRect.width / evt.newRect.height > 1.5f)
            {
                _spaceBetweenLevelButtons /= 2;
            }
            
            OnSpaceBetweenLevelChange?.Invoke();
        }
        
        /*If player passed the area of a level button in preview, it means player drag quite far away the carousel so,
          change the _levelIndexInPreview to where the Carousel's translate.x value close to*/
        private void LevelInPreviewChanger()
        {
            //_levelIndexInPreview increase in negative axis so, if player try to change the opposite direction, we ignore
            if (_menuUiElementReference.CarouselReference.style.translate.value.x.value > 0) return;
            
            float cachedFilmReferenceXAbs =
                math.abs(_menuUiElementReference.CarouselReference.style.translate.value.x.value);
            
            //Check if player go outside from the area of level in preview by comprising to x value of Carousel
            if (!(cachedFilmReferenceXAbs < _areaWidthOfLevelIndexInPreview.x) &&
                !(cachedFilmReferenceXAbs > _areaWidthOfLevelIndexInPreview.y)) return;
            
            if (cachedFilmReferenceXAbs > _areaWidthOfLevelIndexInPreview.x)
            {
                if (_menuUiLevelController.levelPropertiesLength - 1 >= _levelIndexInPreview + 1)
                {
                    _levelIndexInPreview++;
                }
            }

            if (cachedFilmReferenceXAbs < _areaWidthOfLevelIndexInPreview.y)
            {
                if (_levelIndexInPreview > 0)
                {
                    _levelIndexInPreview--;
                }
            }

            //At the end we calculate new button area for the new button preview player change
            UpdateAreaWidthForCurrentButtonInPreview();
        }

        /*Calculating area of level button in preview.*/
        private void UpdateAreaWidthForCurrentButtonInPreview()
        {
            int currentPositionOfLevelInPreview = _levelIndexInPreview * _spaceBetweenLevelButtons;
            int halfOfSpaceBetweenLevelButtons = _spaceBetweenLevelButtons / 2;
            _areaWidthOfLevelIndexInPreview.x = currentPositionOfLevelInPreview - halfOfSpaceBetweenLevelButtons;
            _areaWidthOfLevelIndexInPreview.y = currentPositionOfLevelInPreview + halfOfSpaceBetweenLevelButtons;
        }

        //Making the Carousel's translate same to where _currentScrollX going which this change when player move the wheel
        private void UpdateWheelTranslatePosition()
        {
            _wheelTranslateOnCursor.x = _currentScrollX; //Since we're only moving wheel in x position, we don't update others (They default kept in 0)
            _menuUiElementReference.CarouselReference.style.translate = _wheelTranslateOnCursor;
        }

        /*When something happen cause _spaceBetweenLevelButtons value change (as window size change or Root container shrink/expand..)
          this function call to recalculate space between level buttons based on newly updated _spaceBetweenLevelButtons.*/
        private void ApplySpaceToBetweenOfLevelButtons()
        {
            for (int i = 0; _menuUiElementReference.LevelButtonsReferences.Length > i; i++)
            {
                _buttonTranslateOnCursor.x = i * _spaceBetweenLevelButtons;
                _menuUiElementReference.LevelButtonsReferences[i].style.translate = _buttonTranslateOnCursor;
            }
        }
        

        private void ClickingTheWheel(PointerDownEvent evt)
        {
            _holdingTheMouseOnWheel = true;
            _startPos = evt.position; 
            _hasMovedSignificantly = false; //Resetting the flag
            //Allows to move wheel even outside the wheel
            _menuUiElementReference.DragZoneReference.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void MovingTheWheel(PointerMoveEvent evt)
        {
            if (!_holdingTheMouseOnWheel) return;

            /*At the last we check distance of mouse's x position between the first time player clicked the screen and second time its drag.
              If distance is more than 10, meaning player drag significant distance and intend this as a drag on wheel for swap between levels
              and making _hasMovedSignificantly true to inform the LeftTheWheelToHold() this is not a click*/
            if (!_hasMovedSignificantly && math.abs(_startPos.x - evt.position.x) + math.abs(_startPos.y - evt.position.y) > 10f)
            {
                _hasMovedSignificantly = true;
            }

            if (_hasMovedSignificantly)
            {
                /*Modify cached Translate variable by adding current wheel translate.x and add evt.deltaPosition.x which shows how much the finger moved from the last time
                // Thankfully, deltaPosition return directly distance between current and last time so, we just add those both to each other.*/
                _targetScrollX += evt.deltaPosition.x;
            }
        }
        
        private void LeftTheWheelToHold(PointerUpEvent evt)
        {
            if (!_holdingTheMouseOnWheel) return;
            
            _menuUiElementReference.DragZoneReference.ReleasePointer(evt.pointerId);
            
            /*This boolean depends on how much player drag the wheel. If drag distance less than 10,
              we assume player tried to click on a level, so we send load level signal*/
            if (!_hasMovedSignificantly)
            {
                LoadLevelButton();
            }
            
            //Resetting values
            _holdingTheMouseOnWheel = false;
            _hasMovedSignificantly = false;
        }

        private void LoadLevelButton()
        {
            OnLoadLevelButtonClicked?.Invoke();
        }
        
        //Modify Scales of current level preview in focus and neighbor levels scale
        private void LevelIconScale()
        {
            //Limiting to O(3)
            for (int i = - 1; i <= 1; i++)
            {
                /*if _levelIndexInPreview is 0, meaning there is not level at the left (previous),
                we check the reverse where _levelIndexInPreview not the last level so it's not cause
                array overflow when checking the next (right) level*/
                if ((_levelIndexInPreview == 0 && i == -1) || (_levelIndexInPreview == _menuUiLevelController.levelPropertiesLength - 1 && i == 1)) continue;
                
                _distanceBetweenCenterFocusAndButton = math.abs(_menuUiElementReference.LevelButtonsReferences[_levelIndexInPreview + i].style.translate.value.x.value + _currentScrollX);
                _normalizedReverse = math.saturate(1f - _distanceBetweenCenterFocusAndButton / _spaceBetweenLevelButtons);
                
                //Applying value those cached (For prevent using new word and recreating in loop)
                _scaleValueForLevelButton.x = 1 + 0.25f * _normalizedReverse;
                _scaleValueForLevelButton.y = _scaleValueForLevelButton.x;
                _scaleUIForLevelButton = _scaleValueForLevelButton;
                _styleScaleForLevelButton = _scaleUIForLevelButton;
                
                //And then apply
                _menuUiElementReference.LevelButtonsReferences[_levelIndexInPreview + i].style.scale = _styleScaleForLevelButton;
            }
        }
        
        //Change level's rotate based on current level in preview
        private void LevelIconRotate()
        {
            if (!_holdingTheMouseOnWheel)
            {
                //Adding increment
                _currentAngle += 1f * Time.deltaTime;

                //Caching values
                _currentRotatingButtonAngle.value = _currentAngle;
                _currentRotatingButtonAngle.unit = AngleUnit.Degree;
                _currentRotatingButtonRotate = _currentRotatingButtonAngle;
                _currentRotatingButtonStyleRotate = _currentRotatingButtonRotate;
                
                //Applying increment to the button that in preview
                _currentRotatingButtonStyle.rotate = _currentRotatingButtonStyleRotate;
            }
        }

        private void OnLevelPreviewChange()
        {
            //Caching the style of current level in preview so, LevelIconRotate doesn't need to go lookup style in every frame
            _currentRotatingButtonStyle = _menuUiElementReference
                .LevelButtonsReferences[menuOnLevelInPreviewChangeSo.levelIndexInPreview].style;
            
            _currentAngle = _menuUiElementReference.LevelButtonsReferences[menuOnLevelInPreviewChangeSo.levelIndexInPreview].resolvedStyle.rotate.angle.value;
        }
        
        private void Update()
        {
            _distanceBetweenTargetAndCurrentScrollX = math.abs(_currentScrollX - _targetScrollX);
            
            /*We change _currentScrollX if distance between _currentScrollX and _targetScrollX meaningfully far apart to worth using
              the Mathf.SmoothDamp*/
            if (_distanceBetweenTargetAndCurrentScrollX > 2f)
            {
                _currentScrollX = Mathf.SmoothDamp(_currentScrollX, _targetScrollX, ref _scrollVelocity, SmoothTime);
            }
            
            /*If player drag fast the wheel and left to click on, the wheel go with velocity until it "stops" (_distanceBetweenTargetAndCurrentScrollX > 0.1f).
              With next statement, we're rechange the _targetScrollX that make the _levelIndexInPreview center.
              We check !_holdingTheMouseOnWheel since _distanceBetweenTargetAndCurrentScrollX > 0.1f can also be happens when player
              holding the wheel but didn't move its cursor despite it's still holding the wheel*/
            else if(!_holdingTheMouseOnWheel)
            {
                /*We multiply with negative since, levels keep increase its index at negative axis (e.g. if _spaceBetweenLevelButtons
                  is 400, then _levelIndexInPreview = 0's x position will -400, _levelIndexInPreview = 1's x position will -800* and etc.)*/
                _targetScrollX = -(_levelIndexInPreview * _spaceBetweenLevelButtons);
                
                //If player isn't moving the carousel and Update() now executing to focus the carousel to the selected level preview, we invoke the event
                menuOnLevelInPreviewChangeSo.ChangeLevelInPreview(_menuUiLevelController.levelPropertiesSo[_levelIndexInPreview], _levelIndexInPreview);
            }
            
            UpdateWheelTranslatePosition();
            
            LevelIconScale();
            
            if (_distanceBetweenTargetAndCurrentScrollX > 2f)
            {
                LevelInPreviewChanger();
            }

            LevelIconRotate();
        }
        

        private void OnDestroy()
        {
            _menuUiElementReference.DragZoneReference.UnregisterCallback<PointerDownEvent>(ClickingTheWheel);
            _menuUiElementReference.DragZoneReference.UnregisterCallback<PointerUpEvent>(LeftTheWheelToHold);
            _menuUiElementReference.DragZoneReference.UnregisterCallback<PointerMoveEvent>(MovingTheWheel);
            _menuUiElementReference.Root.UnregisterCallback<GeometryChangedEvent>(UpdateSpaceBetweenMouseOnWindowWidthChange);
            
            menuOnLevelInPreviewChangeSo.LevelPreviewChangeEvent -= OnLevelPreviewChange;
        }
    }
}