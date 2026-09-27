using System;
using System.Collections.Generic;
using UnityEngine;

// Guarda os itens que o player coletou durante o jogo.
// Singleton simples — coloque este script em um GameObject persistente
// (idealmente o mesmo objeto/cena "Bootstrap" do SceneTransitionManager,
// já que os itens coletados precisam sobreviver às trocas de nível).
public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private readonly List<ItemData> items = new List<ItemData>();

    // Disparado sempre que um item é adicionado ou removido,
    // para a InventoryUI (ou qualquer outra coisa) se atualizar.
    public event Action OnInventoryChanged;

    public IReadOnlyList<ItemData> Items => items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ItemData item)
    {
        if (item == null) return;

        items.Add(item);
        Debug.Log($"[Inventory] Item adicionado: {item.displayName}");
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        return items.Exists(i => i.itemId == itemId);
    }

    public void RemoveItem(string itemId)
    {
        int index = items.FindIndex(i => i.itemId == itemId);
        if (index < 0) return;

        Debug.Log($"[Inventory] Item removido: {items[index].displayName}");
        items.RemoveAt(index);
        OnInventoryChanged?.Invoke();
    }
}