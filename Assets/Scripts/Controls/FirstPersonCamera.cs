using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    private Player_FP playerSc;

    [Tooltip("Higher value is more sensitive.")]
    [SerializeField] private Vector2 camSensitivity = new Vector2(3,2);

    [SerializeField] private float headBobtAmt = 0.1f;
    
    private float distance = 0.01f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float playerHeight;
    private float headMovement = 0.0f;  


    private void Start()
    {
        playerSc = lookAt.GetComponent<Player_FP>();
        playerHeight = playerSc.playerHeight;
    }

    private void LateUpdate() 
    {
        Vector3 playerHead = new Vector3(lookAt.position.x, (lookAt.position.y-1) + playerHeight + headMovement, lookAt.position.z);
        Vector3 dir = new Vector3(0,0,-distance);

        //* Clamp Up/Down rotation
        currentX = Mathf.Clamp(currentX, -85, 85);
        Quaternion camRotation = Quaternion.Euler(currentX,currentY,0);
        
        // Debug.Log("Cam rot x/y: " + currentX +" / "+ currentY);
        
        transform.position = playerHead + camRotation * dir;
        transform.LookAt(playerHead);
    }


    /// <summary>
    /// Adds Mouse Delta to X/Y Values
    /// </summary>
    /// <param name="v">Vector2 - mouse delta</param>
    public void Look(Vector2 v) 
    {
        //Debug.Log("Look: " + v);
        currentX -= v.y * (1/camSensitivity.y);
        currentY += v.x * (1/camSensitivity.x);
    }


    /// <summary>
    /// Calculates an up and down movement
    /// </summary>
    /// <param name="amount">float - </param>
    /// <param name="sprinting">bool - </param>
    public void HeadMove(float amount, bool sprinting) 
    {
        amount = Mathf.Clamp(amount, 0,1);

        float stepLerp = MapRange(amount, 0,1, Mathf.PI,Mathf.PI*2);
        float sinMove = Mathf.Sin(stepLerp); 
        float headBob = sprinting ? 0.25f : headBobtAmt;
        float camLerp = MapRange(sinMove, -1,1, -headBob,headBob);

        headMovement = Mathf.Sin(camLerp);
    }


    // Create range from value n which acts in range start1 to stop1 to new range
    private float MapRange(float n, float start1, float stop1, float start2, float stop2) 
    {
        float newval = (n - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        //if (newval != ) {return newval;}
        if (start2 < stop2) 
        {
            return Mathf.Clamp(newval, start2, stop2);
        } 
        else 
        {
            return Mathf.Clamp(newval, stop2, start2);
        }
    }
}
