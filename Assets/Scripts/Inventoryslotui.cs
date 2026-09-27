using UnityEngine;
using UnityEngine.UI;

// Um slot individual da grade de inventário — só mostra o ícone do item.
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void Setup(ItemData item)
    {
        if (iconImage != null)
            iconImage.sprite = item.icon;
    }
}