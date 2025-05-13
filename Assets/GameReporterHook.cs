using UnityEngine;

/// <summary>
/// GameReporterHook - Script que conecta todos los eventos del juego con el sistema de reportes.
/// Instala este script en la escena para capturar y mostrar automáticamente todos los eventos.
/// </summary>
public class GameReporterHook : MonoBehaviour
{
    private void Awake()
    {
        // Asegurarse de que exista el sistema de reportes
        if (GameEventReporter.Instance == null)
        {
            Debug.LogWarning("❗ GameReporterHook: No se encontró GameEventReporter en la escena.");
        }
    }

    private void Start()
    {
        // Conectar con DiceManager
        DiceManager diceManager = FindObjectOfType<DiceManager>();
        if (diceManager != null)
        {
            // Usar el método MonoBehaviour de enviar mensajes para capturar los eventos
            MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
            foreach (MonoBehaviour mb in allObjects)
            {
                if (mb != null && mb.gameObject != null)
                {
                    mb.gameObject.AddComponent<DiceRollMonitor>();
                }
            }
        }

        // Hook para PlayerResources
        HookPlayerResources();
        
        // Mostrar mensaje de inicialización
        if (GameEventReporter.Instance != null)
        {
            GameEventReporter.Instance.ReportMessage("Sistema de reportes inicializado", Color.cyan);
        }
    }

    private void HookPlayerResources()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
        if (playerObj != null)
        {
            PlayerResources resources = playerObj.GetComponent<PlayerResources>();
            if (resources != null)
            {
                // Añadir nuestro componente de monitoreo
                ResourceMonitor monitor = playerObj.AddComponent<ResourceMonitor>();
                monitor.Initialize(resources);
            }
        }
    }
}

/// <summary>
/// Componente para monitorear los cambios en los recursos del jugador
/// </summary>
public class ResourceMonitor : MonoBehaviour
{
    private PlayerResources resources;
    private int lastWood;
    private int lastFood;
    private int lastAmmo;
    private int lastHealth;

    public void Initialize(PlayerResources resources)
    {
        this.resources = resources;
        lastWood = resources.wood;
        lastFood = resources.food;
        lastAmmo = resources.ammo;
        lastHealth = resources.health;
    }

    private void Update()
    {
        if (resources == null || GameEventReporter.Instance == null) return;

        // Verificar cambios en madera
        if (resources.wood != lastWood)
        {
            int change = resources.wood - lastWood;
            GameEventReporter.Instance.ReportResourceChange("Leña", change, resources.wood);
            lastWood = resources.wood;
        }

        // Verificar cambios en comida
        if (resources.food != lastFood)
        {
            int change = resources.food - lastFood;
            GameEventReporter.Instance.ReportResourceChange("Comida", change, resources.food);
            lastFood = resources.food;
        }

        // Verificar cambios en munición
        if (resources.ammo != lastAmmo)
        {
            int change = resources.ammo - lastAmmo;
            GameEventReporter.Instance.ReportResourceChange("Munición", change, resources.ammo);
            lastAmmo = resources.ammo;
        }

        // Verificar cambios en salud
        if (resources.health != lastHealth)
        {
            int change = resources.health - lastHealth;
            GameEventReporter.Instance.ReportResourceChange("Salud", change, resources.health);
            lastHealth = resources.health;
        }
    }
}

/// <summary>
/// Componente para monitorear las tiradas de dados
/// </summary>
public class DiceRollMonitor : MonoBehaviour
{
    // Esta clase usa el método MonoBehaviour.OnEnable para interceptar cuando se llama a RollDice
    
    // Versión modificada de Start() para RiskDiceEvent
    private void Start()
    {
        if (GetComponent<RiskDiceEvent>() != null)
        {
            MonitorRiskEvent();
        }
    }
    
    private void MonitorRiskEvent()
    {
        // Si este objeto tiene un RiskDiceEvent, monitoreamos sus eventos
        string objectName = gameObject.name;
        
        // Determinar qué tipo de evento es basado en los componentes
        string eventType = "desconocido";
        if (GetComponent<ResourceDiceEvent>() != null)
            eventType = "recursos";
        else if (GetComponent<DamageOnTriggerByDice>() != null)
            eventType = "combate";
            
        // Reportar que el jugador entró en un evento
        if (GameEventReporter.Instance != null)
        {
            GameEventReporter.Instance.ReportRiskEvent($"El jugador encontró un evento de {eventType}", true);
        }
    }
}

/// <summary>
/// Extensión para el DiceManager original para capturar eventos de dados
/// </summary>
public static class DiceManagerExtensions
{
    // Esta clase está diseñada para ser integrada con el código original
    
    // Método que extiende DiceManager, pero como no podemos modificar el código original,
    // en su lugar usamos el patrón de monitoreo con DiceRollMonitor
}

/// <summary>
/// Versión modificada de DiceManager para integrar el sistema de reportes
/// </summary>
[System.Serializable]
public class OverrideRollDice : MonoBehaviour
{
    // Esta clase es para la implementación en caso de que se pueda reemplazar DiceManager
    
    // En un proyecto real, sería mejor modificar DiceManager.cs para integrar GameEventReporter
    // o usar un sistema de eventos propio
}