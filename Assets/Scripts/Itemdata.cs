using UnityEngine;

// Representa os dados de um item que pode entrar no inventário
// (uma chave, uma peça, uma ferramenta, etc).
// Crie um asset via botão direito > Create > Lightning Bug > Item
// para cada item do jogo, preenchendo os campos no Inspector.
[CreateAssetMenu(fileName = "NovoItem", menuName = "Lightning Bug/Item")]
public class ItemData : ScriptableObject
{
    public string itemId;       // identificador único, ex.: "chave_porao"
    public string displayName;  // nome mostrado na UI, ex.: "Chave do Porão"
    public Sprite icon;
    [TextArea(2, 5)]
    public string description;
}