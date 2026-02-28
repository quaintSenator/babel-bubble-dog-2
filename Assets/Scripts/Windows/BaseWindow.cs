using UnityEngine;

public class BaseWindow : MonoBehaviour
{
    private string windowName = "";
    public virtual void Close()
    {
        WindowManager.CloseWindow(this);
    }

    public virtual void Open(GameObject wakerObject = null)
    {
        
    }

    /// <summary>
    /// open时 windowManager帮助记一下这个window对应的windowName
    /// </summary>
    /// <param name="name"></param>
    public void ConfigWindowName(string name)
    {
        windowName = name;
    }

    public string GetWindowName()
    {
        return windowName;
    }
}
