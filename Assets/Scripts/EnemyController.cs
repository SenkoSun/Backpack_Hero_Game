using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Настройки")]
    public float attackCooldown = 2f;
    public int damage = 10;
    
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
}