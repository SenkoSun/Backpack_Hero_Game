using UnityEngine;
using System.Collections.Generic;

public class ItemUsageManager : MonoBehaviour
{
    // Синглтон для доступа из других скриптов
    public static ItemUsageManager Instance;

    [Header("Projectile Settings")]
    public GameObject shurikenPrefab;    // Префаб сюрикена
    public Transform projectileSpawnPoint; // Точка появления снарядов

    [Header("Inventory Integration")]
    public ItemsPanelHandler itemsPanelHandler; // Ссылка на ваш ItemsPanelHandler (как пример инвентаря)

    [Header("Enemy Detection")]
    public float detectionRadius = 10f; // Радиус поиска врагов
    public LayerMask enemyLayer; // Слой, на котором находятся враги

    // TODO: Добавить ссылку на ваш менеджер инвентаря или метод получения активных предметов

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private Dictionary<Item, float> itemCooldowns = new Dictionary<Item, float>();
    
    public void UseItem(Item item)
    {
        // Проверяем, активна ли битва, прежде чем использовать предмет
        if (CombatManager.Instance != null && !CombatManager.Instance.isBattleActive) return; // Не использовать предметы, если битва не активна

        if (CanUseItem(item))
        {
            // Обработка эффекта предмета
            ProcessItemEffect(item);
            // Запуск перезарядки
            StartCooldown(item);
        }
    }
    
    private bool CanUseItem(Item item)
    {
        return !itemCooldowns.ContainsKey(item) || 
               Time.time >= itemCooldowns[item];
    }
    
    private void ProcessItemEffect(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Potion:
                // Обработка эффекта зелья
                // Для зелий, возможно, нужно вызвать UseItemInCombat из CombatManager
                 if (CombatManager.Instance != null)
                 {
                      // TODO: Определить цель для зелья (игрок или враг)
                     // CombatManager.Instance.UseItemInCombat(item, targetGameObject);
                 }
                break;
            case Item.ItemType.Weapon:
                if (item.isRanged)
                {
                    // Дальнобойное оружие атакует автоматически в Update
                    // Эту логику, возможно, нужно убрать, если автоматическая атака только в Update
                }
                // TODO: Для ближнего боя нужно добавить логику атаки
                break;
        }
    }
    
    private void StartCooldown(Item item)
    {
        itemCooldowns[item] = Time.time + item.cooldownTime;
        // TODO: Обновить UI кулдауна для этого предмета, если есть такая система
        // item.UpdateCooldownUI(item.cooldownTime, item.cooldownTime); // Пример
    }

    // Метод Update для автоматического использования предметов
    private void Update()
    {
        // Проверяем, активна ли битва, прежде чем искать врагов и атаковать
        if (CombatManager.Instance == null || !CombatManager.Instance.isBattleActive) return; // Не атаковать, если битва не активна

        // Получаем список активных предметов из вашего менеджера инвентаря
        // В данном случае используем список из ItemsPanelHandler как пример активных предметов
        List<Item> activeItems = (itemsPanelHandler != null) ? itemsPanelHandler.GetInventoryItems() : new List<Item>();

        // Ищем ближайшего врага
        Transform targetEnemy = FindNearestEnemy();

        if (targetEnemy != null)
        {
            foreach (var item in activeItems)
            {
                // Проверяем, является ли предмет дальнобойным оружием и готов ли он к использованию
                if (item != null && item.itemType == Item.ItemType.Weapon && item.isRanged && CanUseItem(item))
                {
                    // Используем предмет (бросаем сюрикен) в сторону найденного врага
                    ThrowWeaponTowardsEnemy(item, targetEnemy.position);
                    // TODO: Обновить UI кулдауна для этого предмета после использования
                    // item.UpdateCooldownUI(item.cooldownTime, item.cooldownTime); // Пример
                }
            }
        }

        // TODO: В Update также нужно обновлять UI кулдаунов для всех предметов в инвентаре
        // foreach (var item in itemCooldowns.Keys)
        // {
        //     float remaining = itemCooldowns[item] - Time.time;\
        //     if (remaining < 0) remaining = 0;\
        //     item.UpdateCooldownUI(remaining, item.cooldownTime); // Пример\
        // }\
    }

    // Метод для поиска ближайшего врага в радиусе обнаружения
    private Transform FindNearestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            // TODO: Возможно, добавить проверку, что это действительно враг, используя тег или компонент
             if (hitCollider.CompareTag("Enemy")) // Используем тег для более точной идентификации
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < minDistance)
                {
                    minDistance = distanceToEnemy;
                    nearestEnemy = hitCollider.transform;
                }
            }
        }
        return nearestEnemy;
    }

    // Модифицированный метод ThrowWeapon для стрельбы в сторону цели
    private void ThrowWeaponTowardsEnemy(Item item, Vector3 targetPosition)
    {
        if (shurikenPrefab != null && projectileSpawnPoint != null)
        {
            // Вычисляем направление броска к цели
            Vector3 direction = (targetPosition - projectileSpawnPoint.position).normalized;

            // Создаем сюрикен
            GameObject shuriken = Instantiate(shurikenPrefab, projectileSpawnPoint.position, Quaternion.identity);
            ShurikenProjectile projectile = shuriken.GetComponent<ShurikenProjectile>();

            if (projectile != null)
            {
                // Устанавливаем урон снаряда
                projectile.damage = item.damage;
                // Инициализируем снаряд с направлением к врагу
                projectile.Initialize(direction, transform);
            }
        }
        else
        {
            Debug.LogError("Shuriken prefab or spawn point is not set!");
        }
    }

    // Удаляем старый ThrowWeapon, который использовал позицию мыши
    // private void ThrowWeapon(Item item) { /* ... */ }
}