using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// GameEventReporterManager - Script principal para instalar y gestionar todo el sistema de reportes.
/// Agrega este componente a cualquier GameObject en la escena para inicializar el sistema completo.
/// </summary>
public class GameEventReporterManager : MonoBehaviour
{
    [Header("Configuración General")]
    public bool autoInitialize = true;
    
    [Header("Prefabs")]
    public GameObject reporterPrefab;
    
    [Header("Referencias")]
    public Canvas targetCanvas;
    
    [Header("Apariencia")]
    public TMP_FontAsset messageFont;
    public Color backgroundColor = new Color(0, 0, 0, 0.5f);
    public bool useRoundedCorners = true;
    
    [Header("Configuración de sonido")]
    public AudioClip notificationSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;
    
    private bool initialized = false;
    
    void Awake()
    {
        if (autoInitialize)
        {
            InitializeReportingSystem();
        }
    }
    
    /// <summary>
    /// Inicializa todo el sistema de reportes
    /// </summary>
    public void InitializeReportingSystem()
    {
        if (initialized) return;
        
        Debug.Log("🔄 Inicializando el sistema de reportes unificado...");
        
        // 1. Crear o encontrar el objeto principal del sistema de reportes
        GameObject reporterObj;
        if (reporterPrefab != null)
        {
            reporterObj = Instantiate(reporterPrefab);
        }
        else
        {
            reporterObj = new GameObject("GameEventReporter");
            reporterObj.AddComponent<GameEventReporter>();
        }
        reporterObj.name = "GameEventReporter";
        DontDestroyOnLoad(reporterObj);
        
        // 2. Buscar o crear el canvas para la UI
        Canvas uiCanvas = FindOrCreateCanvas();
        
        // 3. Crear el panel UI para los mensajes
        GameObject reportPanel = CreateReportPanel(uiCanvas);
        
        // 4. Crear la plantilla de texto para los mensajes
        TextMeshProUGUI textTemplate = CreateTextTemplate(reportPanel);
        
        // 5. Configurar el GameEventReporter
        ConfigureReporter(reporterObj, textTemplate);
        
        // 6. Añadir los interceptores para capturar eventos del juego
        AddInterceptors();
        
        initialized = true;
        Debug.Log("✅ Sistema de reportes unificado inicializado correctamente");
    }
    
    private Canvas FindOrCreateCanvas()
    {
        // Si ya hay un canvas asignado, usarlo
        if (targetCanvas != null)
            return targetCanvas;
            
        // Intentar encontrar un canvas existente
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvases)
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return c;
            }
        }
        
        // Si no hay canvas, crear uno
        GameObject canvasObj = new GameObject("GameEventReporterCanvas");
        Canvas newCanvas = canvasObj.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        newCanvas.sortingOrder = 100; // Para asegurar que esté encima de otros elementos
        
        // Añadir componentes necesarios
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return newCanvas;
    }
    
    private GameObject CreateReportPanel(Canvas canvas)
    {
        GameObject panel = new GameObject("EventReportPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        // Configurar RectTransform
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -20);
        rt.sizeDelta = new Vector2(0, 200);
        
        // Opcional: Añadir un fondo
        if (backgroundColor.a > 0)
        {
            Image bg = panel.AddComponent<Image>();
            bg.color = backgroundColor;
            
            // Agregar esquinas redondeadas si se desea
            if (useRoundedCorners)
            {
                // Esto requeriría una imagen con esquinas redondeadas o un shader personalizado
                // Para este ejemplo, usamos color simple
            }
        }
        
        return panel;
    }
    
    private TextMeshProUGUI CreateTextTemplate(GameObject parent)
    {
        GameObject textObj = new GameObject("EventTextTemplate");
        textObj.transform.SetParent(parent.transform, false);
        
        // Configurar RectTransform
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(-40, 30); // Margen de 20px en cada lado
        
        // Configurar TextMeshPro
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.fontSize = 24;
        if (messageFont != null)
            tmpText.font = messageFont;
        tmpText.enableWordWrapping = true;
        tmpText.overflowMode = TextOverflowModes.Overflow;
        tmpText.gameObject.SetActive(false); // La plantilla comienza oculta
        
        return tmpText;
    }
    
    private void ConfigureReporter(GameObject reporterObj, TextMeshProUGUI textTemplate)
    {
        GameEventReporter reporter = reporterObj.GetComponent<GameEventReporter>();
        if (reporter == null)
        {
            reporter = reporterObj.AddComponent<GameEventReporter>();
        }
        
        // Asignar la plantilla de texto
        reporter.eventText = textTemplate;
        
        // Configurar audio si hay sonido
        if (notificationSound != null)
        {
            AudioSource audioSource = reporterObj.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = reporterObj.AddComponent<AudioSource>();
                
            audioSource.clip = notificationSound;
            audioSource.volume = soundVolume;
            audioSource.playOnAwake = false;
            reporter.notificationSound = audioSource;
        }
    }
    
    private void AddInterceptors()
    {
        // Añadir interceptor para PlayerResources
        GameObject hookObj = new GameObject("GameReporterHook");
        hookObj.AddComponent<GameReporterHook>();
        DontDestroyOnLoad(hookObj);
        
        // La clase GameReporterExtensions se encargará automáticamente de parchar DiceManager
    }
}