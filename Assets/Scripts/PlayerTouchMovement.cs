using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject Glider;

    [SerializeField]
    private Vector2 JoystickSize = new Vector2 (100, 100);

    [SerializeField]
    private FloatingJoystick Joystick;

    [SerializeField]
    private float speed = 3.5f;

    private Finger MovementFinger;
    private Vector2 MovementAmount;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerMove(Finger MovedFinger)
    {
        if (MovedFinger == MovementFinger) 
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            if (Vector2.Distance(
                currentTouch.screenPosition,
                Joystick.RectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPosition = (
                    currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                    ).normalized * maxMovement;
            }
            else
            { 
                knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }
    }

    private void HandleFingerUp(Finger LostFinger)
    {
        if (LostFinger == MovementFinger) 
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
        }
    }

    private void HandleFingerDown(Finger TouchedFinger)
    {
        if (MovementFinger == null) { // Maybe add an "Touch Space" to check here
            MovementFinger = TouchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(true);
            Joystick.RectTransform.sizeDelta = JoystickSize;
            Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
        }
    }

    // Makes the Joystick DON'T appears outside the screen
    private Vector2 ClampStartPosition(Vector2 StartPosition)
    {
        if (StartPosition.x < JoystickSize.x / 2)
        {
            StartPosition.x = JoystickSize.x / 2;
        }
        if (StartPosition.y < JoystickSize.y / 2)
        {
            StartPosition.y = JoystickSize.y / 2;
        }
        else if(StartPosition.y > Screen.height - JoystickSize.y / 2) 
        {
            StartPosition.y = Screen.height - JoystickSize.y / 2;
        }

        return StartPosition;
    }

    private void Update()
    {
        Vector3 scaledMovement = speed * Time.deltaTime * new Vector3(MovementAmount.x, 0, MovementAmount.y);
        Fly(MovementAmount);
        //transform.LookAt(transform.position + scaledMovement, Vector3.up);
        transform.Translate(scaledMovement);

        CheckCollissionWithGround();

        //Keep the camera with the player (and smooth)
        Vector3 moveCamTo = transform.position - transform.forward * 10.0f + Vector3.up * 5.0f;
        Camera.main.transform.position = moveCamTo;
        Camera.main.transform.LookAt(transform.position);
    }

    private void CheckCollissionWithGround()
    {
        float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
        if(terrainHeight > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x,
                                             terrainHeight,
                                             transform.position.z);
        }
    }

    private void Fly(Vector2 Rotate) {
        bool rotating = true;
        //transform.Rotate(0, 0, Rotate.y);
        if(rotating) Glider.transform.Rotate(0, 0, -0.1f * Rotate.x);

        //print(Glider.transform.rotation.z);

        // Hovering LEFT
        if (Glider.transform.rotation.z < -0.2f) Glider.transform.rotation = Quaternion.Euler(0, 0, -23f);
        // Hovering RIGHT
        if (Glider.transform.rotation.z > 0.2f) Glider.transform.rotation = Quaternion.Euler(0, 0, 23f);
    }
}
