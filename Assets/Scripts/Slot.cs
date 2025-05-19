using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    // Переменная для отслеживания предмета, который находится в этом слоте
    private Item itemInSlot;

    private void Awake()
    {
        // Убедитесь, что GameObject слота имеет тег "Slot" в инспекторе Unity
        gameObject.tag = "Slot";

        // Проверяем, есть ли уже предмет (дочерний объект с компонентом Item) в слоте при старте сцены
        if (transform.childCount > 0)
        {
            itemInSlot = transform.GetChild(0).GetComponent<Item>();
             if (itemInSlot != null)
            {
                // Если предмет найден, убеждаемся, что его родитель установлен на этот слот
                // и его размер сброшен, чтобы он заполнял слот.
                itemInSlot.transform.SetParent(transform);
                itemInSlot.transform.localPosition = Vector3.zero;
                itemInSlot.ResetSize(); // Вызываем ResetSize у предмета, чтобы он заполнил этот слот
                Debug.Log($"Slot {gameObject.name} initially contains {itemInSlot.gameObject.name}.");
            }
             else
             {
                 // Если дочерний объект есть, но на нем нет компонента Item - очищаем слот
                 ClearSlot(); // Освобождаем слот
             }
        } else {
            itemInSlot = null; // Если нет дочерних объектов, слот свободен
        }
    }

    // Метод, вызываемый Unity Event System при отпускании перетаскиваемого объекта над этим слотом
    public void OnDrop(PointerEventData eventData)
    {
        // Этот метод теперь НЕ СОДЕРЖИТ логику размещения предмета.
        // Вся логика отпускания перенесена в Item.OnEndDrag.
         Debug.Log($"Item dropped over slot {gameObject.name}. Processing handled by Item.OnEndDrag.");
    }

    // Этот метод вызывается ИЗ СКРИПТА ITEM (Item.OnEndDrag или Item.ReturnToParent),
    // чтобы слот обновил ссылку на предмет, который в него поместили.
    public bool TryPlaceItem(Item item)
    {
         // Проверяем, свободен ли слот.
         // ИЛИ проверяем, что предмет, который пытаемся поместить, УЖЕ находится в этом слоте (на случай ReturnToParent).
        if (itemInSlot == null || itemInSlot == item)
        {
             itemInSlot = item; // Устанавливаем ссылку на предмет
             // Логика установки родителя, позиции и размера ПЕРЕНЕСЕНА В Item.OnEndDrag ИЛИ Item.ReturnToParent
             Debug.Log($"Slot {gameObject.name} now holds {item.gameObject.name}.");
             return true; // Сообщаем, что слот теперь держит предмет
        }
         else
         {
              // Этот случай должен быть редок при правильной логике Item.OnEndDrag,
              // но может помочь отловить проблемы, если предмет пытаются
              // поместить в слот, который уже занят другим предметом.
              Debug.LogWarning($"TryPlaceItem called on occupied slot {gameObject.name} for item {item.gameObject.name}! Slot is occupied by {itemInSlot.gameObject.name}");
             return false; // Сообщаем, что слот был занят другим предметом
         }
    }

    // Метод для освобождения слота (вызывается из Item.OnBeginDrag)
    public void ClearSlot()
    {
        // Проверяем, был ли в слоте предмет перед очисткой
        if (itemInSlot != null)
        {
            Debug.Log($"Slot {gameObject.name} cleared. Previously held {itemInSlot.gameObject.name}.");
        } else {
             Debug.Log($"Slot {gameObject.name} cleared (was already empty).");
        }
        itemInSlot = null; // Сбрасываем ссылку на предмет, делая слот свободным.
    }

    // Публичный метод для получения предмета, находящегося в слоте
    public Item GetItemInSlot()
    {
        return itemInSlot;
    }

    // Дополнительно: при уничтожении предмета в слоте, нужно освободить слот.
    // Это можно сделать, добавив в Item.OnDestroy { if (transform.parent != null && transform.parent.CompareTag("Slot")) transform.parent.GetComponent<Slot>().ClearSlot(); }
}