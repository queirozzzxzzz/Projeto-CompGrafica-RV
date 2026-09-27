using UnityEngine;

// Uma nota, carta ou anotação que pode ser lida, mostrada via ReadingUI.
public class ReadableInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string readableName = "Nota";
    [TextArea(3, 10)]
    [SerializeField] private string content;

    public string GetPrompt() => $"Ler {readableName}";

    public void Interact()
    {
        if (ReadingUI.Instance == null)
        {
            Debug.LogWarning("[ReadableInteractable] Nenhuma ReadingUI encontrada na cena.");
            return;
        }

        ReadingUI.Instance.Show(readableName, content);
    }
}