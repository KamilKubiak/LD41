using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMap {

	public Dictionary<WalkingArea,WalkingArea> paths { get; private set; }
	public Dictionary <WalkingArea,int> costs { get; private set; }



	public NavigationMap(Dictionary<WalkingArea,WalkingArea> paths, Dictionary <WalkingArea,int> costs)
    {
        this.paths = paths;
		this.costs = costs;
    }

}
