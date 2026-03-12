using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class KeyDownTest : MonoBehaviour
{
    [SerializeField] private string message = "Space key pressed";

    private void Update()
    {
        if (IsSpacePressedThisFrame())
        {
            //WindowManager.OpenWindow("TestWindow");
        }
        else
        {
            if (IsSPressedThisFrame())
            {
                WindowManager.CloseTopWindow();
            }
        }
    }

    private static bool IsSPressedThisFrame()
    {
        return Input.GetKeyDown(KeyCode.S);
    }
    

    private static bool IsSpacePressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Space);
#else
        return false;
#endif
    }
}
