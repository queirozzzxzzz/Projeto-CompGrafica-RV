using UnityEngine;
using UnityEngine.InputSystem;

// Fica na câmera do player. Todo frame, faz um raycast a partir do centro
// da tela (onde a mira/crosshair fica) para achar objetos interagíveis
// dentro de alcance, e mostra/atualiza a dica de interação na UI.
public class InteractionController : MonoBehaviour
{
    [Header("Configuração do raycast")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Referências de UI")]
    [SerializeField] private CrosshairUI crosshairUI; // ver script abaixo

    private Camera cam;
    private IInteractable currentTarget;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentTarget.Interact();
        }
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            // GetComponentInParent cobre o caso de o Collider estar num
            // objeto filho enquanto o script IInteractable está no pai.
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (interactable != currentTarget)
                {
                    currentTarget = interactable;
                    crosshairUI?.ShowPrompt(interactable.GetPrompt());
                }
                return;
            }
        }

        // Não achou nada interagível na mira — limpa o alvo atual.
        if (currentTarget != null)
        {
            currentTarget = null;
            crosshairUI?.HidePrompt();
        }
    }
}