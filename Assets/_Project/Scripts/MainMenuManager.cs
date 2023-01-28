using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SOLevelOrder levelOrder;

    public void CloseGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        levelOrder.LoadLevelByIndex(0);
    }
}
