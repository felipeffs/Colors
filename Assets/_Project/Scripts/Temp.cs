using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Temp : MonoBehaviour
{
    [SerializeField] private SceneField firstLevel;

    void Start()
    {
        if (SceneManager.sceneCount == 1)
        {
            AsyncOperation loading = SceneManager.LoadSceneAsync(firstLevel, LoadSceneMode.Additive);
            loading.completed += (op) =>
            {
                var scene = SceneManager.GetSceneByName(firstLevel);

                SceneManager.SetActiveScene(scene);
            };
        }

        Destroy(gameObject);
    }
}
