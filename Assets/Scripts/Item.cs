using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Определение перечисления ItemType
    public enum ItemType
    {
        Potion,
        Weapon,
        Armor,
        Accessory
    }

    [Header("Item Settings")]
    public ItemType itemType;        // Тип предмета
    public float effectValue;        // Значение эффекта (урон/лечение)
    public float cooldownTime;       // Время перезарядки
    public bool isEquipped;          // Экипирован ли предмет

    [Header("Weapon Properties")]
    public float damage;             // Урон оружия
    public float attackRange;        // Дистанция атаки
    public float attackSpeed;        // Скорость атаки (ударов в секунду)
    public bool isRanged;            // Является ли оружие дальнобойным
    public float projectileSpeed;    // Скорость снаряда (для дальнобойного оружия)
    public float criticalChance;     // Шанс критического удара
    public float criticalMultiplier; // Множитель критического урона

    [Header("UI References")]
    public Image cooldownImage;      // Ссылка на изображение перезарядки

    [HideInInspector] public Transform parentAfterDrag; // Куда вернуть предмет, если перетаскивание не удалось (его исходный родитель)
    private CanvasGroup canvasGroup;
    private Image itemImage;
    private Color normalColor;
    private const float dragAlpha = 0.6f;

    // Переменные для хранения исходного RectTransform и размера
    private RectTransform originalRect;
    private Vector2 originalSize; // Исходный sizeDelta
    private Vector2 originalAnchorsMin; // Исходные анкоры
    private Vector2 originalAnchorsMax;
    private Vector2 originalPivot;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
        normalColor = itemImage.color;
        originalRect = GetComponent<RectTransform>();
        originalSize = originalRect.sizeDelta; // Сохраняем исходный sizeDelta
        originalAnchorsMin = originalRect.anchorMin; // Сохраняем исходные анкоры
        originalAnchorsMax = originalRect.anchorMax;
        originalPivot = originalRect.pivot; // Сохраняем исходный пивот
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Сохраняем исходного родителя перед любыми изменениями
        parentAfterDrag = transform.parent;

        // Очищаем исходный слот, если предмет был в слоте
        Slot initialSlot = parentAfterDrag.GetComponent<Slot>();
        if (initialSlot != null)
        {
            initialSlot.ClearSlot(); // Вызываем метод ClearSlot у исходного слота
            Debug.Log($"Item {gameObject.name} started dragging from slot {initialSlot.gameObject.name}. Clearing slot.");
        } else {
             // Если предмет не был в слоте, возможно, он был в Items Panel Handler или другом контейнере
             Debug.Log($"Item {gameObject.name} started dragging from non-slot parent: {parentAfterDrag.gameObject.name}");
        }

        // Переносим предмет на верхний уровень UI (Canvas) для перетаскивания
        transform.SetParent(transform.root);

        // Настраиваем RectTransform для перетаскивания на Canvas (центр, исходный размер)
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = originalSize; // Восстанавливаем исходный sizeDelta

        transform.SetAsLastSibling(); // Отображаем поверх всего

        // Эффекты при начале перетаскивания
        canvasGroup.blocksRaycasts = false; // Игнорируем Raycast
        itemImage.color = new Color(normalColor.r, normalColor.g, normalColor.b, dragAlpha); // Полупрозрачность
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Плавное движение за курсором
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.root, // Родитель теперь корневой Canvas
            eventData.position, // Позиция курсора на экране
            GetComponentInParent<Canvas>().worldCamera, // Камера Canvas
            out Vector2 localPoint
        );
        transform.localPosition = localPoint; // Устанавливаем локальную позицию
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        bool handled = false; // Флаг, показывающий, обработано ли отпускание (попал в Slot или ItemsPanelHandler)
        Slot targetSlot = null;
        ItemsPanelHandler targetItemsPanel = null; // Переменная для ссылки на ItemsPanelHandler

        // --- Поиск целевой зоны отпускания ---
        if (dropTarget != null)
        {
            // 1. Проверяем, отпущено ли над Слотом (или предметом в слоте)
            targetSlot = dropTarget.GetComponent<Slot>();
            if (targetSlot == null && dropTarget.transform.parent != null && dropTarget.transform.parent.CompareTag("Slot"))
            {
                targetSlot = dropTarget.transform.parent.GetComponent<Slot>();
            }

            // 2. Если не над Слотом, проверяем, отпущено ли над Items Panel Handler (или его дочерним объектом)
            if (targetSlot == null) // Проверяем только если не попали в слот
            {
                 // Ищем компонент ItemsPanelHandler на dropTarget или его родителях
                 targetItemsPanel = dropTarget.GetComponentInParent<ItemsPanelHandler>();
            }
        }
        // --- Конец поиска целевой зоны ---


        // --- Обработка результата отпускания ---
        if (targetSlot != null && targetSlot.GetItemInSlot() == null)
        {
            // 1. Отпущено над СВОБОДНЫМ слотом
            transform.SetParent(targetSlot.transform); // Устанавливаем родителя на целевой слот
            transform.localPosition = Vector3.zero; // Центрируем предмет в слоте
            ResetSize(); // Сбрасываем размер, чтобы предмет заполнил слот

            // Обновляем ссылку itemInSlot в целевом слоте
            targetSlot.TryPlaceItem(this); // TryPlaceItem теперь просто устанавливает itemInSlot

            handled = true; // Отпускание обработано (успешно помещено в слот)
            Debug.Log($"Item {gameObject.name} successfully placed in slot {targetSlot.gameObject.name}.");
        }
        else if (targetItemsPanel != null)
        {
            // 2. Отпущено над зоной Items Panel Handler
            transform.SetParent(targetItemsPanel.transform); // Устанавливаем родителя на Items Panel Handler

            // Сбрасываем локальную позицию (можно поместить в начало списка или в конец, как нужно)
            transform.localPosition = Vector3.zero; // Пример: помещаем в (0,0) родителя Items Panel Handler

            // Возможно, нужно сбросить размер или установить определенный RectTransform для предметов в этой зоне
             // Например, вернуться к исходному размеру и анкорам 0,1 если Items Panel Handler - это Grid Layout
             RectTransform rt = GetComponent<RectTransform>();
             rt.anchorMin = originalAnchorsMin; // Восстанавливаем исходные анкоры
             rt.anchorMax = originalAnchorsMax;
             rt.pivot = originalPivot; // Восстанавливаем исходный пивот
             rt.sizeDelta = originalSize; // Возвращаем исходный sizeDelta

            // В этой логике предмет не попадает обратно в "слот" Items Panel Handler,
            // а просто становится его дочерним элементом.
            // Если Items Panel Handler использует Grid Layout Group, Unity сама расставит элементы.

            handled = true; // Отпускание обработано (возвращено в зону Items Panel Handler)
            Debug.Log($"Item {gameObject.name} returned to Items Panel Handler zone: {targetItemsPanel.gameObject.name}.");

            // ВАЖНО: Если Items Panel Handler имеет свою логику управления предметами (списки, массивы),
            // возможно, нужно вызвать у targetItemsPanel метод для добавления предмета обратно в его систему.
             // targetItemsPanel.AddItemToList(this); // Пример вызова
        }
        else // 3. Отпущено над занятым слотом ИЛИ над другой областью, не являющейся слотом или ItemsPanelHandler
        {
            // Если отпущено над занятым слотом - возвращаем на исходное место
            if (targetSlot != null && targetSlot.GetItemInSlot() != null)
            {
                 Debug.Log($"Target slot {targetSlot.gameObject.name} is occupied by {targetSlot.GetItemInSlot().gameObject.name}. Returning {gameObject.name} to original position.");
                 ReturnToParent(); // Возвращаем в исходный слот
                 handled = true; // Обработано как возврат в исходный слот
            }
            else
            {
                 // Если отпущено совсем вне целевых зон (не слот, не ItemsPanelHandler)
                 // В этой версии мы возвращаем предмет на исходное место, т.к. не хотим "выбрасывать" куда попало.
                 Debug.Log($"Item {gameObject.name} dropped outside any valid drop zone ({dropTarget?.name ?? "None"}). Returning to original position.");
                 ReturnToParent(); // Возвращаем в исходный слот
                 handled = true; // Обработано как возврат в исходный слот
            }
        }
        // --- Конец обработки результата отпускания ---


        // Восстанавливаем параметры CanvasGroup и цвета в конце
        canvasGroup.blocksRaycasts = true; // Включаем Raycast обратно
        itemImage.color = normalColor; // Восстанавливаем исходный цвет
    }

    // Возвращает предмет на исходное место (родителя parentAfterDrag)
    public void ReturnToParent()
    {
        transform.SetParent(parentAfterDrag); // Возвращаем к сохраненному родителю

        // --- Измененная логика возврата ---
        Slot originalSlot = parentAfterDrag.GetComponent<Slot>();
        ItemsPanelHandler originalItemsPanel = parentAfterDrag.GetComponent<ItemsPanelHandler>(); // Проверяем, был ли исходный родитель ItemsPanelHandler

        if (originalSlot != null)
        {
            // Если исходный родитель был Слотом
            transform.localPosition = Vector3.zero; // Центрируем в слоте
            ResetSize(); // Сбрасываем размер, чтобы заполнить слот

            // Убеждаемся, что исходный слот правильно обновляет свою ссылку itemInSlot
            originalSlot.TryPlaceItem(this); // Возвращаем ссылку в исходный слот

            Debug.Log($"Item {gameObject.name} returned to its original slot: {parentAfterDrag.gameObject.name}.");

        }
        else if (originalItemsPanel != null)
        {
            // Если исходный родитель был Items Panel Handler (или объектом с этим компонентом)
            transform.localPosition = Vector3.zero; // Центрируем в родителе (можно настроить, если нужно другое положение)

            // Восстанавливаем исходный размер и анкоры/пивот, которые были до перетаскивания в слот
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = originalSize; // Исходный sizeDelta
            rt.anchorMin = originalAnchorsMin; // Исходные анкоры
            rt.anchorMax = originalAnchorsMax;
            rt.pivot = originalPivot; // Исходный пивот

            // ВАЖНО: Если Items Panel Handler управляет списком предметов,
            // здесь, возможно, нужно вызвать его метод для добавления предмета обратно в список.
            // originalItemsPanel.AddItemBack(this); // Пример вызова

            Debug.Log($"Item {gameObject.name} returned to its original container: {parentAfterDrag.gameObject.name}.");

        }
        else // Если исходный родитель не Слот и не ItemsPanelHandler (непредвиденный случай или другой тип контейнера)
        {
             // В этом случае можно либо оставить предмет там (на canvas?), либо вернуть его в какое-то дефолтное место.
             // Пока оставим его в центре исходного родителя с исходным размером.
             transform.localPosition = Vector3.zero;
             RectTransform rt = GetComponent<RectTransform>();
             rt.sizeDelta = originalSize;
             rt.anchorMin = originalAnchorsMin;
             rt.anchorMax = originalAnchorsMax;
             rt.pivot = originalPivot;
             Debug.LogWarning($"Item {gameObject.name} returned to non-slot, non-ItemsPanelHandler parent: {parentAfterDrag.gameObject.name}. Check logic.");
        }
        // --- Конец измененной логики возврата ---
    }

    // Устанавливает размер RectTransform так, чтобы он заполнил своего родителя (подходит для предмета в слоте).
    public void ResetSize()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Метод Update для обработки клика (например, правого клика для использования)
    private void Update()
    {
        // Пример: правый клик на предмете для использования
         if (Input.GetMouseButtonDown(1))
         {
             // Проверяем, находится ли курсор над этим элементом UI
             if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
             {
                  Debug.Log("Right clicked on item: " + gameObject.name);

                  // Логика использования: проверьте, где находится предмет (например, в слоте экипировки)
                   Slot currentSlot = transform.parent.GetComponent<Slot>(); // Получаем текущий слот предмета
                   // if (currentSlot != null && currentSlot.CompareTag("EquipSlot")) // Пример проверки
                   // {
                       if (ItemUsageManager.Instance != null)
                       {
                           ItemUsageManager.Instance.UseItem(this); // Передаем сам предмет для использования
                       }
                       else
                       {
                           Debug.LogError("ItemUsageManager.Instance is not found! Ensure it exists in the scene.");
                       }
                   // }
             }
         }
    }

     // Метод для обновления UI перезарядки
     public void UpdateCooldownUI(float remainingTime, float totalTime)
     {
         if (cooldownImage != null)
         {
             cooldownImage.fillAmount = remainingTime / totalTime;
         }
     }
}