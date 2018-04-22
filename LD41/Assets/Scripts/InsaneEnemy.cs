using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsaneEnemy : RangedEnemy {

    public override IEnumerator ExecuteTurn()
    {
        bool endTurn = false;

        StartTurn();
        StartAnimatingWalk();
        while (!endTurn)
        {
            var posibleAreas = new List<WalkingArea>();
            int movementPoints = remainingMovementPoints;
            yield return StartCoroutine(CheckForAttack());
            if (!(movementPoints == remainingMovementPoints))
            {
                StartAnimatingWalk();
            }
            foreach (var tile in startingTile.neighbours)
            {
                if (!tile.IsSocketOccupied && tile.cost<=remainingMovementPoints)
                {
                    posibleAreas.Add(tile);
                }
            }
            endTurn = posibleAreas.Count == 0;
            
            if(!endTurn)
            {
                var nextStep = posibleAreas[Random.Range(0,posibleAreas.Count)];
                //Debug.Log("insane guy picked " + nextStep.name);
                SpendMovementPoints(map.costs[nextStep]);
                transform.localScale = new Vector3(Mathf.Sign(nextStep.CharacterSocket.position.x - transform.position.x), 1, 1);
                while ((transform.position - nextStep.CharacterSocket.position).sqrMagnitude > 0.003f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, nextStep.CharacterSocket.position, speed * Time.deltaTime);
                    yield return null;
                }
                startingTile = nextStep;
            }
            else
            {
                StopWalkingAnimation();
            }
            
            yield return null;

        }
    }

}
