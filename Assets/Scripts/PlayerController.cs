using UnityEngine;
using UnityEngine.InputSystem;

// Controlador de movimentação em primeira pessoa para o Leonardo.
// Requer um CharacterController no mesmo GameObject.
// Usa o Input System novo (Keyboard/Mouse diretamente, sem precisar
// de um Input Actions asset customizado por enquanto).
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Câmera / Olhar")]
    [SerializeField] private Camera playerCamera; // arraste a Main Camera (filha do Player) aqui
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minLookAngle = -80f;
    [SerializeField] private float maxLookAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalLookRotation;

    // Trava de input: usado depois para os QTEs de escada,
    // onde a movimentação livre deve ser desativada.
    public bool InputLocked { get; set; } = false;

    // Velocidade horizontal atual (sem contar a queda/gravidade),
    // consultada pelo SonarController para decair o pulso mais rápido
    // enquanto o player está andando.
    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        // Trava e esconde o cursor — padrão para jogos em primeira pessoa.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (InputLocked)
            return;

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // A sensibilidade aqui é multiplicada por um fator menor porque
        // o delta do novo Input System já vem em pixels do frame,
        // diferente do Input.GetAxis antigo (que era normalizado).
        float mouseX = mouseDelta.x * mouseSensitivity * 0.1f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.1f;

        // Gira o corpo do player no eixo horizontal (Y).
        transform.Rotate(Vector3.up * mouseX);

        // Gira a câmera no eixo vertical (X), com limite para não virar de cabeça para baixo.
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.wKey.isPressed) input.y += 1f;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // Gravidade simples — mantém o player "grudado" no chão.
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Guarda só a velocidade horizontal (ignora a queda) para uso externo.
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;
        CurrentSpeed = horizontalVelocity.magnitude;
    }
}