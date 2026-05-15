using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _mouseSensitivity = 0.3f;
    [SerializeField] private float _verticalClamp = 80f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerInputActions _inputActions;
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private float _xRotation = 0f;
    private bool _jumpPressed;
    private bool _wasGrounded;
    
    public Action Jump;
    public Action Land;
    public Action<float,float,bool> Walk;    
    /*public Action Dash;
    public Action StartFalling;*/

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled  += OnMove;
        _inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled  -= OnMove;
        _inputActions.Player.Jump.performed -= OnJump;
        _inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
            _jumpPressed = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleLook();
        HandleLanding();
    }

    private void HandleJumping()
    {
        
        if (IsGrounded() && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (_jumpPressed)
        {
            _velocity.y = _jumpForce;
            _jumpPressed = false;
            Jump?.Invoke();
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _controller.Move(move * _speed * Time.deltaTime);
        Walk?.Invoke(_moveInput.x, _moveInput.y, IsGrounded());
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_verticalClamp, _verticalClamp);
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void HandleLanding()
    {
        bool isGrounded = IsGrounded();

        if (!_wasGrounded && isGrounded)
        {
            Land?.Invoke();
        }

        _wasGrounded = isGrounded;
    }
    
}
