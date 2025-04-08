using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Location : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int position;
    public string description;  // 描述信息
    public bool isAccessible;  // 是否可进入

    // 基础构造函数
    public virtual void Initialize(Vector2Int position, string description, bool isAccessible)
    {
        this.position = position;
        this.description = description;
        this.isAccessible = isAccessible;
    }

    // 鼠标悬停时显示描述
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Pointer entered location at {position}: {description}");
        ShowDescription();
    }

    // 鼠标离开时隐藏描述
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Pointer exited location at {position}");
        HideDescription();
    }

    // 显示描述信息的逻辑（可以留空，供你以后扩展）
    protected virtual void ShowDescription()
    {
        // 未来扩展：展示描述的 UI 面板或浮动窗口
    }

    // 隐藏描述信息的逻辑（可以留空，供你以后扩展）
    protected virtual void HideDescription()
    {
        // 未来扩展：隐藏描述的 UI 面板或浮动窗口
    }

    // 抽象方法，定义不同类型 Location 的交互效果
    public abstract void Interact();
}

public class EnterableLocation : Location
{
    public override void Interact()
    {
        Debug.Log($"You have entered an accessible location at {position}: {description}");
        // 未来扩展：比如补给、触发特殊事件等效果
    }
}

public class NonEnterableLocation : Location
{
    public override void Interact()
    {
        Debug.Log($"This location at {position} is blocked: {description}");
        // 未来扩展：例如警告、受伤效果等
    }
}
