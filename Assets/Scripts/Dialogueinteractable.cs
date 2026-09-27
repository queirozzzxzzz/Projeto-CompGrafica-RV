using UnityEngine;

// Exemplo: um ponto de diálogo (ex.: um telefone tocando, uma gravação do avô).
// Depende de um sistema de diálogo (DialogueUI) que você ainda vai criar.
public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string dialogueId = "avo_gravacao_01";

    public string GetPrompt() => "Falar";

    public void Interact()
    {
        Debug.Log($"Iniciando diálogo: {dialogueId}");

        // TODO: chamar aqui algo como DialogueUI.Instance.StartDialogue(dialogueId);
    }
}