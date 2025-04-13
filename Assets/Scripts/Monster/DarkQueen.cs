using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DarkQueen：8 方向无限格直线/对角移动，轻量级 MCTS (30×3)。
/// </summary>
public class DarkQueen : Monster
{
    private const int SIMULATIONS = 30;
    private const int DEPTH       = 3;

    private static readonly Vector2Int[] directions =
    {
        new( 1, 0), new(-1, 0), new(0, 1), new(0,-1),
        new( 1, 1), new( 1,-1), new(-1, 1), new(-1,-1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "DarkQueen";
        type        = MonsterType.Queen;
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;
        List<Vector2Int> legal = CalculatePossibleMoves();
        if (legal.Count == 0) return;

        Vector2Int best = SelectByMcts(legal);
        position = best;
        UpdatePosition();

        if (position == player.position)
            Debug.Log("DarkQueen attacks the player.");
    }

    #region MCTS helpers
    private Vector2Int SelectByMcts(List<Vector2Int> moves)
    {
        float bestScore = float.MaxValue;
        Vector2Int best = position;
        foreach (var m in moves)
        {
            float sum = 0f;
            for (int i = 0; i < SIMULATIONS; i++) sum += Simulate(m);
            float avg = sum / SIMULATIONS;
            if (avg < bestScore)
            {
                bestScore = avg;
                best = m;
            }
        }
        return best;
    }

    private float Simulate(Vector2Int start)
    {
        Vector2Int sim = start;
        int depth = 0;
        while (depth < DEPTH && sim != player.position)
        {
            var moves = MovesFrom(sim);
            if (moves.Count == 0) break;
            sim = moves[Random.Range(0, moves.Count)];
            depth++;
        }
        return Vector2Int.Distance(sim, player.position) + depth;
    }
    #endregion

    #region Move generation
    private List<Vector2Int> MovesFrom(Vector2Int origin)
    {
        List<Vector2Int> list = new();
        foreach (var dir in directions)
        {
            Vector2Int cur = origin + dir;
            while (IsValidPosition(cur) && !IsPositionOccupied(cur))
            {
                list.Add(cur);
                if (cur == player.position) break;
                cur += dir;
            }
        }
        return list;
    }

    public override List<Vector2Int> CalculatePossibleMoves() => MovesFrom(position);
    #endregion

    public override GameObject GetPrefab() => Resources.Load<GameObject>("Prefabs/Monster/DarkQueen");
}
