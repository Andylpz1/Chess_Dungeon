using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableNodeUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();  // 获取当前的 UI Canvas
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 鼠标按下时可以做一些额外处理（可选）
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 使用 Canvas 的比例调整拖拽距离
        if (canvas != null)
        {
            Vector2 delta = eventData.delta / canvas.scaleFactor;
            rectTransform.anchoredPosition += delta;
        }
    }
}
