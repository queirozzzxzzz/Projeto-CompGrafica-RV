using UnityEngine;
using UnityEngine.InputSystem;

// UI simples de inventário: um painel com slots de ícone, que abre/fecha
// com uma tecla e se atualiza sozinho sempre que o Inventory muda.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot; // o painel inteiro, ligado/desligado
    [SerializeField] private Transform slotsParent; // onde os slots instanciados vão
    [SerializeField] private InventorySlotUI slotPrefab;

    private bool isOpen;

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnInventoryChanged -= Refresh;
    }

    private void Start()
    {
        panelRoot.SetActive(false);
        Refresh();
    }

    private void Update()
    {
        // Tecla temporária para abrir/fechar o inventário — ajuste conforme
        // a roda de ação ou outro esquema de input que o jogo usar.
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;
            panelRoot.SetActive(isOpen);

            if (isOpen) Refresh();
        }
    }

    private void Refresh()
    {
        // Limpa os slots antigos.
        foreach (Transform child in slotsParent)
            Destroy(child.gameObject);

        if (Inventory.Instance == null) return;

        foreach (var item in Inventory.Instance.Items)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, slotsParent);
            slot.Setup(item);
        }
    }
}