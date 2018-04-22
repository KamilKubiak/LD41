
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public abstract class EnemyScript : Character
{
    protected WalkingArea startingTile;
    protected NavigationMap map;
    [SerializeField]
    protected AudioClip[] weaponStrikeClips;
    [SerializeField]
    protected AudioClip[] injuryClips;
    [SerializeField]
    protected AudioSource weaponAudioSource;
    [SerializeField]
    protected AudioSource injuryAudioSource;
    int groundMask;
    public virtual IEnumerator ExecuteTurn()
    {
        bool endTurn = false;

        if (nextWaypoint == null)
            nextWaypoint = waypoints.First();
        StartAnimatingWalk();
        StartTurn();
        var path = NavigationScript.CreatePathToTarget(map.paths, nextWaypoint);
        while (!endTurn)
        {
            int movementPoints = remainingMovementPoints;
            yield return StartCoroutine(CheckForAttack());
            if (!(movementPoints == remainingMovementPoints))
            {
                StartAnimatingWalk();
            }
            var nextStep = path.Last();
            if (nextStep.IsSocketOccupied)
            {
                endTurn = true;
            }
            else
            {
                path.Remove(nextStep);
                SpendMovementPoints(map.costs[nextStep]);
                transform.localScale = new Vector3(Mathf.Sign(nextStep.CharacterSocket.position.x - transform.position.x), 1, 1);
                while ((transform.position - nextStep.CharacterSocket.position).sqrMagnitude > 0.003f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextStep.CharacterSocket.position, speed * Time.deltaTime);
                    yield return null;
                }
                startingTile = nextStep;
            }
            
            if (nextWaypoint == startingTile)
            {
                nextWaypoint = nextWaypoint == waypoints.Last() ? waypoints[0] : waypoints[waypoints.IndexOf(nextWaypoint) + 1];
                map = NavigationScript.CreatePathsTreeFromStart(startingTile);
                path = NavigationScript.CreatePathToTarget(map.paths, nextWaypoint);
            }
            nextStep = path.Last();
            
            endTurn = endTurn?true:(remainingMovementPoints - nextStep.cost) < 0;
            if (endTurn)
            {
                StopWalkingAnimation();
            }
            yield return null;

        }
    }

    protected abstract IEnumerator CheckForAttack();

    protected virtual void Start()
    {

        var hit = Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("Ground"));
        if (hit)
        {
            //Debug.Log(hit.transform.name);
            startingTile = hit.transform.GetComponent<WalkingArea>();
            transform.position = startingTile.CharacterSocket.position;
        }
        currentHitPoints = startingHitPoints;
        RegisterEnemy();
    }

    private void RegisterEnemy()
    {
        TurnController.Instance.RegisterEnemy(this);
    }

    private void DeregisterEnemy()
    {
        TurnController.Instance.DeregisterEnemy(this);
    }

    [SerializeField]
    protected List<WalkingArea> waypoints;
    protected WalkingArea nextWaypoint;
    [SerializeField]
    protected int movementPointsPerTurn;
    [SerializeField]
    protected int attackCost;
    [SerializeField]
    protected int remainingMovementPoints;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int damagePerHit;
    [SerializeField]
    protected int startingHitPoints;
    protected int currentHitPoints;
    protected virtual void StartTurn()
    {
        map = NavigationScript.CreatePathsTreeFromStart(startingTile);
        remainingMovementPoints = movementPointsPerTurn;
    }
    protected void SpendMovementPoints(int amount)
    {
        //Debug.Log(remainingMovementPoints + " remaining points");
        remainingMovementPoints -= amount;
    }

    public void TakeDamage(int shotDamage)
    {
        currentHitPoints -= shotDamage;
        injuryAudioSource.PlayOneShot(injuryClips[Random.Range(0, injuryClips.Length)]);
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        DeregisterEnemy();
        Destroy(gameObject);
    }
}
