using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class OpenSampleSceneShortcut
{
    private const string NGUIScenePath = "Assets/Scenes/NGUIScene.unity";

    [MenuItem("Tools/Open NGUI Scene %#e")]
    private static void OpenNGUIScene()
    {
        OpenSceneWithPrompt(NGUIScenePath);
    }

    [MenuItem("Tools/Open City Scene %#q")]
    private static void OpenNGUISceneAndPlay()
    {
        OpenSceneWithPrompt(NGUIScenePath, playAfterOpen: true);
    }

    private static void OpenSceneWithPrompt(string scenePath, bool playAfterOpen = false)
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        if (!File.Exists(scenePath))
        {
            EditorUtility.DisplayDialog(
                "Scene Not Found",
                $"Could not find scene at path:\n{scenePath}",
                "OK");
            return;
        }

        EditorSceneManager.OpenScene(scenePath);

        if (playAfterOpen)
        {
            EditorApplication.isPlaying = true;
        }
    }
}
