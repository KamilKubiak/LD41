using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField]
    protected SpriteRenderer render;
    [SerializeField]
    Sprite[] walkingAnimation;
    [SerializeField]
    int walkingFramerate = 10;
    [SerializeField]
    Sprite idleFrame;
    [SerializeField]
    Sprite[] fightingAnimation;
    [SerializeField]
    int attackingFramerate = 10;
    [SerializeField]
    GameObject BloodParticles;

    IEnumerator AnimateWalking()
    {
        
        float swapTime = 1f / walkingFramerate;
        float timer = 0.0f;
        int index = 0;
        while (true)
        {
            
            timer += Time.deltaTime;
            yield return null;
            if (timer >= swapTime)
            {
                index = index + 1 >= walkingAnimation.Length ? 0 : index + 1;
                render.sprite = walkingAnimation[index];
                timer = 0;
            }
            

        }
    }
    protected void StartAnimatingWalk()
    {
        StartCoroutine("AnimateWalking");
    }
    protected void StopWalkingAnimation()
    {
        Debug.Log("stopping Anim");
        StopCoroutine("AnimateWalking");
        render.sprite = idleFrame;
    }

    protected void StartAnimatingAttack()
    {
        StartCoroutine(AnimateAttack());
    }

    

    protected IEnumerator AnimateAttack()
    {
        float swapTime = 1f / attackingFramerate;
        float timer = 0.0f;
        int index = 0;
        
        while (index<fightingAnimation.Length)
        {
            render.sprite = fightingAnimation[index];
            timer += Time.deltaTime;
            Debug.Log("timer on Animation: " + timer + " swapping every " + swapTime);
            if (timer >= swapTime)
            {

                Debug.Log("new Anim: " + fightingAnimation[index]);
                index++;
                timer = 0;
            }
            yield return null;
        }
    }

    protected void SpawnBloodParticles(Vector2 hitVector)
    {
        Quaternion rotation = new Quaternion();
        Vector3 norTar = hitVector.normalized;
        float angle = Mathf.Atan2(norTar.y, norTar.x) * Mathf.Rad2Deg;
        rotation.eulerAngles = new Vector3(angle, 90, 0);

        Instantiate(BloodParticles, transform.position, rotation);

    }

}
