using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackGreenSpider : EnemyAttackSpider
{
    public override void DealBiteDamage()
    {
        if (target == null || currentAttackZone == null) return;

        GameObject playerObj = currentAttackZone.PlayerObject;
        if (playerObj != null && currentAttackZone.GetComponent<Collider2D>().OverlapPoint(target.position))
        {
            playerObj.GetComponent<IHealth>()?.ApplyDamage(damage);
            playerObj.GetComponent<IRoting>()?.ApplyRot(15f, 3f, 4);
        }

        if (currentAttackZone.PlayerObject != null)
        {
            var playerHealth = currentAttackZone.PlayerObject.GetComponent<IHealth>();
            if (playerHealth.IsDead)
            {
                currentAttackZone.NotInside();
            }
        }
    }
}
