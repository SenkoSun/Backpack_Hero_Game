using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    private CanvasGroup canvasGroup;
    private Image itemImage;
    private Color normalColor;
    private const float dragAlpha = 0.6f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
        normalColor = itemImage.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        // Эффекты при начале перетаскивания
        canvasGroup.blocksRaycasts = false;
        itemImage.color = new Color(normalColor.r, normalColor.g, normalColor.b, dragAlpha);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        // Плавное движение за курсором (с lerp для плавности)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform.parent,
            Input.mousePosition,
            GetComponentInParent<Canvas>().worldCamera,
            out Vector2 localPoint
        );
        transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Восстанавливаем параметры
        canvasGroup.blocksRaycasts = true;
        itemImage.color = normalColor;
        
        // Проверяем куда упал предмет
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        
        if (dropTarget == null || (!dropTarget.CompareTag("Slot") && !dropTarget.CompareTag("ItemsPanel")))
        {
            // Возвращаем на исходную позицию если не попал в допустимую зону
            ReturnToParent();
        }
    }

    public void ReturnToParent()
    {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        ResetSize();
    }

    public void ResetSize()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}