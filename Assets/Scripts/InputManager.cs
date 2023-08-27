using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float _swipeMultiplier = 2f;

    private Vector3 _firstMousePosition;
    private Vector3 _currentMousePosition;
    private Vector3 _preMousePosition;
    private Vector3 _deltaMousePosition;

    public Vector3 DeltaMousePosition { get { return _deltaMousePosition; } }


    private float _screenWidth;

    private void Awake()
    {
        _screenWidth = Screen.width;
    }

    void Start()
    {

    }

    void Update()
    {
        SwipeInputCalculate();
    }

    private void SwipeInputCalculate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstMousePosition = Input.mousePosition;
            _currentMousePosition = _firstMousePosition;
            _preMousePosition = _currentMousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _preMousePosition = _currentMousePosition;
            _currentMousePosition = Input.mousePosition;

            _deltaMousePosition = ((_currentMousePosition - _preMousePosition) / _screenWidth) * _swipeMultiplier;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _preMousePosition = Vector3.zero;
            _currentMousePosition = Vector3.zero;
            _firstMousePosition = Vector3.zero;
            _deltaMousePosition = Vector3.zero;
        }
    }
}
