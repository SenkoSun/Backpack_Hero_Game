using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy")) // Чтобы не уничтожался при коллизии с другими врагами
        {
            Destroy(gameObject);
        }
    }
}