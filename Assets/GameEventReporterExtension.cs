using UnityEngine;

/// <summary>
/// Extensiones para los scripts existentes para reportar eventos al GameEventReporter
/// </summary>
public static class GameReporterExtensions
{
    /// <summary>
    /// Parche para DiceManager.RollDice
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void PatchDiceManager()
    {
        // Esperar un frame para asegurarnos de que DiceManager ya esté inicializado
        GameObject patchObj = new GameObject("DiceManagerPatch");
        DiceManagerPatch patcher = patchObj.AddComponent<DiceManagerPatch>();
        GameObject.DontDestroyOnLoad(patchObj);
    }
}

/// <summary>
/// Componente para parchear DiceManager
/// </summary>
public class DiceManagerPatch : MonoBehaviour
{
    private bool patched = false;
    private DiceManager originalManager;

    private void Update()
    {
        if (!patched && DiceManager.Instance != null)
        {
            originalManager = DiceManager.Instance;
            PatchDiceManager();
            patched = true;
            
            // Ya no necesitamos actualizar
            enabled = false;
        }
    }

    private void PatchDiceManager()
    {
        Debug.Log("🔄 Parcheando DiceManager para reportar eventos de dados");
        
        // Crear un GameObject que servirá como interceptor
        GameObject interceptor = new GameObject("DiceRollInterceptor");
        interceptor.transform.parent = originalManager.transform;
        DiceRollInterceptor diceInterceptor = interceptor.AddComponent<DiceRollInterceptor>();
        
        // No podemos reemplazar el método original, pero podemos monitorear a través de eventos
        Debug.Log("✅ Sistema de intercepción de dados configurado");
    }
}

/// <summary>
/// Componente que intercepta las llamadas a RollDice
/// </summary>
public class DiceRollInterceptor : MonoBehaviour
{
    private void Awake()
    {
        // Suscribirse a un evento personalizado
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar memory leaks
        Application.logMessageReceived -= OnLogMessage;
    }

    private void OnLogMessage(string condition, string stacktrace, LogType type)
    {
        // Detectar mensajes de registro específicos de DiceManager
        if (condition.Contains("🎲 DiceManager: Lanzado un dado"))
        {
            try
            {
                // Parsear el mensaje para obtener la información
                string[] parts = condition.Split('.');
                string diceInfo = parts[0];
                
                // Extraer el número de caras y el resultado
                int sides = ExtractNumber(diceInfo, "caras");
                int result = ExtractNumber(diceInfo, "Resultado");
                
                if (sides > 0 && result > 0 && GameEventReporter.Instance != null)
                {
                    GameEventReporter.Instance.ReportDiceRoll(sides, result);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al procesar mensaje de dados: {e.Message}");
            }
        }
        
        // Capturar mensajes específicos de RiskDiceEvent
        CaptureRiskDiceEvents(condition);
    }
    
    private void CaptureRiskDiceEvents(string message)
    {
        // Detectar mensajes de eventos de riesgo
        if (message.Contains("Jugador entró en rango del evento de riesgo"))
        {
            if (GameEventReporter.Instance != null)
                GameEventReporter.Instance.ReportRiskEvent("Evento de riesgo detectado", true);
        }
        else if (message.Contains("Loot recibido"))
        {
            // Ejemplo: "🪵 Loot recibido: 10 de Madera"
            try {
                int amount = ExtractNumber(message, "recibido");
                string resourceType = ExtractResourceType(message);
                
                if (GameEventReporter.Instance != null && !string.IsNullOrEmpty(resourceType))
                    GameEventReporter.Instance.ReportRiskEvent($"Loot obtenido: {amount} de {resourceType}", true);
            }
            catch {
                // Si no se puede extraer específicamente, mostrar el mensaje general
                if (GameEventReporter.Instance != null)
                    GameEventReporter.Instance.ReportMessage(message);
            }
        }
        else if (message.Contains("recibiste") && message.Contains("daño"))
        {
            // Ejemplo: "💀 Fallaste con un 1, recibiste 15 de daño."
            try {
                int damage = ExtractNumber(message, "recibiste");
                
                if (GameEventReporter.Instance != null)
                    GameEventReporter.Instance.ReportRiskEvent($"Recibiste {damage} de daño", false);
            }
            catch {
                if (GameEventReporter.Instance != null)
                    GameEventReporter.Instance.ReportMessage(message);
            }
        }
        else if (message.Contains("perdiste") && message.Contains("munición"))
        {
            // Ejemplo: "🔫 Te salvaste del daño, pero perdiste 3 de munición por tirar un 3."
            try {
                int ammo = ExtractNumber(message, "perdiste");
                
                if (GameEventReporter.Instance != null)
                    GameEventReporter.Instance.ReportRiskEvent($"Perdiste {ammo} de munición", false);
            }
            catch {
                if (GameEventReporter.Instance != null)
                    GameEventReporter.Instance.ReportMessage(message);
            }
        }
    }

    private int ExtractNumber(string text, string keyword)
    {
        try
        {
            int keywordIndex = text.IndexOf(keyword);
            if (keywordIndex < 0) return -1;
            
            // Buscar números después de la palabra clave
            string remainder = text.Substring(keywordIndex + keyword.Length);
            string numberStr = "";
            bool foundDigit = false;
            
            foreach (char c in remainder)
            {
                if (char.IsDigit(c))
                {
                    numberStr += c;
                    foundDigit = true;
                }
                else if (foundDigit)
                {
                    // Si ya encontramos un dígito y ahora es otro carácter, rompemos
                    break;
                }
            }
            
            if (numberStr.Length > 0)
                return int.Parse(numberStr);
            return -1;
        }
        catch
        {
            return -1;
        }
    }
    
    private string ExtractResourceType(string message)
    {
        try
        {
            // Buscar "de Madera", "de Comida", etc.
            if (message.Contains("de Madera"))
                return "Madera";
            else if (message.Contains("de Comida"))
                return "Comida";
            else if (message.Contains("de Salud"))
                return "Salud";
            else if (message.Contains("de Munición"))
                return "Munición";
            else
                return "";
        }
        catch
        {
            return "";
        }
    }
}