using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controla a mira central (crosshair) e o texto de dica de interação
// que aparece embaixo dela quando há algo interagível na frente do player.
public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Cores")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow; // quando há algo interagível

    private void Awake()
    {
        HidePrompt();
    }

    public void ShowPrompt(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
            promptText.enabled = true;
        }

        if (crosshairImage != null)
            crosshairImage.color = activeColor;
    }

    public void HidePrompt()
    {
        if (promptText != null)
            promptText.enabled = false;

        if (crosshairImage != null)
            crosshairImage.color = idleColor;
    }
}