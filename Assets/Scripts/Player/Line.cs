using System;
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
        
        //Cubes will change their position to on
        [SerializeField] private Transform goalPosition;
        
        [SerializeField] private float updateInterval;
        private WaitForSeconds _waitForSecondsCloneCube;

        private Coroutine _cloneCubesCoroutine;

        private void OnEnable()
        {
            // This function prevent Zig-Zag, cropped line when pressed to change direction.
            // It's directly put cube where player press
            Movement.PlayerPressed += ChangeNextCloneCubePositionOnGoalPosition;
        }

        private void Awake()
        {
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
        }

        private void InitializeCloneCubes()
        {
            //Clone cubes create by amount of clonedCubesCount
            _clonedCubes = new GameObject[clonedCubesCount];
            for (int i = 0; i < _clonedCubes.Length; i++)
            {
                _clonedCubes[i] = Instantiate(cloneCube, parentCubeClone, goalPosition);
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
            _clonedCubes[_currentTransformChangedCubeClone].transform.position = goalPosition.position;
            _currentTransformChangedCubeClone++;
            if (_currentTransformChangedCubeClone == _clonedCubes.Length)
            {
                _currentTransformChangedCubeClone = 0;
            }
        }
    }
}