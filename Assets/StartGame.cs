using UnityEngine;

public class MenuManagerr : MonoBehaviour
{
    [Header("Меню")]
    public GameObject menuBackground;
    public GameObject playButton;
    public GameObject upgradeButton;
    public GameObject exitButton;

    [Header("Игровой режим")]
    public GameObject gameUI;
    public GameObject player;

    public void StartGame()
    {
        // Выключаем меню
        menuBackground.SetActive(false);
        playButton.SetActive(false);
        upgradeButton.SetActive(false);
        exitButton.SetActive(false);

        // Включаем игру
        gameUI.SetActive(true);
        player.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}