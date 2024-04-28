using System;
using System.Collections.Generic;
using Runtime.Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SharkController))]
public class SharkInputManager : InputManager
{
    public Color[] colors;
    
    private SharkController shark;
    private SharkVisuals visuals;
    private Camera mainCam;

    private SharkController.InputData inputData;
    public int id { get; private set; } = -1;

    public static readonly List<SharkInputManager> All = new();
    
    private void Awake()
    {
        mainCam = Camera.main;
        shark = GetComponent<SharkController>();
        visuals = GetComponent<SharkVisuals>();
    }

    private void OnEnable()
    {
        id = All.Count;
        All.Add(this);
    }

    private void OnDisable()
    {
        id = -1;
        All.Remove(this);
    }

    private void Start()
    {
        if (colors.Length > 0) visuals.SetColor(colors[id % colors.Length]);

        if (keyboard == null && mouse == null && gamepad == null)
        {
            Debug.LogWarning($"Destroying \"{name}\" because it has no controller bound");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (keyboard != null && mouse != null) DoInputKMaM();
        else if (gamepad != null) DoInputGamepad();
    }

    private void DoInputKMaM()
    {
        var mousePositionScreen = mouse.position.ReadValue();
        var mousePositionWorld = (Vector2)mainCam.ScreenToWorldPoint(mousePositionScreen);
        inputData.goalPosition = mousePositionWorld;
        inputData.moving = mouse.leftButton.isPressed;
        inputData.fast = mouse.rightButton.isPressed;
    }

    private void DoInputGamepad()
    {
        var stick = gamepad.leftStick.ReadValue();
        inputData.goalPosition = shark.body.position + stick * 5f;
        inputData.fast = gamepad.rightTrigger.isPressed;
        inputData.moving = stick.magnitude > float.Epsilon;
    }

    private void FixedUpdate()
    {
        shark.input = inputData;
        inputData = default;
    }
}
