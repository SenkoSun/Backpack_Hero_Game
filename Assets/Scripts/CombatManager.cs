using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance; // Singleton pattern for easy access

    [Header("Battle State")]
    public bool isBattleActive = false; // Состояние битвы: активна или нет

    [Header("Враг")]
    public GameObject enemy;
    public float enemySpawnDelay = 1f;

    public int countEnemy = 3;

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
                PlayerStats playerStats = target.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.Heal(item.effectValue);
                }
            }
        }
        // TODO: Добавить логику для других типов предметов (оружие, броня, аксессуары)
    }

    public void ProverkaCombat()
    {
        Debug.Log($"{countEnemy} {enemy} {isBattleActive}");
        if (isBattleActive && countEnemy > 0 && enemy != null)
        {
            //Вызов ActivateEnemy c Задержкой
            Invoke("ActivateEnemy", enemySpawnDelay);
        }
    }

    void ActivateEnemy()
    {
        if (enemy != null)
        {
            enemy.SetActive(true);

            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.level = 1; 
            if (enemyController != null)
            {
                enemyController.enabled = true;
            }
        }
    }

    // Метод для запуска битвы
    public void StartBattle(int level)
    {
        if (!isBattleActive)
        {
            isBattleActive = true;
            Debug.Log("Battle Started!");
            ProverkaCombat();
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