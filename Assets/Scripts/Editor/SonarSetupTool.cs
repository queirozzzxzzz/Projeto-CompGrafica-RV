using UnityEngine;
using UnityEditor;

// Ferramenta de editor: aplica um material e/ou uma layer em todos os
// Renderers filhos do(s) objeto(s) selecionado(s) de uma vez.
//
// Uso: selecione o objeto raiz do prefab da casa na Hierarchy,
// vá em Tools > Sonar > Configurar Objetos Selecionados,
// escolha o material e a layer desejados, e clique em Aplicar.
public class SonarSetupTool : EditorWindow
{
    private Material sonarMaterial;
    private bool applyMaterial = true;

    private int targetLayer;
    private bool applyLayer = true;

    private bool includeInactive = true;

    [MenuItem("Tools/Sonar/Configurar Objetos Selecionados")]
    private static void OpenWindow()
    {
        GetWindow<SonarSetupTool>("Configurar Sonar");
    }

    private void OnGUI()
    {
        GUILayout.Label("Aplica material e/ou layer em todos os Renderers\nfilhos dos objetos selecionados na Hierarchy.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        applyMaterial = EditorGUILayout.Toggle("Aplicar Material", applyMaterial);
        using (new EditorGUI.DisabledScope(!applyMaterial))
        {
            sonarMaterial = (Material)EditorGUILayout.ObjectField("Material do Sonar", sonarMaterial, typeof(Material), false);
        }

        EditorGUILayout.Space();

        applyLayer = EditorGUILayout.Toggle("Aplicar Layer", applyLayer);
        using (new EditorGUI.DisabledScope(!applyLayer))
        {
            targetLayer = EditorGUILayout.LayerField("Layer Alvo", targetLayer);
        }

        EditorGUILayout.Space();
        includeInactive = EditorGUILayout.Toggle("Incluir objetos inativos", includeInactive);

        EditorGUILayout.Space();

        if (GUILayout.Button("Aplicar aos Objetos Selecionados", GUILayout.Height(30)))
        {
            AplicarConfiguracao();
        }

        EditorGUILayout.HelpBox(
            "Selecione o(s) objeto(s) raiz na Hierarchy antes de clicar em Aplicar.\n" +
            $"Objetos selecionados: {Selection.gameObjects.Length}",
            MessageType.Info);
    }

    private void AplicarConfiguracao()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("Nenhum objeto selecionado. Selecione o(s) objeto(s) raiz na Hierarchy primeiro.");
            return;
        }

        int totalRenderers = 0;
        int totalObjetos = 0;

        foreach (GameObject root in Selection.gameObjects)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(includeInactive);

            Undo.RegisterCompleteObjectUndo(root, "Configurar Sonar");

            foreach (Renderer rend in renderers)
            {
                if (applyMaterial && sonarMaterial != null)
                {
                    Undo.RecordObject(rend, "Aplicar Material Sonar");

                    // Preserva o número de slots de material existentes,
                    // só troca todos para o material do sonar.
                    Material[] mats = new Material[rend.sharedMaterials.Length];
                    for (int i = 0; i < mats.Length; i++)
                        mats[i] = sonarMaterial;

                    rend.sharedMaterials = mats;
                }

                if (applyLayer)
                {
                    Undo.RecordObject(rend.gameObject, "Aplicar Layer Sonar");
                    rend.gameObject.layer = targetLayer;
                }

                totalRenderers++;
            }

            totalObjetos++;
        }

        Debug.Log($"[SonarSetupTool] Configuração aplicada em {totalRenderers} Renderer(s) dentro de {totalObjetos} objeto(s) selecionado(s).");
    }
}