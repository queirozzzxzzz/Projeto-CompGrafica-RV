// Interface que todo objeto interagível do jogo deve implementar:
// itens para pegar, notas/textos para ler, painéis para acionar diálogo, etc.
public interface IInteractable
{
    // Texto curto mostrado na UI quando a mira aponta pro objeto.
    // Ex.: "Pegar chave", "Ler carta", "Falar"
    string GetPrompt();

    // Chamado quando o player aperta a tecla de interagir com a mira em cima do objeto.
    void Interact();
}