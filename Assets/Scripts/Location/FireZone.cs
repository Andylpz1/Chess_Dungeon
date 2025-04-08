using UnityEngine;
using System.Collections.Generic;

public class FireZone : MonoBehaviour
{
    // 火域持续的敌方回合数
    public int remainingEnemyTurns;

    // 存储构成火域的多边形顶点（世界坐标）
    private List<Vector3> polygonPoints;

    private LineRenderer lineRenderer;

    /// <summary>
    /// 使用多边形顶点初始化火域，持续时间为 duration
    /// </summary>
    /// <param name="points">多边形顶点列表（世界坐标），必须闭合</param>
    /// <param name="duration">火域持续敌方回合数</param>
    public void Initialize(List<Vector3> points, int duration)
    {
        remainingEnemyTurns = duration;
        // 确保传入的顶点列表不为空，并且闭合
        if (points == null || points.Count < 2)
        {
            Debug.LogError("FireZone initialization failed: polygon points are null or insufficient.");
            return;
        }
        polygonPoints = new List<Vector3>(points);
        // 如果多边形不闭合，则自动闭合
        if (polygonPoints[0] != polygonPoints[polygonPoints.Count - 1])
        {
            polygonPoints.Add(polygonPoints[0]);
        }
        
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = polygonPoints.Count;
        lineRenderer.SetPositions(polygonPoints.ToArray());
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
    }

    /// <summary>
    /// 在每个敌方回合开始时调用火域效果：
    /// 对火域内敌人造成2点伤害，并减少持续回合数，若用尽则销毁火域
    /// </summary>
    public void OnEnemyTurnStart()
    {
        if (polygonPoints == null)
        {
            Debug.LogWarning("FireZone polygonPoints is null. Skipping damage.");
            return;
        }

        Monster[] allMonsters = FindObjectsOfType<Monster>();
    foreach (Monster monster in allMonsters)
    {
        if (IsPointInPolygon(monster.transform.position, polygonPoints))
        {
            monster.TakeDamage(2);
        }
    }

        remainingEnemyTurns--;
        if (remainingEnemyTurns <= 0)
            DestroySelf();
    }

    /// <summary>
    /// 判断一个点是否处于由多边形顶点构成的区域内
    /// 使用射线法进行判断
    /// </summary>
    private bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        // 如果多边形为空或点数不足，则返回 false
        if (polygon == null || polygon.Count < 3)
        {
            Debug.LogWarning("IsPointInPolygon: polygon is null or has fewer than 3 points.");
            return false;
        }
    
        // 先判断点是否位于任意边上
        float epsilon = 0.01f; // 可根据需要调整精度
        for (int i = 0; i < polygon.Count - 1; i++)
        {
            Vector3 a = polygon[i];
            Vector3 b = polygon[i + 1];
            if (IsPointOnSegment(point, a, b, epsilon))
            {
                return true;
            }
        }
    
        // 使用射线法：从 point 向右投射水平射线，计算与多边形边的交点数量
        int intersectCount = 0;
        for (int i = 0; i < polygon.Count - 1; i++)
        {
            Vector3 a = polygon[i];
            Vector3 b = polygon[i + 1];

            // 检查点的 y 坐标是否在边 a、b 的 y 区间内
            if (((a.y > point.y) != (b.y > point.y)) &&
                // 计算射线与边的交点的 x 坐标
                (point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x))
            {
                intersectCount++;
            }
        }
        return (intersectCount % 2) == 1;
    }

    private bool IsPointOnSegment(Vector3 p, Vector3 a, Vector3 b, float epsilon)
    {
        // 计算向量叉积，检查 p 是否与 a、b 共线
        float cross = Mathf.Abs((b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x));
        if (cross > epsilon)
            return false;

        // 计算点 p 是否在 a、b 之间（利用点积判断）
        float dot = (p.x - a.x) * (p.x - b.x) + (p.y - a.y) * (p.y - b.y);
        return dot <= epsilon;
    }


    /// <summary>
    /// 销毁火域对象，同时清除火域覆盖范围内的所有燃点
    /// </summary>
    public void DestroySelf()
    {
        if (polygonPoints == null)
        {
            Debug.LogWarning("FireZone polygonPoints is null during DestroySelf.");
        }
        else
        {
            FirePoint[] allFirePoints = FindObjectsOfType<FirePoint>();
            foreach (FirePoint fp in allFirePoints)
            {
                // 使用燃点的 transform.position 与火域 polygon 判断
                if (IsPointInPolygon(fp.transform.position, polygonPoints))
                {
                    fp.DestroySelf();
                }
            }
        }
        Destroy(gameObject);
    }
}
