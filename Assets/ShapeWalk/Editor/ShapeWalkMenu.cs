using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SA.Demo.Editor
{
    public static class ShapeWalkMenu
    {
        [MenuItem("Tools/SA/Shape Workshop/Open Scene")]
        public static void Open()
        {
            if (EditorApplication.isPlaying) { Debug.LogWarning("SA: stop Play before opening the shape scene."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene("Assets/Scenes/ShapeWalk.unity", OpenSceneMode.Single);
            Frame();
            Debug.Log("SA: Shape workshop opened. Press Play to change proportions and switch clips.");
        }

        [MenuItem("Tools/SA/Shape Workshop/Frame Game View")]
        public static void Frame()
        {
            PlayModeWindow.SetCustomRenderingResolution(1280, 720, "Shape workshop");
            EditorApplication.ExecuteMenuItem("Window/General/Game");
            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
                if (window.GetType().FullName == "UnityEditor.GameView")
                {
                    window.maximized = true;
                    window.Focus();
                    window.Repaint();
                    break;
                }
        }
    }
}
