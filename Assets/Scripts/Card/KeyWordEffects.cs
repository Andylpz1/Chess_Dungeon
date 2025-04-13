using UnityEngine;

namespace Effects
{
    public static class KeywordEffects
    {
        /// <summary>
        /// 根据玩家 targetAttackPosition 判断该格是否存在怪物，若存在返回该怪物，否则返回 null
        /// </summary>
        public static Monster GetMonsterAtPosition(Vector2Int pos)
        {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (GameObject monsterObject in monsters)
            {
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(pos))
                {
                    return monster;
                }
            }
            return null;
        }

        /// <summary>
        /// 将任意方向向量转换为最接近的四个正方向（上、下、左、右）
        /// </summary>
        public static Vector2 RoundToCardinal(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                return new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                return new Vector2(0, Mathf.Sign(direction.y));
            }
        }

        /// <summary>
        /// 对目标怪物应用击退效果：
        /// 尝试将其沿指定方向移动 1 格，
        /// 如果目标格超出棋盘范围或被阻挡，则对怪物造成 1 点伤害。
        /// </summary>
        /// <param name="target">目标怪物</param>
        /// <param name="direction">击退方向（应为 (1,0), (-1,0), (0,1) 或 (0,-1)）</param>
        /// <param name="player">玩家对象，用于判断目标位置是否合法</param>
        public static void ApplyKnockback(Monster target, Vector2 direction, Player player)
        {
            Vector2Int currentPos = target.position;
            Vector2Int knockbackDir = new Vector2Int((int)direction.x, (int)direction.y);
            Vector2Int desiredPos = currentPos + knockbackDir;
            
            if (IsPositionValid(desiredPos, player))
            {
                target.position = desiredPos;
                target.UpdatePosition();
            }
            else
            {
                target.TakeDamage(1);
            }
        }

        /// <summary>
        /// 判断目标位置是否有效：
        /// 1. 必须在棋盘范围内（player.IsValidPosition）
        /// 2. 该位置未被怪物或不可进入的地形阻挡（!player.IsBlockedBySomething）
        /// </summary>
        private static bool IsPositionValid(Vector2Int pos, Player player)
        {
            return player.IsValidPosition(pos) && !player.IsBlockedBySomething(pos);
        }

        /// <summary>
        /// 封装攻击并击退的完整效果：
        /// 1. 根据玩家的 targetAttackPosition 获取目标怪物
        /// 2. 计算击退方向（从玩家到怪物方向，再转换为上下左右方向）
        /// 3. 应用击退效果
        /// </summary>
        /// <param name="player">玩家对象，必须包含 targetAttackPosition、IsValidPosition、IsBlockedBySomething 等方法</param>
        public static void AttackWithKnockback(Player player)
        {
            // 根据玩家的攻击目标位置判断是否存在怪物
            Monster targetMonster = GetMonsterAtPosition(player.targetAttackPosition);
            if (targetMonster != null)
            {
                // 计算方向：从玩家到目标怪物
                Vector2 direction = (targetMonster.transform.position - player.transform.position).normalized;
                // 转换为卡尔迪纳方向
                Vector2 cardinalDirection = RoundToCardinal(direction);
                // 应用击退效果（尝试移动 1 格）
                ApplyKnockback(targetMonster, cardinalDirection, player);
            }
        }
    }
}
