using System.Collections;
using UnityEngine;

namespace Player
{ 
    /// <summary>
    /// Trail comes right behind of player
    /// </summary>
    public class Line : MonoBehaviour
    {
        //CloneCube section for line effect
        [Header("Pool Settings")]
        [Tooltip("The prefab used for the trail segments.")]
        [SerializeField] private GameObject cloneCube;
        private GameObject[] _clonedCubes;
        [SerializeField] private int clonedCubesCount;
        [SerializeField] private Transform parentCubeClone;
        private int _currentTransformChangedCubeClone;
        private MaterialPropertyBlock _propBlock;
        
        //Cubes will change their position to on
        [SerializeField] private Transform goalPosition;
        
        [SerializeField] private float updateInterval;
        private WaitForSeconds _waitForSecondsCloneCube;

        private bool _playerOnGround = true;

        private Coroutine _cloneCubesCoroutine;

        private void OnEnable()
        {
            // This function prevent Zig-Zag, cropped line when pressed to change direction.
            // It's directly put cube where player press
            Movement.PlayerPressed += ChangeNextCloneCubePositionOnGoalPosition;

            //Stop Line drawing when player not on the ground (e.g. On air, Dead etc.)
            GroundStateChecker.OnGroundChange += OnGroundStateChange;
        }

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
            
            _waitForSecondsCloneCube = new WaitForSeconds(updateInterval);
            InitializeCloneCubes();
        }

        private void Start()
        {
            StartTransformCloneCubes();
        }

        private void OnDisable()
        {
            Movement.PlayerPressed -= ChangeNextCloneCubePositionOnGoalPosition;
            GroundStateChecker.OnGroundChange -= OnGroundStateChange;
        }

        private void InitializeCloneCubes()
        {
            //Clone cubes create by amount of clonedCubesCount
            _clonedCubes = new GameObject[clonedCubesCount];
            for (int i = 0; i < _clonedCubes.Length; i++)
            {
                _clonedCubes[i] = Instantiate(cloneCube, parentCubeClone, goalPosition);
                
                //Breaking SRP batch by overriding with an empty Property Block.
                _clonedCubes[i].GetComponent<Renderer>().SetPropertyBlock(_propBlock);
            }
        }

        /// <summary>
        /// Update _clonedCubes position based on <see cref="goalPosition"/> at <see cref="_waitForSecondsCloneCube"/> seconds
        /// </summary>
        IEnumerator TransformCloneCubes()
        {
            while (true)
            {
                ChangeNextCloneCubePositionOnGoalPosition();
                yield return _waitForSecondsCloneCube;
            }
        }

        private void StartTransformCloneCubes()
        {
            _cloneCubesCoroutine = StartCoroutine(TransformCloneCubes());
        }

        private void StopTransformCloneCubes()
        {
            if (_cloneCubesCoroutine != null)
            {
                StopCoroutine(_cloneCubesCoroutine);
            }
        }
        
        private void ChangeNextCloneCubePositionOnGoalPosition()
        {
            if (_playerOnGround)
            {
                _clonedCubes[_currentTransformChangedCubeClone].transform.position = goalPosition.position;
                _currentTransformChangedCubeClone++;
                if (_currentTransformChangedCubeClone == _clonedCubes.Length)
                {
                    _currentTransformChangedCubeClone = 0;
                }  
            }
        }

        private void OnGroundStateChange(bool groundState)
        {
            _playerOnGround = groundState;
        }
    }
}