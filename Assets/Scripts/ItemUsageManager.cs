using UnityEngine;
using System.Collections.Generic;

public class ItemUsageManager : MonoBehaviour
{
    // Синглтон для доступа из других скриптов
    public static ItemUsageManager Instance;

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
                break;
            // Добавьте другие случаи
        }
    }
    
    private void StartCooldown(Item item)
    {
        itemCooldowns[item] = Time.time + item.cooldownTime;
    }
}