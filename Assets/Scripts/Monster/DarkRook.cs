using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DarkRook 采用轻量级 Monte‑Carlo Tree Search (MCTS) 选择下一步位置。
/// 仅在无障碍直线格中展开搜索，避免路径受阻。
/// 评估函数 = 终局距玩家的欧氏距离 + 行动步数，越小越好。
/// </summary>
public class DarkRook : Monster
{
    // 四个正方向 (横纵)
    private static readonly Vector2Int[] directions =
    {
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1)
    };

    // MCTS 参数
    private const int SIMULATIONS_PER_MOVE = 30; // 每个候选动作的随机模拟次数
    private const int MAX_PLAYOUT_DEPTH    = 3;  // 单次随机模拟的最长深度

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "DarkRook";
        type        = MonsterType.Rook;
    }

    /// <summary>
    /// 使用 MCTS 选取下一步。
    /// </summary>
    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        // 生成候选动作
        List<Vector2Int> legalMoves = CalculatePossibleMoves();
        if (legalMoves.Count == 0) return;

        Vector2Int bestMove = position;
        float      bestEval = float.MaxValue;

        foreach (Vector2Int move in legalMoves)
        {
            float cumulative = 0f;
            for (int i = 0; i < SIMULATIONS_PER_MOVE; i++)
                cumulative += SimulatePlayout(move);

            float avgScore = cumulative / SIMULATIONS_PER_MOVE;
            if (avgScore < bestEval)
            {
                bestEval = avgScore;
                bestMove = move;
            }
        }

        position = bestMove;
        UpdatePosition();

        if (position == player.position)
        {
            Debug.Log("DarkRook attacks the player.");
            // player.TakeDamage(1);
        }
    }

    /// <summary>
    /// 随机模拟一次走子，返回评估值。
    /// </summary>
    private float SimulatePlayout(Vector2Int start)
    {
        Vector2Int simPos = start;
        int        depth  = 0;

        while (depth < MAX_PLAYOUT_DEPTH && simPos != player.position)
        {
            List<Vector2Int> moves = GetLegalMovesFrom(simPos);
            if (moves.Count == 0) break;

            simPos = moves[Random.Range(0, moves.Count)];
            depth++;
        }

        // 评估：离玩家越近越好，同时偏好较短路径
        return Vector2Int.Distance(simPos, player.position) + depth;
    }

    /// <summary>
    /// 返回 origin 出发的所有合法直线格（无障碍）。
    /// </summary>
    private List<Vector2Int> GetLegalMovesFrom(Vector2Int origin)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        foreach (Vector2Int dir in directions)
        {
            Vector2Int cur = origin + dir;
            while (IsValidPosition(cur) && !IsPositionOccupied(cur))
            {
                moves.Add(cur);
                if (cur == player.position) break; // 到达玩家即可停止沿该方向继续
                cur += dir;
            }
        }
        return moves;
    }

    /// <summary>
    /// 供外部 (例如可视化) 查询所有可行位置。
    /// </summary>
    public override List<Vector2Int> CalculatePossibleMoves()
        => GetLegalMovesFrom(position);

    public override GameObject GetPrefab()
        => Resources.Load<GameObject>("Prefabs/Monster/DarkRook");
}
