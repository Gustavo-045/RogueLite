using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Extensiones para RiskDiceEvent para integrar con el sistema de reportes
/// </summary>
public class RiskDiceEventReporter : MonoBehaviour
{
    private RiskDiceEvent riskEvent;
    
    private void Awake()
    {
        riskEvent = GetComponent<RiskDiceEvent>();
        if (riskEvent == null)
        {
            Debug.LogError("❌ RiskDiceEventReporter debe agregarse a un objeto con RiskDiceEvent");
            Destroy(this);
            return;
        }
        
        // Reemplazar método Original_AskPlayerChoice por nuestro método extendido
        //PatchRiskDiceEventButtons();
    }
    
    /// <summary>
    /// Parcha los botones en el panel de decisión para reportar eventos cuando son presionados
    /// </summary>
    /*private void PatchRiskDiceEventButtons()
    {
        // Encontrar el RiskDiceUIManager
        RiskDiceUIManager uiManager = FindObjectOfType<RiskDiceUIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("❗ No se pudo encontrar RiskDiceUIManager para reportar eventos de decisión");
            return;
        }
        
        // Guardar las referencias a los botones originales
        Button riskyButton = uiManager.buttonRisky;
        Button fleeButton = uiManager.buttonFlee;
        
        if (riskyButton != null)
        {
            // Guardar los listeners originales
            Button.ButtonClickedEvent originalRiskyClickEvent = new Button.ButtonClickedEvent();
            foreach (var handler in riskyButton.onClick.GetPersistentEventCount())
            {
                originalRiskyClickEvent.AddListener(riskyButton.onClick.GetPersistentMethodName(handler));
            }
            
            // Limpiar y agregar nuestro listener primero
            riskyButton.onClick.RemoveAllListeners();
            riskyButton.onClick.AddListener(() => {
                if (GameEventReporter.Instance != null)
                {
                    GameEventReporter.Instance.ReportRiskEvent("Jugador eligió modo ARRIESGADO", true);
                }
                
                // Llamar a todos los listeners originales
                originalRiskyClickEvent.Invoke();
            });
        }
        
        if (fleeButton != null)
        {
            // Guardar los listeners originales
            Button.ButtonClickedEvent originalFleeClickEvent = new Button.ButtonClickedEvent();
            var handler = fleeButton.onClick.GetPersistentEventCount();
            originalFleeClickEvent.AddListener(fleeButton.onClick.GetPersistentMethodName(handler));
            
            
            // Limpiar y agregar nuestro listener primero
            fleeButton.onClick.RemoveAllListeners();
            fleeButton.onClick.AddListener(() => {
                if (GameEventReporter.Instance != null)
                {
                    GameEventReporter.Instance.ReportRiskEvent("Jugador eligió HUIR", false);
                }
                
                // Llamar a todos los listeners originales
                originalFleeClickEvent.Invoke();
            });
        }
    }*/
}

/// <summary>
/// Extensión para monitorear movimientos del jugador
/// </summary>
public class PlayerMovementReporter : MonoBehaviour
{
    private PlayerController playerController;
    private Vector3 lastPosition;
    private float updateInterval = 0.5f; // Checar cada medio segundo
    private float lastUpdateTime;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerMovementReporter debe agregarse al objeto del jugador");
            Destroy(this);
            return;
        }
        
        lastPosition = transform.position;
        lastUpdateTime = Time.time;
    }
    
    void Update()
    {
        // Solo verificar periódicamente para no sobrecargar
        if (Time.time - lastUpdateTime < updateInterval)
            return;
            
        lastUpdateTime = Time.time;
        
        // Verificar si el jugador se ha movido
        if (Vector3.Distance(lastPosition, transform.position) > 0.1f)
        {
            // El jugador se movió
            Vector3 direction = transform.position - lastPosition;
            string directionStr = GetDirectionString(direction);
            
            if (GameEventReporter.Instance != null)
            {
                GameEventReporter.Instance.ReportMessage($"Movimiento: {directionStr}", Color.cyan);
            }
            
            lastPosition = transform.position;
        }
    }
    
    private string GetDirectionString(Vector3 direction)
    {
        // Determinar la dirección principal
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? "→ Este" : "← Oeste";
        }
        else
        {
            return direction.y > 0 ? "↑ Norte" : "↓ Sur";
        }
    }
}

/// <summary>
/// Script para agregar fácilmente todos los reporteros necesarios
/// </summary>
public class AddAllReporters : MonoBehaviour
{
    private void Start()
    {
        // 1. Buscar al jugador y agregar reportero de movimiento
        GameObject player = GameObject.FindGameObjectWithTag("Player1");
        if (player != null && player.GetComponent<PlayerMovementReporter>() == null)
        {
            player.AddComponent<PlayerMovementReporter>();
        }
        
        // 2. Buscar todos los eventos de riesgo y agregar reporteros
        RiskDiceEvent[] riskEvents = FindObjectsOfType<RiskDiceEvent>();
        foreach (RiskDiceEvent riskEvent in riskEvents)
        {
            if (riskEvent.gameObject.GetComponent<RiskDiceEventReporter>() == null)
            {
                riskEvent.gameObject.AddComponent<RiskDiceEventReporter>();
            }
        }
        
        // 3. Buscar UI manager de dados y asegurarse de que reporta eventos
        DiceUIManager diceUI = FindObjectOfType<DiceUIManager>();
        if (diceUI != null && diceUI.GetComponent<DiceUIReporter>() == null)
        {
            diceUI.gameObject.AddComponent<DiceUIReporter>();
        }
        
        // Registrar que se agregaron todos los reporteros
        if (GameEventReporter.Instance != null)
        {
            GameEventReporter.Instance.ReportMessage("Sistema de reportes conectado a todos los componentes del juego", Color.green);
        }
        
        // Auto-destruirse después de la configuración
        Destroy(this);
    }
}

/// <summary>
/// Clase para interceptar eventos de UI de dados
/// </summary>
public class DiceUIReporter : MonoBehaviour
{
    private DiceUIManager diceUIManager;
    
    private void Awake()
    {
        diceUIManager = GetComponent<DiceUIManager>();
        if (diceUIManager == null)
        {
            Debug.LogError("❌ DiceUIReporter debe agregarse al objeto con DiceUIManager");
            Destroy(this);
            return;
        }
        
        // Aquí podríamos parchear DiceUIManager.ShowDiceResult si fuera necesario
        // pero ya estamos detectando las tiradas de dados a través de los logs
    }
}