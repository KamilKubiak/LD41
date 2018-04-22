using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    [SerializeField]
    float damping;
    private void Start()
    {
        transform.position = new Vector3(PlayerScript.PlayerPosition.x, PlayerScript.PlayerPosition.y, transform.position.z);
    }
    // Update is called once per frame
    void LateUpdate () {
        transform.position = Vector3.Lerp(transform.position, new Vector3(PlayerScript.PlayerPosition.x, PlayerScript.PlayerPosition.y, transform.position.z), damping*Time.deltaTime);
        
	}
}
