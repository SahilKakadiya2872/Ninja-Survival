using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeathByFalling : MonoBehaviour
{
    [SerializeField] float fallingDistanceThreshold = 1f;

    Animator anim;
    bool isTriggered;
    float startPos;
    float endPos;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if (anim.GetBool("OnGround") == false) 
        {
            if (!isTriggered) 
            {
                startPos = transform.position.y;
                StartCoroutine("FallingDelay");
                isTriggered = !isTriggered;        
           }
        }
    }

    // using coroutine because we need to wait while character falls
    IEnumerator FallingDelay()
    {
        // get start Y
        startPos = transform.position.y;

        // wait until character is back on ground
        while (anim.GetBool("OnGround") == false) yield return null;

        // get end Y
        endPos = transform.position.y;

        float yDelta = startPos - endPos;
        
        if (yDelta > fallingDistanceThreshold) 
        {
            anim.SetTrigger("Death");
        } else
        {
            // reset gate bool to try again
            isTriggered = !isTriggered;
        }
    }

}
