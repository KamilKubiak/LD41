using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticles : MonoBehaviour {

    [SerializeField]
    ParticleSystem particles;
	
	// Update is called once per frame
	void Update () {
        if (!particles.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
