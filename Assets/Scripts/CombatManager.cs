using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public void UseItemInCombat(Item item, GameObject target)
    {
        if (item.itemType == Item.ItemType.Potion)
        {
            // Применение эффекта зелья
            if (target.CompareTag("Enemy"))
            {
                // Нанесение урона врагу
                target.GetComponent<EnemyController>().TakeDamage(item.effectValue);
            }
            else if (target.CompareTag("Player"))
            {
                // Лечение игрока
                target.GetComponent<PlayerHealth>().Heal(item.effectValue);
            }
        }
    }
}