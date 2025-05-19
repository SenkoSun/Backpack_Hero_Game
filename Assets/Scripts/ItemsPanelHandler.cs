using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemsPanelHandler : MonoBehaviour, IDropHandler
{
    // Список для хранения предметов в инвентаре
    public List<Item> inventoryItems = new List<Item>();
    // TODO: Добавить ссылку на ваш Grid Layout Group или компонент, который расставляет предметы, если используете его
    // public GridLayoutGroup gridLayoutGroup;

    private void Awake()
    {
        gameObject.tag = "ItemsPanel"; // Убедитесь что есть тег ItemsPanel
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        if (item != null)
        {
            item.ReturnToParent();
        }
    }

    public void AddItemBack(Item item)
    {
        if (item == null) return;

        item.transform.localPosition = Vector3.zero;

        Debug.Log($"Item {item.gameObject.name} successfully added back to ItemsPanelHandler's internal list/system.");
    }

    // Публичный метод для получения списка предметов в инвентаре
    public List<Item> GetInventoryItems()
    {
        return inventoryItems;
    }

    // TODO: Возможно, нужен метод для удаления предмета из инвентаря при перетаскивании
}