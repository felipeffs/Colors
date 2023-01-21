using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "new Level Order", menuName = "Level Order", order = 0)]
public class SOLevelOrder : ScriptableObject
{
    [SerializeField] private List<SceneField> levelsList;
    [SerializeField] private SceneField menuScene;

    public void LoadNextLevel()
    {
        var nextLevelIndex = GetCurrentSceneIndex() + 1;

        if (nextLevelIndex >= levelsList.Count || nextLevelIndex <= -1)
        {
            SceneManager.LoadScene(menuScene);
        }
        else
        {
            SceneManager.LoadScene(levelsList[nextLevelIndex]);
        }
    }

    private int GetCurrentSceneIndex()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelsList.Count; i++)
        {
            if (levelsList[i].SceneName == sceneName)
            {
                return i;
            }
        }

        return -1;
    }
}
