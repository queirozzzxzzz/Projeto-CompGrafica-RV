using UnityEngine;
using UnityEngine.SceneManagement;

// Fica no mesmo GameObject (ou perto) do Inventory/SceneTransitionManager
// na cena Bootstrap. Depois que os singletons persistentes já existem
// (Awake deles já rodou), carrega a cena de gameplay real.
//
// No Editor, se a ferramenta PlayFromBootstrap.cs estiver ativa, ela usa
// EditorPrefs para lembrar qual cena você estava editando e carrega ela
// automaticamente. Fora do Editor (build final), sempre usa a cena
// definida em 'fallbackSceneName'.
public class BootstrapLoader : MonoBehaviour
{
    [Tooltip("Cena carregada quando não há uma cena 'lembrada' do Editor (ou em builds finais).")]
    [SerializeField] private string fallbackSceneName = "SampleScene";

    private void Start()
    {
        string sceneToLoad = fallbackSceneName;

#if UNITY_EDITOR
        string remembered = UnityEditor.EditorPrefs.GetString(EditorSceneMemory.LastEditedScenePrefKey, "");
        if (!string.IsNullOrEmpty(remembered))
            sceneToLoad = remembered;
#endif

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}