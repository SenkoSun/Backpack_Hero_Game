using UnityEngine;
using UnityEngine.EventSystems;

public class ItemsPanelHandler : MonoBehaviour, IDropHandler
{
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
}