using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WindowManagerHotkeyTest : MonoBehaviour
{
    private const string TestWindowName = "Test Window";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureRunnerExists()
    {
        if (FindObjectOfType<WindowManagerHotkeyTest>() != null)
        {
            return;
        }

        GameObject runner = new GameObject("[WindowManagerHotkeyTest]");
        DontDestroyOnLoad(runner);
        runner.AddComponent<WindowManagerHotkeyTest>();
    }

    private void Update()
    {
        if (IsSKeyPressedThisFrame())
        {
            WindowManager.OpenWindow(TestWindowName);
        }
    }

    private static bool IsSKeyPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.sKey.wasPressedThisFrame)
        {
            return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.S);
#else
        return false;
#endif
    }
}
