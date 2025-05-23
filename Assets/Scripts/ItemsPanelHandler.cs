using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;

public class ItemsPanelHandler : MonoBehaviour, IDropHandler
{
    // Список для хранения предметов в инвентаре
    public List<Item> inventoryItems = new List<Item>();
    // TODO: Добавить ссылку на ваш Grid Layout Group или компонент, который расставляет предметы, если используете его
    // public GridLayoutGroup gridLayoutGroup;

    [Header("Настройки предметов")]
    [SerializeField] private int numberOfRandomItems = 4; // Количество случайных предметов
    [SerializeField] private string itemsFolderPath = "Items"; // Путь к папке с префабами предметов

    private void Awake()
    {
        gameObject.tag = "ItemsPanel"; // Убедитесь что есть тег ItemsPanel
    }

    private void Start()
    {
        LoadRandomItems();
    }

    private void LoadRandomItems()
    {
        // Загружаем все префабы из папки Items
        Object[] itemPrefabs = Resources.LoadAll(itemsFolderPath, typeof(GameObject));
        
        if (itemPrefabs.Length == 0)
        {
            Debug.LogError($"Не найдены префабы предметов в папке {itemsFolderPath}!");
            return;
        }

        // Создаем список для хранения индексов
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            availableIndices.Add(i);
        }

        // Выбираем случайные предметы
        for (int i = 0; i < numberOfRandomItems && availableIndices.Count > 0; i++)
        {
            // Выбираем случайный индекс
            int randomIndex = Random.Range(0, availableIndices.Count);
            int itemIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            // Создаем предмет
            GameObject itemObject = Instantiate(itemPrefabs[itemIndex] as GameObject, transform);
            Item item = itemObject.GetComponent<Item>();
            
            if (item != null)
            {
                inventoryItems.Add(item);
                item.transform.localPosition = Vector3.zero;
                Debug.Log($"Создан предмет: {itemObject.name}");
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        if (item != null)
        {
            // Check if the item was dragged from a slot
            // The parentAfterDrag is the original parent before dragging.
            Slot previousSlot = item.parentAfterDrag.GetComponent<Slot>();

            if (previousSlot != null)
            {
                // Item was dragged from a slot, add it back to the inventoryItems list
                // We assume Item.OnEndDrag has already set the parent to this ItemsPanelHandler
                // and the item's position/size is set correctly.
                AddItemBack(item);
                Debug.Log($"Item {item.gameObject.name} dropped from slot {previousSlot.gameObject.name} onto ItemsPanelHandler. Added back to inventory.");
            }
            else
            {
                // Item was dragged from somewhere else (likely the ItemsPanel itself), 
                // let Item.OnEndDrag handle the parent/positioning. We don't need to do anything specific here.
                Debug.Log($"Item {item.gameObject.name} dropped onto ItemsPanelHandler from non-slot parent. Item.OnEndDrag handles final placement.");
                // If you had specific logic for items dropped from other sources, it would go here.
            }
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