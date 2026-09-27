using UnityEngine;
using UnityEngine.InputSystem;

// Controla o pulso de sonar do celular usando um efeito de pós-processamento
// em tela cheia (SonarEdge.shader, via Full Screen Pass Renderer Feature).
// Em vez de trocar material objeto por objeto, esse script só envia
// propriedades globais de shader (origem, raio, quantidade, borrão) que o
// efeito de tela cheia lê para desenhar arestas + preenchimento cinza.
public class SonarController : MonoBehaviour
{
    [Header("Configuração do pulso")]
    [SerializeField] private float pulseRadius = 8f;
    [SerializeField] private float cooldownDuration = 3f;

    [Header("Decaimento do pulso")]
    [Tooltip("Quanto o pulso decai por segundo enquanto o player está parado.")]
    [SerializeField] private float idleDecayRate = 0.25f;
    [Tooltip("Decaimento extra por unidade de velocidade do player (esvai mais rápido andando).")]
    [SerializeField] private float movementDecayMultiplier = 0.6f;

    [Header("Referências")]
    [SerializeField] private Transform pulseOrigin; // normalmente a própria câmera do player
    [SerializeField] private PlayerController player; // para ler a velocidade atual

    private static readonly int SonarOriginID = Shader.PropertyToID("_SonarOrigin");
    private static readonly int SonarRadiusID = Shader.PropertyToID("_SonarRadius");
    private static readonly int SonarAmountID = Shader.PropertyToID("_SonarAmount");

    private float cooldownTimer;
    private bool isOnCooldown;

    private Vector3 revealOrigin;
    private float revealAmount; // 0 = totalmente escondido, 1 = recém revelado

    // Exposto para o sistema de detecção do invasor consultar
    // (cada pulso conta como "uso do sonar" para a chance cumulativa).
    public int PulseUseCount { get; private set; }

    public bool CanPulse => !isOnCooldown;

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                isOnCooldown = false;
        }

        // Input temporário: botão esquerdo do mouse dispara o pulso.
        // Depois isso deve ser chamado pela roda de ação, só quando
        // o celular estiver "equipado" na mão do player.
        if (Mouse.current.leftButton.wasPressedThisFrame && CanPulse)
        {
            Pulse();
        }

        DecayReveal();
        PushShaderGlobals();
    }

    // Chamado quando o player pega/re-equipa o celular — reinicia o cooldown,
    // conforme a regra de design (recarrega ao puxar o celular de novo).
    public void ResetCooldown()
    {
        isOnCooldown = false;
        cooldownTimer = 0f;
    }

    public void Pulse()
    {
        if (!CanPulse) return;

        isOnCooldown = true;
        cooldownTimer = cooldownDuration;
        PulseUseCount++;

        revealOrigin = pulseOrigin != null ? pulseOrigin.position : transform.position;
        revealAmount = 1f; // recém revelado — decai a partir daqui

        // TODO: aqui é o ponto certo para notificar o sistema de detecção
        // do invasor sobre mais um "uso do sonar" (chance cumulativa),
        // ex.: DetectionSystem.Instance.RegisterSonarUse();
    }

    private void DecayReveal()
    {
        if (revealAmount <= 0f) return;

        float speed = player != null ? player.CurrentSpeed : 0f;
        float decayRate = idleDecayRate + speed * movementDecayMultiplier;
        revealAmount = Mathf.Max(0f, revealAmount - decayRate * Time.deltaTime);
    }

    private void PushShaderGlobals()
    {
        Shader.SetGlobalVector(SonarOriginID, revealOrigin);
        Shader.SetGlobalFloat(SonarRadiusID, pulseRadius);
        Shader.SetGlobalFloat(SonarAmountID, revealAmount);
    }
}