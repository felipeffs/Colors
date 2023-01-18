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
        var nextLevelIndex = GetCurrentScene() + 1;

        if (nextLevelIndex >= levelsList.Count)
        {
            SceneManager.LoadScene(menuScene);
        }
        else
        {
            SceneManager.LoadScene(levelsList[nextLevelIndex]);
        }
    }

    private int GetCurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
