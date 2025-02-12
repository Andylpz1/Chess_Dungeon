using UnityEngine;

public class WallLocation : NonEnterableLocation
{
    public override void Interact()
    {
        Debug.Log($"You see a wall at {position}. It's too thick to pass through.");
        // 由于没有特殊效果，Interact 方法可以是简单提示信息或留空
    }

    public void InitializeWall(Vector2Int wallPosition, string description = "A dense, impassable wall.")
    {
        Initialize(wallPosition, description, false);
    }
}
