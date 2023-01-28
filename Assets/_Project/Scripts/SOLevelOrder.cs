using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "new Level Order", menuName = "Level Order", order = 0)]
public class SOLevelOrder : ScriptableObject
{
    [SerializeField] private List<SceneField> levelsList;
    [SerializeField] private SceneField menuScene;
    [SerializeField] private SceneField essentialScene;

    public void LoadLevelByIndex(int orderIndex)
    {
        SceneManager.LoadSceneAsync(essentialScene, LoadSceneMode.Single);
        AsyncOperation loading = SceneManager.LoadSceneAsync(levelsList[orderIndex], LoadSceneMode.Additive);

        // set the level as active scene
        loading.completed += (op) =>
                {
                    var scene = SceneManager.GetSceneByName(levelsList[orderIndex]);

                    SceneManager.SetActiveScene(scene);
                };
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Single);
    }

    public void LoadNextLevel()
    {
        var currentLevelOrderPosition = GetCurrentLevelOrderPosition(out Scene[] openScenes);
        var nextLevelOrderPosition = currentLevelOrderPosition + 1;

        // Unload level scenes
        foreach (var scene in openScenes)
        {
            SceneManager.UnloadSceneAsync(scene);
        }

        // Load next level scene
        string sceneToLoad;
        LoadSceneMode sceneLoadMethod;
        if (nextLevelOrderPosition >= levelsList.Count || nextLevelOrderPosition <= -1)
        {
            sceneToLoad = menuScene;
            sceneLoadMethod = LoadSceneMode.Single;
        }
        else
        {
            sceneLoadMethod = LoadSceneMode.Additive;
            sceneToLoad = levelsList[nextLevelOrderPosition];
        }

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad, sceneLoadMethod);

        // set the level as active scene
        loading.completed += (op) =>
        {
            var scene = SceneManager.GetSceneByName(sceneToLoad);

            SceneManager.SetActiveScene(scene);
        };
    }

    private int GetCurrentLevelOrderPosition(out Scene[] openScenes)
    {
        openScenes = (from Scene scene in GetAllOpenScenes()
                      where scene.name != essentialScene.SceneName
                      select scene).ToArray(); ;

        Scene currentScene = openScenes.First();

        for (int i = 0; i < levelsList.Count; i++)
        {
            var sceneOnList = SceneManager.GetSceneByName(levelsList[i].SceneName).buildIndex;
            if (sceneOnList == currentScene.buildIndex)
                return i;
        }
        return -1;
    }

    private Scene[] GetAllOpenScenes()
    {
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }

        return loadedScenes;
    }
}
