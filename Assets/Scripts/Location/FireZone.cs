using UnityEngine;
using System.Collections.Generic;

public class FireZone : MonoBehaviour
{
    public int remainingEnemyTurns;
    private List<Vector3> polygonPoints;
    private LineRenderer lineRenderer;

    public void Initialize(List<Vector3> points, int duration)
    {
        remainingEnemyTurns = duration;
        if (points == null || points.Count < 2)
        {
            Debug.LogError("FireZone initialization failed: polygon points are null or insufficient.");
            return;
        }
        polygonPoints = new List<Vector3>(points);
        if (polygonPoints[0] != polygonPoints[polygonPoints.Count - 1])
            polygonPoints.Add(polygonPoints[0]);

        lineRenderer = GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = polygonPoints.Count;
        lineRenderer.SetPositions(polygonPoints.ToArray());
        lineRenderer.startColor = lineRenderer.endColor = Color.red;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
    }

    public void OnEnemyTurnStart()
    {
        if (polygonPoints == null)
        {
            Debug.LogWarning("FireZone polygonPoints is null. Skipping damage.");
            return;
        }

        foreach (Monster monster in FindObjectsOfType<Monster>())
        {
            Vector3 pos = monster.transform.position;
            bool hit = IsPointInPolygon(pos, polygonPoints)
                       || IsCellTouchedByPolygonEdges(pos, polygonPoints);
            if (hit)
                monster.TakeDamage(2);
        }

        if (--remainingEnemyTurns <= 0)
            DestroySelf();
    }

    private bool IsPointInPolygon(Vector3 point, List<Vector3> poly)
    {
        if (poly == null || poly.Count < 3) return false;
        // 边上判断
        float eps = 0.01f;
        for (int i = 0; i < poly.Count - 1; i++)
            if (IsPointOnSegment(point, poly[i], poly[i + 1], eps))
                return true;
        // 射线法
        int cnt = 0;
        for (int i = 0; i < poly.Count - 1; i++)
        {
            Vector3 a = poly[i], b = poly[i + 1];
            if (((a.y > point.y) != (b.y > point.y)) &&
                (point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x))
                cnt++;
        }
        return (cnt & 1) == 1;
    }

    private bool IsPointOnSegment(Vector3 p, Vector3 a, Vector3 b, float eps)
    {
        float cross = Mathf.Abs((b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x));
        if (cross > eps) return false;
        float dot = (p.x - a.x) * (p.x - b.x) + (p.y - a.y) * (p.y - b.y);
        return dot <= eps;
    }

    /// <summary>
    /// 判断怪物所在的 1×1 单元格（以整数坐标为左下角）是否与多边形任一边相交
    /// </summary>
    private bool IsCellTouchedByPolygonEdges(Vector3 worldPos, List<Vector3> poly)
    {
        // 确定格子左下角坐标
        int gx = Mathf.FloorToInt(worldPos.x);
        int gy = Mathf.FloorToInt(worldPos.y);
        Rect cell = new Rect(gx, gy, 1f, 1f);

        // 每条多边形边，与该 Rect 逐一测试
        for (int i = 0; i < poly.Count - 1; i++)
        {
            Vector2 p1 = new Vector2(poly[i].x, poly[i].y);
            Vector2 p2 = new Vector2(poly[i+1].x, poly[i+1].y);
            if (SegmentIntersectsRect(p1, p2, cell))
                return true;
        }
        return false;
    }

    // 线段-矩形 相交检测
    private bool SegmentIntersectsRect(Vector2 a, Vector2 b, Rect r)
    {
        // 若任一端点在矩形内，算相交
        if (r.Contains(a) || r.Contains(b)) return true;

        // 检查与矩形四条边的线段相交
        Vector2[] corners = new Vector2[]
        {
            new Vector2(r.xMin, r.yMin),
            new Vector2(r.xMax, r.yMin),
            new Vector2(r.xMax, r.yMax),
            new Vector2(r.xMin, r.yMax)
        };
        for (int i = 0; i < 4; i++)
        {
            Vector2 c1 = corners[i];
            Vector2 c2 = corners[(i + 1) % 4];
            if (SegmentsIntersect(a, b, c1, c2))
                return true;
        }
        return false;
    }

    // 经典的线段相交（严格相交，不含端点共线情况）
    private bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);
        return (o1 * o2 < 0f) && (o3 * o4 < 0f);
    }

    private float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        // 返回叉积符号：>0 左转, <0 右转
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    public void DestroySelf()
    {
        if (polygonPoints != null)
        {
            foreach (FirePoint fp in FindObjectsOfType<FirePoint>())
            {
                if (IsPointInPolygon(fp.transform.position, polygonPoints))
                    fp.DestroySelf();
            }
        }
        Destroy(gameObject);
    }
}
