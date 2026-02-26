using UnityEngine;

public class BaseWindow : MonoBehaviour
{
    public virtual void Close()
    {
        WindowManager.CloseWindow(this);
    }
}
