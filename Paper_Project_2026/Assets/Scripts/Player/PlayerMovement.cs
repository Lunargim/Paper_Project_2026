using System;
using FMOD.Studio;
using FMODUnity;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 0.3f;
    [SerializeField] private float verticalClamp = 80f;

    private PlayerInputActions inputActions;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool jumpPressed;
    private bool wasGrounded;
    
    private EventInstance playerFootsteps;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Start()
    {
        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
        RuntimeManager.AttachInstanceToGameObject(playerFootsteps, this.transform);
    
        playerFootsteps.getDescription(out var description);
        description.getPath(out var path);
        Debug.Log("Path effettivo: " + path);
    
        FMOD.RESULT result = playerFootsteps.start();
        Debug.Log("Test start diretto: " + result);
        playerFootsteps.stop(STOP_MODE.IMMEDIATE);   
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled  += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled  -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (controller.isGrounded)
            jumpPressed = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        CheckLanding();
        UpdateSound();
    }

    private void FixedUpdate()
    {
        UpdateSound();
    }

    private void HandleMovement()
    {
        
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = false;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.jump, this.transform.position);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void CheckLanding()
    {
        bool isGrounded = controller.isGrounded;

        if (!wasGrounded && isGrounded)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.jumpLanding, this.transform.position);
        }

        wasGrounded = isGrounded;
    }

    private void UpdateSound()
    {
        // start footsteps event if the player has an x velocity and is on the ground
        if ((moveInput.x != 0 || moveInput.y != 0) && controller.isGrounded)
        {
            // get the playback state
            playerFootsteps.getPlaybackState(out var playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        // otherwise, stop the footsteps event
        else 
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
