using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DarkKnight 采用轻量级 MCTS (30×3) 选择下一步位置。
/// Knight 可跳跃障碍，故不检测路径，仅检测落点合法性。
/// 评估 = 距离玩家 + 深度；越小越优。
/// </summary>
public class DarkKnight : Monster
{
    private static readonly Vector2Int[] knightMoves =
    {
        new Vector2Int( 2, 1), new Vector2Int( 1, 2), new Vector2Int(-1, 2), new Vector2Int(-2, 1),
        new Vector2Int(-2,-1), new Vector2Int(-1,-2), new Vector2Int( 1,-2), new Vector2Int( 2,-1)
    };

    private const int SIMULATIONS_PER_MOVE = 30;
    private const int MAX_PLAYOUT_DEPTH    = 3;

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "DarkKnight";
        type        = MonsterType.Knight;
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;
        List<Vector2Int> legal = CalculatePossibleMoves();
        if (legal.Count == 0) return;

        Vector2Int best = position;
        float      bestScore = float.MaxValue;

        foreach (var move in legal)
        {
            float sum = 0f;
            for (int i = 0; i < SIMULATIONS_PER_MOVE; i++)
                sum += Simulate(move);
            float avg = sum / SIMULATIONS_PER_MOVE;
            if (avg < bestScore)
            {
                bestScore = avg;
                best      = move;
            }
        }

        position = best;
        UpdatePosition();
        if (position == player.position)
        {
            Debug.Log("DarkKnight attacks the player.");
            // player.TakeDamage(1);
        }
    }

    private float Simulate(Vector2Int start)
    {
        Vector2Int sim = start;
        int depth = 0;
        while (depth < MAX_PLAYOUT_DEPTH && sim != player.position)
        {
            List<Vector2Int> moves = GetLegalMovesFrom(sim);
            if (moves.Count == 0) break;
            sim = moves[Random.Range(0, moves.Count)];
            depth++;
        }
        return Vector2Int.Distance(sim, player.position) + depth;
    }

    private List<Vector2Int> GetLegalMovesFrom(Vector2Int origin)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach (var m in knightMoves)
        {
            Vector2Int tgt = origin + m;
            if (IsValidPosition(tgt) && (!IsPositionOccupied(tgt) || tgt == player.position))
                list.Add(tgt);
        }
        return list;
    }

    public override List<Vector2Int> CalculatePossibleMoves() => GetLegalMovesFrom(position);
    public override GameObject GetPrefab() => Resources.Load<GameObject>("Prefabs/Monster/DarkKnight");
}
