using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SOLevelOrder levelOrder;
    [SerializeField] private ButtonPlus settingsInitialButton;

    public void CloseGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        levelOrder.LoadLevelByIndex(0);
    }

    public void SettingsMenu()
    {
        gameObject.SetActive(false);
        UIPlusModule.Instance.ChangeInitialSelectedButton(settingsInitialButton);
        UIPlusModule.Instance.SetSelected(settingsInitialButton);
    }
}
