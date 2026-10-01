#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ferramenta de Editor: sempre que você aperta Play em QUALQUER cena
// (ex.: SampleScene), redireciona automaticamente para a cena Bootstrap
// primeiro — garantindo que Inventory, SceneTransitionManager, etc.
// existam antes do resto do jogo rodar — e depois carrega de volta a
// cena que você estava editando.
//
// IMPORTANTE: ajuste BOOTSTRAP_SCENE_PATH abaixo para o caminho real
// da sua cena Bootstrap (ex.: "Assets/_Scenes/Bootstrap.unity").
[InitializeOnLoad]
public static class PlayFromBootstrap
{
    private const string BOOTSTRAP_SCENE_PATH = "Assets/Scenes/Bootstrap.unity";

    static PlayFromBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode) return;

        Scene activeScene = EditorSceneManager.GetActiveScene();

        // Se já está na Bootstrap, não faz nada — deixa o Play seguir normal.
        if (activeScene.path == BOOTSTRAP_SCENE_PATH) return;

        // Lembra o NOME (não o caminho) da cena que estava aberta,
        // para o BootstrapLoader carregar ela de volta em seguida.
        EditorPrefs.SetString(EditorSceneMemory.LastEditedScenePrefKey, activeScene.name);

        // Cancela essa tentativa de Play...
        EditorApplication.isPlaying = false;

        // ...abre a Bootstrap...
        EditorSceneManager.OpenScene(BOOTSTRAP_SCENE_PATH);

        // ...e dispara o Play de novo, agora com a Bootstrap ativa.
        EditorApplication.isPlaying = true;
    }
}
#endif