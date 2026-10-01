// Classe pequena, SEM estar dentro de uma pasta "Editor", para que tanto
// scripts de runtime (BootstrapLoader.cs) quanto scripts de editor
// (PlayFromBootstrap.cs) possam enxergar a mesma constante.
//
// Motivo técnico: scripts dentro de uma pasta "Editor" compilam num
// assembly separado (ex.: Assembly-CSharp-Editor), que o assembly
// principal do jogo (Assembly-CSharp) NÃO consegue referenciar — só o
// contrário é permitido. Por isso a constante precisa morar aqui fora.
public static class EditorSceneMemory
{
    public const string LastEditedScenePrefKey = "LightningBug_LastEditedSceneName";
}