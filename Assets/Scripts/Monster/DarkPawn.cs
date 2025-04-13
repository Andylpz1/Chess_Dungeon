using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DarkPawn：向玩家方向迈一步或对角吃子，使用轻量级 MCTS (30×3) 选取下一步。
/// 不实现首回合两步与 En‑Passant。
/// </summary>
public class DarkPawn : Monster
{
    private const int SIMULATIONS = 30;
    private const int DEPTH       = 3;

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "DarkPawn";
        type        = MonsterType.Pawn;
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;
        List<Vector2Int> legal = CalculatePossibleMoves();
        if (legal.Count == 0) return;

        Vector2Int bestMove = SelectByMcts(legal);
        position = bestMove;
        UpdatePosition();

        if (position == player.position)
            Debug.Log("DarkPawn attacks the player.");
    }

    #region MCTS helpers
    private Vector2Int SelectByMcts(List<Vector2Int> moves)
    {
        float bestScore = float.MaxValue;
        Vector2Int best = position;
        foreach (var m in moves)
        {
            float sum = 0f;
            for (int i = 0; i < SIMULATIONS; i++) sum += SimulatePlayout(m);
            float avg = sum / SIMULATIONS;
            if (avg < bestScore)
            {
                bestScore = avg;
                best = m;
            }
        }
        return best;
    }

    private float SimulatePlayout(Vector2Int start)
    {
        Vector2Int sim = start;
        int depth = 0;
        while (depth < DEPTH && sim != player.position)
        {
            var moves = GetMovesFrom(sim);
            if (moves.Count == 0) break;
            sim = moves[Random.Range(0, moves.Count)];
            depth++;
        }
        return Vector2Int.Distance(sim, player.position) + depth;
    }
    #endregion

    #region Move generation
    private List<Vector2Int> GetMovesFrom(Vector2Int origin)
    {
        List<Vector2Int> list = new();
        int forwardDir = (player.position.y > origin.y) ? 1 : -1; // 向玩家纵向前进
        Vector2Int forward = origin + new Vector2Int(0, forwardDir);
        if (IsValidPosition(forward) && !IsPositionOccupied(forward)) list.Add(forward);

        // 对角吃子
        Vector2Int leftDiag  = origin + new Vector2Int(-1, forwardDir);
        Vector2Int rightDiag = origin + new Vector2Int( 1, forwardDir);
        if (IsValidPosition(leftDiag)  && (!IsPositionOccupied(leftDiag)  || leftDiag  == player.position)) list.Add(leftDiag);
        if (IsValidPosition(rightDiag) && (!IsPositionOccupied(rightDiag) || rightDiag == player.position)) list.Add(rightDiag);

        return list;
    }

    public override List<Vector2Int> CalculatePossibleMoves() => GetMovesFrom(position);
    #endregion

    public override GameObject GetPrefab() => Resources.Load<GameObject>("Prefabs/Monster/DarkPawn");
}
