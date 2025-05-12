using UnityEngine;
using UnityEngine.UI; // Не забудьте добавить для работы с UI

public class MenuManagerr : MonoBehaviour
{
    [Header("Меню")]
    public GameObject menuBackground;
    public GameObject playButton;
    public GameObject upgradeButton;
    public GameObject exitButton;
    public GameObject player; 

    [Header("Игровой режим")]
    public GameObject gameUI;
    public Slider healthSlider; // Перетащите сюда ваш HP-бар
    
    [Header("Враг")]
    public GameObject enemy;
    public float enemySpawnDelay = 2f;

    public void StartGame()
    {
        
        // Выключаем меню
        menuBackground.SetActive(false);
        playButton.SetActive(false);
        upgradeButton.SetActive(false);
        exitButton.SetActive(false);

        // Включаем игровые элементы
        gameUI.SetActive(true);
        player.SetActive(true);
        
        // Инициализируем HP-бар
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                healthSlider.maxValue = playerHealth.maxHealth;
                healthSlider.value = playerHealth.maxHealth;
                playerHealth.ResetPlayer();
            }
            
        }
        

        // Активируем врага
        Invoke("ActivateEnemy", enemySpawnDelay);
    }

    void ActivateEnemy()
    {
        if (enemy != null)
        {
            enemy.SetActive(true);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enabled = true;
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}