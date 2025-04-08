using UnityEngine;
using UnityEngine.EventSystems;

public class FirePoint : EnterableLocation
{
    // 燃点在棋盘中的位置（与 Location.position 一致，但单独作为逻辑字段使用）
    public Vector2Int gridPosition;

    // 引用场景中的 LocationManager（集成了燃点管理功能）
    private LocationManager locationManager;

    /// <summary>
    /// 初始化燃点。描述固定为“燃点”，并设置为可进入。
    /// 同时将燃点注册到 LocationManager 中，便于后续检测连通性和生成火域。
    /// </summary>
    /// <param name="position">棋盘坐标</param>
    /// <param name="description">描述（此处固定为“燃点”）</param>
    /// <param name="isAccessible">是否可进入（燃点通常是可进入的）</param>
    public override void Initialize(Vector2Int position, string description, bool isAccessible)
    {
        // 固定描述为“燃点”，并标记为可进入
        base.Initialize(position, "燃点", true);
        gridPosition = position;

        // 尝试获取场景中的 LocationManager，并注册此燃点
        locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null)
        {
            locationManager.OnFirePointAdded(this);
        }
        else
        {
            Debug.LogWarning("LocationManager not found in the scene.");
        }
    }

    /// <summary>
    /// 当玩家或其他对象与燃点交互时调用。
    /// 对于燃点来说，通常不做额外交互效果，只是提供提示。
    /// </summary>
    public override void Interact()
    {
        Debug.Log($"Interacted with FirePoint at {gridPosition}. It serves as terrain for forming Fire Zones.");
    }

    /// <summary>
    /// 销毁燃点。调用时会从 LocationManager 中移除记录，然后销毁自身。
    /// </summary>
    public void DestroySelf()
    {
        if (locationManager != null)
        {
            locationManager.RemoveFirePoint(this);
        }
        Destroy(gameObject);
    }
}
