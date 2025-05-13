using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// GameEventReporterInitializer - Script para inicializar el sistema de reportes de eventos.
/// Agrega este script a un objeto en la escena para configurar automáticamente el sistema.
/// </summary>
public class GameEventReporterInitializer : MonoBehaviour
{
    [Header("Referencia prefab")]
    public GameObject eventReporterPrefab;  // Prefab que contiene el GameEventReporter (opcional)
    
    [Header("Si no hay prefab, configuración manual")]
    public Canvas targetCanvas;              // Canvas donde se crearán los elementos UI
    public TMP_FontAsset messageFont;        // Fuente para los mensajes
    public AudioClip notificationSound;      // Sonido para las notificaciones
    
    void Awake()
    {
        // Comprobar si ya existe un GameEventReporter
        if (GameEventReporter.Instance != null)
        {
            Debug.Log("GameEventReporter ya está inicializado.");
            return;
        }
        
        // Si se proporciona un prefab, instanciarlo
        if (eventReporterPrefab != null)
        {
            GameObject reporterObj = Instantiate(eventReporterPrefab);
            reporterObj.name = "GameEventReporter";
            DontDestroyOnLoad(reporterObj);
            return;
        }
        
        // Crear todo desde cero si no hay prefab
        SetupReporterFromScratch();
    }
    
    void SetupReporterFromScratch()
    {
        // Encontrar o crear un canvas si no se proporciona uno
        if (targetCanvas == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas c in canvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    targetCanvas = c;
                    break;
                }
            }
            
            // Si no se encontró ningún canvas, crear uno nuevo
            if (targetCanvas == null)
            {
                GameObject canvasObj = new GameObject("EventReporterCanvas");
                targetCanvas = canvasObj.AddComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }
        
        // Crear el objeto principal del reporter
        GameObject reporterObj = new GameObject("GameEventReporter");
        GameEventReporter reporter = reporterObj.AddComponent<GameEventReporter>();
        DontDestroyOnLoad(reporterObj);
        
        // Crear el panel de mensajes
        GameObject messagesPanel = new GameObject("MessagesPanel");
        messagesPanel.transform.SetParent(targetCanvas.transform, false);
        RectTransform panelRect = messagesPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(0.5f, 1);
        panelRect.anchoredPosition = new Vector2(0, -60);
        panelRect.sizeDelta = new Vector2(0, 300);
        
        // Crear el texto de plantilla para los mensajes
        GameObject textObj = new GameObject("EventTextTemplate");
        textObj.transform.SetParent(messagesPanel.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.pivot = new Vector2(0.5f, 1);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(-40, 30);
        
        // Configurar el componente TextMeshPro
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.fontSize = 24;
        if (messageFont != null)
            tmpText.font = messageFont;
        tmpText.enableWordWrapping = true;
        tmpText.gameObject.SetActive(false);  // Ocultar la plantilla
        
        // Configurar el componente de audio si se proporciona un clip
        if (notificationSound != null)
        {
            AudioSource audioSource = reporterObj.AddComponent<AudioSource>();
            audioSource.clip = notificationSound;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            reporter.notificationSound = audioSource;
        }
        
        // Asignar la referencia del texto al reporter
        reporter.eventText = tmpText;
    }
}