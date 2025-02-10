using UnityEngine;
using System.Collections.Generic;


public class ActivatePoint : Scene
{
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
    }


    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/UI/activate");
    }
}

public class DeactivatePoint : Scene
{
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
    }


    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/UI/deactivate");
    }
}
