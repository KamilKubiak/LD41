using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerScript : Character {
	
    public delegate void PlayerStatus(int damage, Vector2 hitVector);
    public static event PlayerStatus PlayerAttacked;
    public static Vector2 PlayerPosition { get; private set; }
	
    public static void CallPlayerAttacked(int damage, Vector2 hitVector)
    {
        var handler = PlayerAttacked;
        if (handler != null)
            handler(damage, hitVector);
    }



    [SerializeField]
	float speed = 5f;
	NavigationMap pathMap;
	[SerializeField]
	WalkingArea selectedTile;
	[SerializeField]
	WalkingArea startingTile;
	[SerializeField]
	int movementPointsPerTurn = 10;
    [SerializeField]
	int remainingMovementPoints;
    [SerializeField]
    int startingPlayerHealth = 100;
    [SerializeField]
    float crosshairDistance = 1f;
    [SerializeField]
    Transform crosshair;
    [SerializeField]
    LineRenderer bulletTrace;
    [SerializeField]
    Transform gunTransform;
    [SerializeField]
    int ShotCost;
    [SerializeField]
    int ShotDamage;
    [SerializeField]
    AudioClip[] weaponClips;
    [SerializeField]
    AudioClip[] injuryClips;
    [SerializeField]
    AudioClip[] footstepsClips;
    [SerializeField]
    AudioSource weaponAudioSource;
    [SerializeField]
    AudioSource injuryAudioSource;
    [SerializeField]
    AudioSource footAudioSource;
   


    bool died;
    new Camera camera;
    int groundMask;
    int hitMask;
    int playerHealth;
	bool playersTurn;
    
	// Use this for initialization
	void Awake () {
        
        died = false;
		WalkingArea.TileClicked+= OnNewTileClicked;
        PlayerAttacked += OnPlayerAttacked;
        groundMask = LayerMask.GetMask("Ground","Ladders");
        hitMask = LayerMask.GetMask("Ground", "Obstacles","Enemies");
		var hit = Physics2D.Raycast(transform.position, Vector2.down, 1, groundMask);
		if (hit)
		{
			Debug.Log(hit.transform.name);
			startingTile = hit.transform.GetComponent<WalkingArea> ();
			transform.position = startingTile.CharacterSocket.position;
		}
        playerHealth = startingPlayerHealth;
        UIController.Instance.SetPlayerHealthTextValue(playerHealth);
        camera = Camera.main;
        PlayerPosition = transform.position;
        WalkingArea.TileHovered += OnTileHovered;
        WalkingArea.TileCleared += OnTileCleared;
        var hp = PlayerPrefs.GetInt("hp");
        playerHealth = hp == 0 ? startingPlayerHealth : hp;
    }

    private void OnTileCleared(WalkingArea tile)
    {
        selectedTile = null;
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(4f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnTileHovered(WalkingArea tile)
    {
        if (!playersTurn || moving)
        {
            return;
        }

        if (selectedTile == null || selectedTile != tile)
        {
            selectedTile = tile;
            var path = NavigationScript.CreatePathToTarget(pathMap.paths, tile);
            int remainingMovement = remainingMovementPoints;
            for (int i = path.Count-2; i >=0 ; i--)
            {
                var step = path[i];
                remainingMovement -= step.cost;
                
                step.rend.color = remainingMovement >= 0 ? step.reachableColor : step.unreachableColor;
            }
            UIController.Instance.SetMovementCostTextValue(pathMap.costs[selectedTile]);
        }
    }

    private void OnPlayerAttacked(int damage, Vector2 hitVector)
    {
        playerHealth -= damage;
        if (!injuryAudioSource.isPlaying)
        {
            injuryAudioSource.PlayOneShot(injuryClips[Random.Range(0, injuryClips.Length)]);
            SpawnBloodParticles(hitVector);
        }
        
        if (playerHealth <= 0)
        {
            Die();
        }
        UIController.Instance.SetPlayerHealthTextValue(playerHealth);
    }

    private void Die()
    {
        render.enabled = false;
        StartCoroutine(RestartGame());
    }

    void OnNewTileClicked (WalkingArea tile)
	{
		if (!playersTurn||moving) {
			return;
		}
        if (tile != null && selectedTile == tile)
        {
            if (pathMap.costs[selectedTile] <= remainingMovementPoints)
            {
                MoveToCurrentTile();
                UseMovementPoints(pathMap.costs[selectedTile]);
            }
        }
	}

    void StartTurn()
    {
		playersTurn = true;
		remainingMovementPoints = movementPointsPerTurn;
		UIController.Instance.SetRemainingMovmentPointsTextValue (remainingMovementPoints);
		pathMap = NavigationScript.CreatePathsTreeFromStart(startingTile);
        
    }

	void MoveToCurrentTile()
	{
		if (selectedTile != startingTile) {
			StartCoroutine (PlayerMovement (selectedTile));
		}	
	}

	void UseMovementPoints(int cost)
	{
		remainingMovementPoints -= cost;
		UIController.Instance.SetRemainingMovmentPointsTextValue (remainingMovementPoints);
	}
    bool moving;
	IEnumerator PlayerMovement(WalkingArea selectedTile)
	{
        StartAnimatingWalk();
        moving = true;
		var path = NavigationScript.CreatePathToTarget (pathMap.paths, selectedTile);
		while(path.Count>0 && !died)
		{
			var nextStep = path.Last ();
			path.Remove (nextStep);
			while ((transform.position - nextStep.CharacterSocket.position).sqrMagnitude >0.003f) {
                if (!footAudioSource.isPlaying)
                {
                    footAudioSource.PlayOneShot(footstepsClips[Random.Range(0, footstepsClips.Length)]);
                }
                PlayerPosition = transform.position;
                transform.position = Vector2.MoveTowards (transform.position, nextStep.CharacterSocket.position, speed * Time.deltaTime);
				yield return null;
			}
		}
        
		startingTile = selectedTile;
        StopWalkingAnimation();

        pathMap = NavigationScript.CreatePathsTreeFromStart(startingTile);
        moving = false;
	}

	public IEnumerator ExecuteTurn()
	{
		bool endTurn = false;
		StartTurn();
        startingTile.LeaveSocket(gameObject);
		while (!endTurn&& !died) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				endTurn = true;
                startingTile.TakeSocket(gameObject);
                PlayerPosition = transform.position;
                playersTurn = false;
                WalkingArea.CallTileCleared(null);
            }
            else if (Input.GetMouseButtonDown(1)&&!moving&&remainingMovementPoints>=ShotCost)
            {

                UseMovementPoints(ShotCost);
                Vector2 crosshairVector = ((Vector2)(crosshair.position - gunTransform.position)).normalized;
                var shotHit = Physics2D.Raycast(gunTransform.position, crosshairVector, float.PositiveInfinity, hitMask);
                var trace = new Vector3[2];
                trace[0] = new Vector3(gunTransform.position.x, gunTransform.position.y,0);
                if (shotHit)
                {
                    trace[1] = new Vector3(shotHit.point.x, shotHit.point.y, 0);
                    bulletTrace.SetPositions(trace);
                    var enemy = shotHit.transform.GetComponent<EnemyScript>();
                    if (enemy)
                    {
                        Debug.Log("found Enemy");
                        enemy.TakeDamage(ShotDamage);
                    }
                }
                else
                {
                    var missPoint =(Vector2)transform.position+ crosshairVector*25;
                    trace[1] = trace[1] = new Vector3(missPoint.x, missPoint.y, 0);
                    bulletTrace.SetPositions(trace);
                    
                }
                weaponAudioSource.PlayOneShot(weaponClips[Random.Range(0, weaponClips.Length)]);
                StartCoroutine(DisableBulletTrace());
            }
			yield return null;
		}

        
	}

    private void OnDisable()
    {
        PlayerPrefs.SetInt("hp", playerHealth);
    }

    private IEnumerator DisableBulletTrace()
    {
        yield return new WaitForSeconds(0.06f);
        var trace = new Vector3[2];
        trace[0] = trace[1] = Vector3.zero;
        bulletTrace.SetPositions(trace);
    }

    protected void Update()
    {
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 crosshairVector = (mousePosition - (Vector2)transform.position).normalized;

        transform.localScale = new Vector3(Mathf.Sign(crosshairVector.x), 1, 1);
        crosshair.localPosition  =  new Vector2(Mathf.Abs(crosshairVector.x),crosshairVector.y) * crosshairDistance;
    }

   


}
