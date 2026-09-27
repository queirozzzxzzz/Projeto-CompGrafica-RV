using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Painel de leitura: mostra título + conteúdo de uma nota/carta na tela,
// travando o movimento do player e liberando o cursor enquanto está aberto.
// Singleton simples — coloque em um Canvas na cena (não precisa persistir
// entre cenas, a não ser que você queira; se cada level tiver sua própria
// instância, tudo bem também).
public class ReadingUI : MonoBehaviour
{
    public static ReadingUI Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Referência")]
    [SerializeField] private PlayerController player;

    private bool isOpen;

    private void Awake()
    {
        Instance = this;
        panelRoot.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen) return;

        // Fecha a leitura com E ou Esc.
        if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Show(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
        panelRoot.SetActive(true);
        isOpen = true;

        if (player != null) player.InputLocked = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        isOpen = false;

        if (player != null) player.InputLocked = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}