﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchConfigType : int { Follow = 0, FixedJoystick = 1, DynamicJoystick = 2 }

public class Movement : MonoBehaviour {

    [Header("Speed")]
    [SerializeField] float initialMoveSpeed = 6f;
    [SerializeField] float currentMoveSpeed = 6f;

    [Header("Touch Joystick")]
    [SerializeField] GameObject joystick;
    [SerializeField] GameObject innerCircle;
    [SerializeField] GameObject outerCircle;


    public static string PLAYER_MOVEMENT_KEY = "playerMove";

    // Touch
    bool touching = false;
    bool inJoystick = false; // for Fixed Joystick only
    Vector2 pointA;
    Vector2 pointB;
    SpriteRenderer outerCircleSR;
    SpriteRenderer innerCircleSR;

    // Player 
    TouchConfigType touchConfig;
    Player player;
    Vector3 respawnPos;
    float xPadding = 0.5f;
    float yPadding = 0.5f;

    // General
    public static bool pauseCheck = true;
    Camera gameCamera;
    float fixedDeltaTime;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    public Vector3 RespawnPos { get => respawnPos; set => respawnPos = value; }
    public TouchConfigType TouchConfig { get => touchConfig; set => touchConfig = value; }

    private void Start() {
        if (PlayerPrefs.HasKey(PLAYER_MOVEMENT_KEY)) {
            TouchConfig = (TouchConfigType)PlayerPrefs.GetInt(PLAYER_MOVEMENT_KEY);
        } else {
            TouchConfig = TouchConfigType.Follow;
        }
        player = GetComponent<Player>();
        SetUpMoveBoundaries();
#if UNITY_ANDROID || UNITY_IOS
        ResetMovementMode();
#endif
    }

    private void Update() {
        GetInput();
    }

    private void FixedUpdate() {
        fixedDeltaTime = Time.fixedDeltaTime;
#if UNITY_ANDROID || UNITY_IOS
        MoveWithTouch();
#else
        MoveWithoutTouch();
#endif
    }

    private void SetUpMoveBoundaries() {
        gameCamera = Camera.main;
        Renderer renderer = GetComponent<Renderer>();

        Vector3 bottomLeftWorldCoordinates = gameCamera.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRightWorldCoordinates = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Vector3 movementRangeMin = bottomLeftWorldCoordinates + renderer.bounds.extents;
        Vector3 movementRangeMax = topRightWorldCoordinates - renderer.bounds.extents;

        RespawnPos = gameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, 0));
        xMin = movementRangeMin.x + xPadding;
        xMax = movementRangeMax.x - xPadding;
        yMin = movementRangeMin.y + yPadding;
        yMax = movementRangeMax.y - yPadding * 4;
    }

    private void ResizeAndReposJoystick() {
        if (TouchConfig == TouchConfigType.FixedJoystick) {
            Renderer renderer = outerCircle.GetComponent<Renderer>();
            Vector3 bottomRightWorldCoordinates = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
            Vector3 extents = renderer.bounds.extents * 1.5f;
            Vector2 joystickPos = new Vector2(bottomRightWorldCoordinates.x - extents.x, bottomRightWorldCoordinates.y + extents.y);
            outerCircle.transform.position = joystickPos;
            innerCircle.transform.position = joystickPos;
            pointB = joystickPos;
        }
        outerCircleSR = outerCircle.GetComponent<SpriteRenderer>();
        innerCircleSR = innerCircle.GetComponent<SpriteRenderer>();
    }

    private void GetInput() {
        if (pauseCheck && Input.touchCount > 0) {
            touching = true;
            Touch touch = Input.GetTouch(0);
            switch (TouchConfig) {
                case TouchConfigType.Follow:
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {
                        pointA = gameCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                    }
                    break;
                case TouchConfigType.FixedJoystick:
                    if (touch.phase == TouchPhase.Began) {
                        pointA = gameCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                        if (IsPointInJoystick(pointA)) {
                            inJoystick = true;
                        } else {
                            inJoystick = false;
                        }
                    }
                    if (inJoystick && touch.phase == TouchPhase.Moved) {
                        pointA = gameCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                    }
                    break;
                case TouchConfigType.DynamicJoystick:
                    if (touch.phase == TouchPhase.Began) {
                        pointA = gameCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                    } else if (touch.phase == TouchPhase.Moved) {
                        pointB = gameCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                    }
                    break;
            }
        } else {
            touching = false;
        }
    }

    private bool IsPointInJoystick(Vector2 point) {
        var distanceSquared = (point.x - pointB.x) * (point.x - pointB.x) + (point.y - pointB.y) * (point.y - pointB.y);
        float radius = outerCircle.GetComponent<Renderer>().bounds.extents.magnitude;
        return distanceSquared <= radius * radius;
    }

    private void MoveWithTouch() {
        if (touching && (TouchConfig != TouchConfigType.FixedJoystick || inJoystick)) {
            Vector2 offset;
            if (TouchConfig == TouchConfigType.Follow) {
                offset = new Vector2(player.transform.position.x, player.transform.position.y) - pointA;
                offset *= -1;
            } else {
                offset = pointB - pointA;
                if (TouchConfig == TouchConfigType.DynamicJoystick) {
                    if (!outerCircleSR.enabled) {
                        outerCircleSR.enabled = true;
                    }
                    if (!innerCircleSR.enabled) {
                        innerCircleSR.enabled = true;
                    }
                } else {
                    offset *= -1;
                }
            }
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
            var deltaX = direction.x * currentMoveSpeed * fixedDeltaTime;
            var deltaY = direction.y * currentMoveSpeed * fixedDeltaTime;
            var newXPos = Mathf.Clamp(player.transform.position.x + deltaX, xMin, xMax);
            var newYPos = Mathf.Clamp(player.transform.position.y + deltaY, yMin, yMax);
            if (TouchConfig == TouchConfigType.DynamicJoystick) {
                outerCircle.transform.position = pointA;
            }
            innerCircle.transform.position = new Vector2(outerCircle.transform.position.x + direction.x, outerCircle.transform.position.y + direction.y);
            player.transform.position = new Vector3(newXPos, newYPos, 0f);
        } else {
            innerCircle.transform.position = new Vector2(outerCircle.transform.position.x, outerCircle.transform.position.y);
            if (TouchConfig == TouchConfigType.DynamicJoystick) {
                outerCircleSR.enabled = false;
                innerCircleSR.enabled = false;
            }
        }
    }

    private void MoveWithoutTouch() {
        var deltaX = Input.GetAxis("Horizontal") * currentMoveSpeed * fixedDeltaTime;
        var deltaY = Input.GetAxis("Vertical") * currentMoveSpeed * fixedDeltaTime;
        var newXPos = Mathf.Clamp(player.transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(player.transform.position.y + deltaY, yMin, yMax);
        player.transform.position = new Vector2(newXPos, newYPos);
    }

    public void SetMovementModePref(int mode) {
        TouchConfig = (TouchConfigType)mode;
        PlayerPrefs.SetInt(PLAYER_MOVEMENT_KEY, mode);
        PlayerPrefs.Save();
        ResetMovementMode();
    }

    private void ResetMovementMode() {
        if (TouchConfig == TouchConfigType.FixedJoystick || TouchConfig == TouchConfigType.DynamicJoystick) {
            currentMoveSpeed = initialMoveSpeed;
            joystick.SetActive(true);
            ResizeAndReposJoystick();
        } else {
            currentMoveSpeed = initialMoveSpeed * 1.15f;
            joystick.SetActive(false);
        }
    }
}