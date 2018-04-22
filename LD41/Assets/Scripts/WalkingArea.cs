using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WalkingArea : MonoBehaviour {

    public delegate void TileOperation(WalkingArea tile);
    public static event TileOperation TileClicked;
    public static event TileOperation TileHovered;
    public static event TileOperation TileCleared;
    public static void CallTileClicked(WalkingArea tile)
    {
        var handler = TileClicked;
        if (handler != null)
            handler(tile);
    }
    public static void CallTileHovered(WalkingArea tile)
    {
        var handler = TileHovered;
        if (handler != null)
            handler(tile);
    }
    public static void CallTileCleared(WalkingArea tile)
    {
        var handler = TileCleared;
        if (handler != null)
            handler(tile);
    }
    public GameObject ObjectInSocket { get; private set; }
    public bool IsSocketOccupied { get { return ObjectInSocket != null; } }
    public int cost;
    public SpriteRenderer rend;
	public List<WalkingArea> neighbours{ get; private set;}
	[System.NonSerialized]
	public int groundMask;

	public Transform CharacterSocket{ get{ return charcterSocket;}}
	[SerializeField]
	private Transform charcterSocket;
	public Color reachableColor;
    public Color unreachableColor;
    protected virtual void Awake()
    {
        groundMask = LayerMask.GetMask("Ground", "Ladders");
		neighbours = new List<WalkingArea> ();
		FindNeighbours();
		TileClicked+= OnTileClicked;
        TileCleared += ClearTileRender;
    }

    private void ClearTileRender(WalkingArea tile)
    {
        rend.color = Color.white;
    }

    protected virtual void OnTileClicked (WalkingArea tile)
    {
        return;

		if (tile == this) {
			rend.color = reachableColor;
		} else {
			rend.color = Color.white;
		}
    }

    private void FindNeighbours()
    {
        Vector2 size = rend.sprite.bounds.extents;

        foreach (var item in Physics2D.OverlapAreaAll((Vector2)transform.position-size-new Vector2(0.01f,0.01f), (Vector2)transform.position + size - new Vector2(0.01f, 0.01f),groundMask))
        {
            var obj = item.GetComponent<WalkingArea>();
            if (obj == this)
            {
                continue;
            }
            neighbours.Add(obj);
        }
    }

	protected virtual void OnMouseDown()
	{
		CallTileClicked (this);
	}

    protected virtual void OnMouseOver()
    {
        CallTileHovered(this);
    }

    protected virtual void OnMouseExit()
    {
        CallTileCleared(this);
    }

    public void TakeSocket(GameObject obj)
    {
        ObjectInSocket = obj;
    }

    public void LeaveSocket(GameObject obj)
    {
        if (ObjectInSocket == obj)
        {
            ObjectInSocket = null;
        }
    }

    
}
