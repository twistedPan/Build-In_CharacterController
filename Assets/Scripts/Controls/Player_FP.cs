using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FP : MonoBehaviour
{   
    [Space][Header("Walk & Sprint")]
    [SerializeField] private float maxWalkSpeed = 4.0f;
    [SerializeField] private float walkAcceleration = 5.0f;
    [SerializeField] private float maxSprintSpeed = 8.0f;
    [SerializeField] private float sprintAcceleration = 9.0f;
    private float speed = 0.0f;

    [Space][Header("Stuff")]
    public float playerHeight = 1.8f;

    [Space][Header("Step Cycle")]
    [SerializeField] private float runstepInterval = 0.6f;
    [SerializeField] private float stepInterval = 12.0f;
    private float stepCycle = 0.0f;
    private float nextStep = 0.0f;

    private PlayerController controls;
    private CharacterController controller;
    private Transform camTransform;
    private FirstPersonCamera firstPersonCamera;
    private Vector3 origin;
    private float playerDownVelocity;
    public Vector2 moveInput { get; private set; }

    private bool isGrounded = true;
    private bool isSprinting = false;
    private float death_Depht = -15;
    private float gravityValue = -9.81f;

    private void Awake() 
    {
        camTransform = Camera.main.transform;
        firstPersonCamera = camTransform.GetComponent<FirstPersonCamera>();
        controller = GetComponent<CharacterController>();

        origin = transform.position;

        //* Set Inputs
        controls = new PlayerController();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = new Vector2(0,0);
        controls.Player.Sprint.performed += _ => isSprinting = true;
        controls.Player.Sprint.canceled += _ => isSprinting = false;
        controls.Player.Look.started += ctx => FindObjectOfType<FirstPersonCamera>().Look(ctx.ReadValue<Vector2>());

        //* hide mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() 
    {
        Movement();

        //* if fall of map spawn at origin
        if (transform.position.y <= death_Depht) transform.position = origin;
    }


    void Movement()
    {
        Vector3 move = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
        Vector3 moveDir = new Vector3(0.0f, 0.0f, 0.0f);
        
        isGrounded = IsGrounded();

        //* calc movement speed and sprintSpeed
        float maxMovingSpeed = isSprinting ? maxSprintSpeed : maxWalkSpeed;                 //* set maximum speed
        float currentAcceleration = isSprinting ? sprintAcceleration : walkAcceleration;    //* set current acceleration

        //* calc rotation angle
        float targetAngle = Mathf.Atan2(move.x,move.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;

        //* Walk Direction & Orientation
        if (move.magnitude <= 0) //* -> stand still
        { 
            speed = 0; //* reset speed
        } 
        else if (move.magnitude >= 0.1f) //* -> move
        {
            moveDir = Quaternion.Euler(0f,targetAngle, 0f) * Vector3.forward;

            //* set start speed after full stop -> feels better
            if (speed < 1) speed = 1.5f; 
            else 
            {
                //* add acceleration to speed 
                if (speed < maxMovingSpeed) speed = speed + currentAcceleration * Time.deltaTime;
                //* clamp at current max speed
                speed = Mathf.Clamp(speed, 0, maxMovingSpeed);
            }
        } 

        //* 
        if (isGrounded && playerDownVelocity < 0) playerDownVelocity = 0f;

        //* set player rotation
        transform.rotation = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);

        //* apply movement
        controller.Move(moveDir * Time.deltaTime * speed);

        //* apply gravity
        playerDownVelocity += gravityValue * 4 * Time.deltaTime;
        controller.Move(new Vector3(0,playerDownVelocity,0) * Time.deltaTime);

        //* calc steps
        StepCycle(speed);
    }


    //* Ground Check
    bool IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, controller.height/2+0.1f);
        return hit.collider != null;
    }


    //* Step Simulation
    void StepCycle(float speed)
    {
        if (moveInput.magnitude > 0) 
        {
            stepCycle += (moveInput.magnitude + (speed*(isSprinting ? runstepInterval : 1f))/10)*
                             Time.fixedDeltaTime;
        }
        else if (stepCycle > 1)
        {
            stepCycle = 0;
            nextStep = 0;
        }

        float stepProgressAmount = (nextStep - stepCycle) / stepInterval;
        firstPersonCamera.HeadMove(stepProgressAmount, isSprinting);
        
        if (!(stepCycle > nextStep)) return;
        else
        {
            nextStep = stepCycle + stepInterval;
            PlayFootStepSound();
        }
    }

    //* Step Sound
    void PlayFootStepSound()
    {
        // Play Footstep Audio
    }

    
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}
