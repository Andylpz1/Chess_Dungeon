using UnityEngine;

public class ForestLocation : NonEnterableLocation
{
    public override void Interact()
    {
        Debug.Log($"You see a dense forest at {position}. It's too thick to pass through.");
        // 由于没有特殊效果，Interact 方法可以是简单提示信息或留空
    }

    public void InitializeForest(Vector2Int forestPosition, string description = "A dense, impassable forest.")
    {
        Initialize(forestPosition, description, false);
    }
}
