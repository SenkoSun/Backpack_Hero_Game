using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;          // Название предмета
    public Item.ItemType type;       // Тип предмета
    public float effectValue;        // Значение эффекта
    public float cooldownTime;       // Время перезарядки
    public string description;       // Описание
    public Sprite icon;             // Иконка
}