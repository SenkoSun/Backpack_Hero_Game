using UnityEngine;

public class ShurikenProjectile : MonoBehaviour
{
    public float speed = 20f;        // Скорость полета сюрикена
    public float damage = 10f;       // Урон сюрикена
    public float rotationSpeed = 720f; // Скорость вращения сюрикена
    public float lifetime = 3f;      // Время жизни сюрикена

    private Vector3 direction;       // Направление полета
    private Transform player;        // Ссылка на игрока

    private void Start()
    {
        // Уничтожаем сюрикен через lifetime секунд
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 targetDirection, Transform playerTransform)
    {
        direction = targetDirection.normalized;
        player = playerTransform;
        
        // Поворачиваем сюрикен в направлении полета
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        // Двигаем сюрикен
        transform.position += direction * speed * Time.deltaTime;
        
        // Вращаем сюрикен
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, попали ли мы во врага
        if (other.CompareTag("Enemy"))
        {
            // Наносим урон врагу
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // Уничтожаем сюрикен после попадания
            Destroy(gameObject);
        }
        // Если сюрикен попал в стену или другой объект
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
} 