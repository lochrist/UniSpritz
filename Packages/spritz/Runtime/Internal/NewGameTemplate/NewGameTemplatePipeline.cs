using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

namespace UniMini
{
    public class NewGameTemplatePipeline : ISceneTemplatePipeline
    {
        public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
        {
            return true;
        }

        public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
        {

        }

        public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                var searchQueryPath = EditorUtility.SaveFilePanel("Save search query...", "Assets", "NewSpritzGame", "unity");
                if (string.IsNullOrEmpty(searchQueryPath))
                    return;

                sceneName = FileUtil.GetProjectRelativePath(searchQueryPath.Replace("\\", "/"));
            }

            // Save scene on disk
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, sceneName);

            // Modify and save game script next to the scene
            var sceneBaseName = Path.GetFileNameWithoutExtension(sceneName);
            var sceneFolder = Path.GetDirectoryName(sceneName).Replace("\\", "/");
            var gameTemplateText = File.ReadAllText("Assets/Scripts/Internal/NewGameTemplate/NewGameTemplate.cs.template");
            gameTemplateText = gameTemplateText.Replace("public class TO_REPLACE_GAME_NAME", $"public class {sceneBaseName}");
            var gameScriptPath = Path.Join(sceneFolder, sceneBaseName + ".cs");
            File.WriteAllText(gameScriptPath, gameTemplateText);
            AssetDatabase.ImportAsset(gameScriptPath, ImportAssetOptions.ForceSynchronousImport);

            var gameRoot = FindGameRoot();
            if (gameRoot == null)
                return;
            if (gameRoot.tag != "Untagged")
            {
                gameRoot.tag = "Untagged";
            }

            SessionState.SetString("TryToAttachGameScript", scene.name);
        }

        [InitializeOnLoadMethod]
        static void TryToAttachGameScriptAfterDomainReload()
        {
            // TODO: not the most elegant way but afterDomainReload doesn't work on a static method in the template... might need to be attached to a real GameObject?
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            var sceneName = SessionState.GetString("TryToAttachGameScript", "");
            SessionState.SetString("TryToAttachGameScript", "");
            if (sceneName != currentScene.name)
                return;
            TryToAttachGameScript();
        }

        [MenuItem("Test/Attach Game")]
        static void TryToAttachGameScript()
        {
            var gameRoot = FindGameRoot();
            if (gameRoot == null || gameRoot.tag != "Untagged")
                return;

            Debug.Log($"Unbound Game found: {gameRoot.name}");
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            var sceneBaseName = Path.GetFileNameWithoutExtension(currentScene.path);
            var sceneFolder = Path.GetDirectoryName(currentScene.path).Replace("\\", "/");
            var gameScriptPath = Path.Join(sceneFolder, sceneBaseName + ".cs").Replace("\\", "/");
            var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(gameScriptPath);
            if (scriptAsset == null)
            {
                Debug.Log($"Cannot find game script class to attach: {gameScriptPath}");
            }
            var gameType = scriptAsset.GetClass();
            if (gameType == null)
            {
                Debug.Log($"Game type is not valid in {gameScriptPath}");
                return;
            }
            if (gameRoot.GetComponent(gameType) == null)
            {
                Debug.Log($"Binding Game : {gameType.Name}");
                gameRoot.AddComponent(gameType);
                gameRoot.tag = "GameController";
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currentScene);
            }
        }

        static GameObject FindGameRoot()
        {
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(currentScene.path) || string.IsNullOrEmpty(currentScene.name))
                return null;
            return currentScene.GetRootGameObjects().FirstOrDefault(go => go.name == "Game");
        }
    }
}