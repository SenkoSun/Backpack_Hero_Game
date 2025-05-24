using UnityEngine;
using UnityEngine.UI; // Не забудьте добавить для работы с UI
using TMPro; // Добавляем для работы с TextMeshPro
public class EnemyController : MonoBehaviour
{
    [Header("Настройки")]
    public float attackCooldown = 2f;
    public int damage = 10;
    public float maxHealth = 50f; // Максимальное здоровье врага

    private float currentHealth; // Текущее здоровье
    public Slider healthSlider; // Перетащите сюда ваш HP-бар
    
    [Header("Снаряд")]
    public GameObject projectilePrefab; // Перетащите префаб снаряда сюда!
    public Transform firePoint;         // Точка выстрела
    public float projectileSpeed = 5f;
    
    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Если firePoint не назначен - используем позицию врага
        if (firePoint == null) 
            firePoint = transform;

        // Инициализируем текущее здоровье максимальным при старте
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        UpdateHealthUI();
        // Устанавливаем время следующей атаки в будущем, чтобы избежать немедленной атаки при спавне
        nextAttackTime = Time.time + attackCooldown; // Можно установить еще больше, если нужна дополнительная задержка после старта битвы
    }

    void Update()
    {
        // Проверяем, активна ли битва, прежде чем атаковать
        if (CombatManager.Instance == null || !CombatManager.Instance.isBattleActive) return; // Не атаковать, если битва не активна

        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Не назначен projectilePrefab!");
            return;
        }

        // Создаем снаряд
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Направление к игроку
        Vector2 direction = (player.position - firePoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;

        // Настраиваем урон
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
            projectileScript.damage = damage;
        TakeDamage(25);
    }

    public void TakeDamage(float amount)
    {
        // Вычитаем урон из текущего здоровья
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");
        UpdateHealthUI();
        // Проверяем, умер ли враг
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    // Метод для обработки смерти врага

    void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        healthSlider.gameObject.SetActive(false);
        // enemy.SetActive(false);
        // TODO: Добавить логику выпадения лута, анимацию смерти и т.д.
        
        Destroy(gameObject); // Пока просто уничтожаем объект
        // CombatManager.Instance.ProverkaCombat();
    }
}