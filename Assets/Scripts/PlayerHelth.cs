using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public Slider healthSlider;
    public GameObject deathScreen; // Перетащите DeathScreen сюда
    
    private int currentHealth;
    private bool isDead = false;

    public GameObject player; // Перетащите игрока
    public GameObject[] enemies; // Перетащите всех врагов

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (deathScreen != null) 
            deathScreen.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        UpdateHealthUI();
        
        if (currentHealth <= 0) Die();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    void Die()
    {
        isDead = true;
        // 1. Отключаем игрока и врагов
        player.SetActive(false);
        foreach(GameObject enemy in enemies)
        {
            if(enemy != null) 
                enemy.SetActive(false);
        }

        // 2. Показываем экран смерти
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            // Делаем кнопку активной
            Button menuButton = deathScreen.GetComponentInChildren<Button>();
            if(menuButton != null) 
                menuButton.interactable = true;
        }

        // 3. Останавливаем игру
        Time.timeScale = 0f;
    }

    // Вызывается по кнопке "В меню"
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Возвращаем нормальную скорость игры
        SceneManager.LoadScene(0); // Загружаем первую сцену (меню)
    }

        public void ResetPlayer()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += Mathf.RoundToInt(amount); // Преобразуем float в int для здоровья
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Не превышаем максимальное здоровье
        UpdateHealthUI();
    }
}