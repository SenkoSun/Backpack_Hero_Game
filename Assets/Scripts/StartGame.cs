using UnityEngine;
using UnityEngine.UI; // Не забудьте добавить для работы с UI
using TMPro; // Добавляем для работы с TextMeshPro

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

    [Header("Кнопка Битвы")]
    public Button battleToggleButton; // Перетащите сюда кнопку Старт/Пауза
    public TMP_Text battleButtonText; // Перетащите сюда текстовый компонент кнопки

    private bool isPaused = true; // Изначально битва на паузе

    public void StartGame()
    {
        // Убедитесь, что кнопка битвы и ее текст назначены
        if (battleToggleButton == null || battleButtonText == null)
        {
            Debug.LogError("Кнопка битвы или ее текстовый компонент не назначены в инспекторе MenuManagerr!");
            return;
        }

        // Выключаем меню
        menuBackground.SetActive(false);
        playButton.SetActive(false);
        upgradeButton.SetActive(false);
        exitButton.SetActive(false);

        // Включаем игровые элементы
        gameUI.SetActive(true);
        player.SetActive(true);

        // Убедитесь, что CombatManager существует на сцене и не активен изначально
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.isBattleActive = false; // Убеждаемся, что битва не активна при старте игры
            isPaused = true; // Устанавливаем начальное состояние паузы
            UpdateBattleButtonText(); // Обновляем текст кнопки в соответствии с начальным состоянием

            // Привязываем наш новый метод к кнопке
            battleToggleButton.onClick.RemoveAllListeners(); // Удаляем старые слушатели (если были)
            battleToggleButton.onClick.AddListener(ToggleBattleState); // Добавляем нового слушателя

        } else {
             Debug.LogError("CombatManager.Instance не найден! Невозможно управлять битвой.");
        }
        
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
        
        // Активируем врага (он не начнет атаковать до старта битвы благодаря проверке в EnemyController)
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

    // Новый метод для переключения состояния битвы
    public void ToggleBattleState()
    {
        if (CombatManager.Instance == null)
        {
            Debug.LogError("CombatManager.Instance не найден! Невозможно переключить состояние битвы.");
            return;
        }

        isPaused = !isPaused; // Переключаем состояние паузы
        CombatManager.Instance.isBattleActive = !isPaused; // Обновляем состояние битвы в CombatManager
        UpdateBattleButtonText(); // Обновляем текст кнопки

        Debug.Log($"Battle state toggled. isBattleActive: {CombatManager.Instance.isBattleActive}");
    }

    // Метод для обновления текста на кнопке
    private void UpdateBattleButtonText()
    {
        if (battleButtonText != null)
        {
            battleButtonText.text = isPaused ? "СТАРТ БИТВЫ" : "ПАУЗА БИТВЫ";
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}