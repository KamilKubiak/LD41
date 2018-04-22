using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NavigationScript  {

	public static List<WalkingArea> CreatePathToTarget(Dictionary<WalkingArea, WalkingArea> pathsTree, WalkingArea target)
    {
        
		var path = new List<WalkingArea> ();
        WalkingArea step = target;
        // check if a path exists
        if (pathsTree.ContainsKey(target))
        {
            path.Add(step);
            while (pathsTree.ContainsKey(step))
            {
                step = pathsTree[step];
                path.Add(step);
            }
        }
        return path;
    }

	public static NavigationMap CreatePathsTreeFromStart(WalkingArea start)
    {
        var tilesChecked = new HashSet<WalkingArea>();
        var toBeChecked = new List<WalkingArea>();
        var previousPoint = new Dictionary<WalkingArea, WalkingArea>();
        var travelCost = new Dictionary<WalkingArea, int>();
		travelCost.Add (start, 0);
          toBeChecked.Add(start);

        while (toBeChecked.Count > 0)
        {
            
            toBeChecked.Sort((x,y)=> travelCost[y]-travelCost[x]);
            WalkingArea current = toBeChecked.Last();
            toBeChecked.Remove(current);
            tilesChecked.Add(current);
            foreach (var next in current.neighbours)
            {
				//Debug.Log ("checking tile " + next.transform.name);
                if (!tilesChecked.Contains(next)) 
				{
                    var nextCost = travelCost[current] + next.cost;
                    if (travelCost.ContainsKey(next))
                    {
						//Debug.Log ("path to tile " + next.transform.name+ "already exists");
                        if (nextCost < travelCost[next])
                        {
							//Debug.Log ("path to tile " + next.transform.name+ "has lower cost");
                            travelCost[next] = nextCost;
                            previousPoint.Add(next, current);
                        }
                    }
                    else
                    {
						//Debug.Log ("path to tile " + next.transform.name+ "didn't exist, adding");
                        travelCost.Add(next, nextCost);
                        previousPoint.Add(next, current);
                    }
                    toBeChecked.Add(next);

                	}
            	}

            }
			var map = new NavigationMap (previousPoint, travelCost);
			return map;
        }

		
    }
