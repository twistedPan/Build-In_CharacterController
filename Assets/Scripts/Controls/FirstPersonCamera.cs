using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform lookAt;
    private Player_FP playerSc;

    [Tooltip("Higher value is more sensitive.")]
    [SerializeField] private Vector2 camSensitivity = new Vector2(3,2);

    private float distance = 0.01f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float playerHeight;

    private void Start()
    {
        playerSc = lookAt.GetComponent<Player_FP>();
        playerHeight = playerSc.playerHeight;
    }

    private void LateUpdate() 
    {
        Vector3 playerHead = new Vector3(lookAt.position.x, (lookAt.position.y-1) + playerHeight, lookAt.position.z);
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

}
