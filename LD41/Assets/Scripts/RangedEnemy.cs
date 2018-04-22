using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RangedEnemy : EnemyScript
{
    int hitMask;
    [SerializeField]
    float shootingRange=8.5f;
    [SerializeField]
    Transform gunTransform;
    [SerializeField]
    LineRenderer bulletTrace;
    [SerializeField]
    float postHitRechargeTime;
    [SerializeField]
    float maxMissVariable = 0.08f;
    protected override void Start()
    {
        base.Start();
        hitMask = LayerMask.GetMask("Player", "Obstacles", "Ground", "Enemies");
    }

    Vector3 lastPosition;

    

    protected override IEnumerator CheckForAttack()
    {
        bool canAttack = false;
        do
        {
            canAttack = false;
            var toPlayerVector = PlayerScript.PlayerPosition - (Vector2)transform.position;
            var visionCheck = Physics2D.Raycast(gunTransform.position, toPlayerVector.normalized,shootingRange,hitMask);
            if (visionCheck)
            {
                var player = visionCheck.transform.GetComponent<PlayerScript>();
                if (player)
                {
                    var shootingVector = (PlayerScript.PlayerPosition + new Vector2(Random.Range(-maxMissVariable, maxMissVariable), Random.Range(-maxMissVariable, maxMissVariable)) - (Vector2)gunTransform.position).normalized;
                    if (attackCost <= remainingMovementPoints)
                    {
                        StopWalkingAnimation();
                        SpendMovementPoints(attackCost);
                        canAttack = true;
                        Debug.Log("Range Enemy Attacks");
                        var shotHit = Physics2D.Raycast(gunTransform.position, shootingVector, float.PositiveInfinity, hitMask);
                        var trace = new Vector3[2];
                        trace[0] = new Vector3(gunTransform.position.x, gunTransform.position.y, 0);
                        if (shotHit)
                        {
                            
                            trace[1] = new Vector3(shotHit.point.x, shotHit.point.y, 0);
                            bulletTrace.SetPositions(trace);
                            player = shotHit.transform.GetComponent<PlayerScript>();
                            if (player)
                            {
                                PlayerScript.CallPlayerAttacked(damagePerHit,shootingVector);
                            }
                        }
                        else
                        {
                            var missPoint = (Vector2)transform.position + shootingVector * 25;
                            trace[1] = trace[1] = new Vector3(missPoint.x, missPoint.y, 0);
                            bulletTrace.SetPositions(trace);

                        }
                        weaponAudioSource.PlayOneShot(weaponStrikeClips[Random.Range(0, injuryClips.Length)]);
                        StartCoroutine(DisableBulletTrace());
                        yield return new WaitForSeconds(postHitRechargeTime);
                    }
                }
            }
        } while (canAttack);
    }

    private IEnumerator DisableBulletTrace()
    {
        yield return new WaitForSeconds(0.06f);
        var trace = new Vector3[2];
        trace[0] = trace[1] = Vector3.zero;
        bulletTrace.SetPositions(trace);
    }
}
