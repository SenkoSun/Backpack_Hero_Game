using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Настройки")]
    public float attackCooldown = 2f;
    public int damage = 10;
    public float maxHealth = 50f; // Максимальное здоровье врага

    private float currentHealth; // Текущее здоровье
    
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
    }

    void Update()
    {
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
    }

    public void TakeDamage(float amount)
    {
        // Вычитаем урон из текущего здоровья
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        // Проверяем, умер ли враг
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Метод для обработки смерти врага
    void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        // TODO: Добавить логику выпадения лута, анимацию смерти и т.д.
        Destroy(gameObject); // Пока просто уничтожаем объект
    }
}