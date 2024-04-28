using Runtime.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject aiPrefab;
    public InputAction spawnAction;
    
    private void OnEnable()
    {
        spawnAction.Enable();
        spawnAction.performed += SpawnEvent;
    }

    private void OnDisable()
    {
        spawnAction.performed -= SpawnEvent;
        spawnAction.Disable();
    }
    
    private void OnDestroy()
    {
        spawnAction.Dispose();
    }
    
    private void SpawnEvent(InputAction.CallbackContext ctx)
    {
        switch (ctx.control.device)
        {
            case Keyboard keyboard:
                SpawnPlayer(keyboard, Mouse.current);
                break;
            case Mouse mouse:
                SpawnPlayer(Keyboard.current, mouse);
                break;
            case Gamepad gamepad:
                SpawnPlayer(gamepad);
                break;
        }
    }

    private GameObject SpawnPlayer(GameObject prefab)
    {
        var instance = Instantiate(prefab);
        return instance;
    }
    
    private void SpawnPlayer(Keyboard keyboard, Mouse mouse)
    {
        var player = SpawnPlayer(playerPrefab);
        player.GetComponent<InputManager>().BindToDevice(keyboard, mouse);
    }

    private void SpawnPlayer(Gamepad gamepad)
    {
        var player = SpawnPlayer(playerPrefab);
        player.GetComponent<InputManager>().BindToDevice(gamepad);
    }

    public void SpawnDummyPlayer()
    {
        SpawnPlayer(aiPrefab);
    }
}
