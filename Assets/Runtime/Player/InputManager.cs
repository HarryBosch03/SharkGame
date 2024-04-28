using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    public class InputManager : MonoBehaviour
    {
        public Gamepad gamepad;
        public Keyboard keyboard;
        public Mouse mouse;
        
        public void BindToDevice(Gamepad gamepad)
        {
            this.gamepad = gamepad;
            keyboard = null;
            mouse = null;
        }
        
        public void BindToDevice(Keyboard keyboard, Mouse mouse)
        {
            gamepad = null;
            this.keyboard = keyboard;
            this.mouse = mouse;
        }
    }
}