using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    private void Awake()
    {
        gameObject.tag = "Slot"; // Убедитесь что есть тег Slot
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Item item = eventData.pointerDrag.GetComponent<Item>();
            if (item != null)
            {
                item.transform.SetParent(transform);
                item.ResetSize();
                item.transform.localPosition = Vector3.zero;
            }
        }
    }
}