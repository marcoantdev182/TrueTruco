#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrucoDeMesaOnline.EditorTools
{
    public static class TrucoSceneCreator
    {
        private const string ScenePath = "Assets/Scenes/TrucoTableLocal.unity";

        [MenuItem("Truco de Mesa/Create Local MVP Scene")]
        public static void CreateLocalMvpScene()
        {
            Directory.CreateDirectory("Assets/Scenes");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject root = new GameObject("Local Offline Scene Root");
            root.AddComponent<LocalOfflineSceneRoot>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = root;
            Debug.Log("Created local MVP scene at " + ScenePath);
        }
    }
}
#endif

