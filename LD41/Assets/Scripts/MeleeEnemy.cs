using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeleeEnemy : EnemyScript
{
    [SerializeField]
    float yAttackRange = 0.8f;
    [SerializeField]
    float attackCooldown = 0.2f;

    protected override IEnumerator CheckForAttack()
    {
        bool canAttack = false;
        do
        {
            StopWalkingAnimation();
            canAttack = false;
            foreach (var item in startingTile.neighbours)
            {
                if (item.IsSocketOccupied)
                {
                    var player = item.ObjectInSocket.GetComponent<PlayerScript>();
                    if (player)
                    {
                        if (Mathf.Abs(player.transform.position.y - transform.position.y) < yAttackRange)
                            if (attackCost <= remainingMovementPoints)
                            {
                                var rot = player.transform.position.x - transform.position.x;
                                transform.localScale = new Vector3(Mathf.Sign(rot), 1, 1);
                                weaponAudioSource.PlayOneShot(weaponStrikeClips[Random.Range(0, weaponStrikeClips.Length)]);
                                canAttack = true;
                                Debug.Log("Melee Enemy Attacks");
                                SpendMovementPoints(attackCost);
                                PlayerScript.CallPlayerAttacked(damagePerHit,player.transform.position-transform.position);
                                yield return StartCoroutine(AnimateAttack());
                                yield return new WaitForSeconds(attackCooldown);
                            }
                    }
                }
            }
        } while (canAttack);
        StartAnimatingWalk();
    }


}
