using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance; // Singleton pattern for easy access

    [Header("Battle State")]
    public bool isBattleActive = false; // Состояние битвы: активна или нет

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Метод для использования предмета в бою (сейчас только зелья)
    public void UseItemInCombat(Item item, GameObject target)
    {
        // Проверяем, активна ли битва, прежде чем использовать предмет
        if (!isBattleActive) return; // Не использовать предметы, если битва не активна

        if (item.itemType == Item.ItemType.Potion)
        {
            // Применение эффекта зелья
            if (target.CompareTag("Enemy"))
            {
                // Нанесение урона врагу
                EnemyController enemy = target.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(item.effectValue);
                }
            }
            else if (target.CompareTag("Player"))
            {
                // Лечение игрока
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(item.effectValue);
                }
            }
        }
        // TODO: Добавить логику для других типов предметов (оружие, броня, аксессуары)
    }

    // Метод для запуска битвы
    public void StartBattle()
    {
        if (!isBattleActive)
        {
            isBattleActive = true;
            Debug.Log("Battle Started!");
            // TODO: Возможно, добавить логику для старта вражеских действий, музыки и т.д.
        }
    }

    // Метод для остановки битвы (например, при победе или поражении)
    public void EndBattle()
    {
        if (isBattleActive)
        {
            isBattleActive = false;
            Debug.Log("Battle Ended!");
            // TODO: Возможно, добавить логику для перехода к следующему этапу, показа статистики и т.д.
        }
    }

    // TODO: Добавить логику управления ходами, атаками врагов и т.д. в Update или других методах
}