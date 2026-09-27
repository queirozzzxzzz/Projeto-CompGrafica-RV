using UnityEngine;

// Objeto que pode ser pego e vai para o inventário (ex.: uma chave, uma peça).
public class PickupInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData item;

    public string GetPrompt() => item != null ? $"Pegar {item.displayName}" : "Pegar item";

    public void Interact()
    {
        if (item == null)
        {
            Debug.LogWarning($"[PickupInteractable] Nenhum ItemData atribuído em {gameObject.name}.");
            return;
        }

        Inventory.Instance.AddItem(item);
        gameObject.SetActive(false); // remove o objeto da cena depois de pego
    }
}