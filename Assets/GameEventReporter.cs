using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.UI;

/// <summary>
/// GameEventReporter - Sistema de reportes unificado para mostrar todos los eventos importantes del juego.
/// Muestra mensajes sobre cambios de recursos, resultados de dados, y eventos de riesgo.
/// </summary>
public class GameEventReporter : MonoBehaviour
{
    [Header("Configuración UI")]
    public TextMeshProUGUI eventText;              // Componente TextMeshPro donde se mostrará el texto
    public float messageDuration = 3.0f;           // Duración de cada mensaje en pantalla
    public int maxMessages = 5;                    // Máximo número de mensajes visibles al mismo tiempo
    public float messageSpacing = 35f;             // Espacio vertical entre mensajes
    public AudioSource notificationSound;          // Sonido al mostrar un mensaje (opcional)

    [Header("Efectos visuales")]
    public bool fadeOutMessages = true;            // Si los mensajes desaparecen gradualmente
    public bool slideMessages = true;              // Si los mensajes se deslizan al aparecer/desaparecer
    public float messageFadeTime = 0.5f;           // Tiempo de transición para aparecer/desaparecer
    
    // Colores para diferentes tipos de eventos
    public Color resourceGainColor = new Color(0.2f, 0.8f, 0.2f);       // Verde para ganancias
    public Color resourceLossColor = new Color(0.8f, 0.2f, 0.2f);       // Rojo para pérdidas
    public Color diceRollColor = new Color(0.2f, 0.6f, 0.8f);           // Azul para tiradas de dados
    public Color riskEventColor = new Color(0.8f, 0.6f, 0.2f);          // Naranja para eventos de riesgo

    public Image diceImage;  // Referencia al Image UI donde se mostrará el dado
    public Sprite[] diceSprites;  // Array de sprites de los dados (debe tener 6 sprites, uno por cada cara)


    // Lista de mensajes activos
    private List<MessageData> activeMessages = new List<MessageData>();
    
    // Instancia única (patrón Singleton)
    public static GameEventReporter Instance { get; private set; }

    // Clase para almacenar datos de cada mensaje
    private class MessageData
    {
        public string text;
        public TextMeshProUGUI textObject;
        public float expireTime;
        public Coroutine fadeCoroutine;
        
        public MessageData(string text, TextMeshProUGUI textObject, float expireTime)
        {
            this.text = text;
            this.textObject = textObject;
            this.expireTime = expireTime;
        }
    }

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Crear el contenedor de mensajes si no existe
        if (eventText == null)
        {
            Debug.LogError("❌ GameEventReporter: No se ha asignado el TextMeshProUGUI para mostrar eventos.");
        }
    }

    private void Start()
    {
        // Suscribirse a eventos globales si es necesario
        // Podría expandirse para usar un sistema de eventos más complejo
    }

    private void Update()
    {
        // Comprobar y eliminar mensajes caducados
        CleanExpiredMessages();
    }

    /// <summary>
    /// Reporta un cambio en los recursos del jugador
    /// </summary>
    /// <param name="resourceName">Nombre del recurso (Leña, Comida, etc.)</param>
    /// <param name="amount">Cantidad (positiva para ganancia, negativa para pérdida)</param>
    /// <param name="newTotal">Nuevo total del recurso (opcional)</param>
    public void ReportResourceChange(string resourceName, int amount, int newTotal = -1)
    {
        string emoji = GetResourceEmoji(resourceName);
        string message;
        
        if (amount > 0)
        {
            message = $"{emoji} <color=#{ColorUtility.ToHtmlStringRGB(resourceGainColor)}> +{amount} {resourceName}</color>";
            if (newTotal >= 0)
                message += $" (Total: {newTotal})";
        }
        else
        {
            message = $"{emoji} <color=#{ColorUtility.ToHtmlStringRGB(resourceLossColor)}> {amount} {resourceName}</color>";
            if (newTotal >= 0)
                message += $" (Total: {newTotal})";
        }
        
        ShowMessage(message);
    }

    /// <summary>
    /// Reporta el resultado de una tirada de dados
    /// </summary>
    /// <param name="sides">Número de caras del dado</param>
    /// <param name="result">Resultado de la tirada</param>
 public void ReportDiceRoll(int sides, int result)
{
    // Asegúrate de que 'diceSprites' tenga los sprites correctos para cada cara del dado
    if (result >= 1 && result <= 6 && diceSprites.Length == 6)
    {
        // Cambia la imagen del dado al sprite correspondiente al número
        diceImage.sprite = diceSprites[result - 1];  // Restamos 1 porque el array es 0-indexado
    }

    // Opcional: Mostrar un mensaje con la tirada (si aún quieres mostrarlo)
    //string message = $"🎲 <color=#{ColorUtility.ToHtmlStringRGB(diceRollColor)}>Dado de {sides} caras: <b>{result}</b></color>";
    //ShowMessage(message);
}


    /// <summary>
    /// Reporta un evento especial de riesgo
    /// </summary>
    /// <param name="eventDescription">Descripción del evento</param>
    /// <param name="isSuccessful">Si el evento fue exitoso o no</param>
    public void ReportRiskEvent(string eventDescription, bool isSuccessful)
    {
        string emoji = isSuccessful ? "✅" : "❌";
        string message = $"{emoji} <color=#{ColorUtility.ToHtmlStringRGB(riskEventColor)}>{eventDescription}</color>";
        ShowMessage(message);
    }

    /// <summary>
    /// Reporta un mensaje general en el sistema
    /// </summary>
    /// <param name="message">Mensaje a mostrar</param>
    /// <param name="color">Color del mensaje (opcional)</param>
    public void ReportMessage(string message, Color? color = null)
    {
        Color textColor = color ?? Color.white;
        ShowMessage($"<color=#{ColorUtility.ToHtmlStringRGB(textColor)}>{message}</color>");
    }

    /// <summary>
    /// Limpia todos los mensajes de la pantalla
    /// </summary>
    public void ClearAllMessages()
    {
        foreach (var msg in activeMessages)
        {
            if (msg.fadeCoroutine != null)
                StopCoroutine(msg.fadeCoroutine);
            
            Destroy(msg.textObject.gameObject);
        }
        
        activeMessages.Clear();
    }

    /// <summary>
    /// Muestra un mensaje en la pantalla
    /// </summary>
    /// <param name="message">Mensaje a mostrar</param>
    private void ShowMessage(string message)
    {
        // Crear un nuevo objeto de texto
        TextMeshProUGUI newMessageText = Instantiate(eventText, eventText.transform.parent);
        newMessageText.gameObject.SetActive(true);
        newMessageText.text = message;
        
        // Registrar el nuevo mensaje
        float expireTime = Time.time + messageDuration;
        MessageData msgData = new MessageData(message, newMessageText, expireTime);
        activeMessages.Add(msgData);
        
        // Reproducir sonido de notificación si está configurado
        if (notificationSound != null)
            notificationSound.Play();
        
        // Eliminar mensajes antiguos si superamos el máximo
        while (activeMessages.Count > maxMessages)
        {
            var oldestMsg = activeMessages[0];
            if (oldestMsg.fadeCoroutine != null)
                StopCoroutine(oldestMsg.fadeCoroutine);
                
            Destroy(oldestMsg.textObject.gameObject);
            activeMessages.RemoveAt(0);
        }
        
        // Reposicionar todos los mensajes
        RepositionMessages();
        
        // Aplicar efectos visuales
        if (fadeOutMessages || slideMessages)
        {
            msgData.fadeCoroutine = StartCoroutine(FadeMessage(msgData));
        }
    }

    /// <summary>
    /// Reposiciona todos los mensajes activos en la pantalla
    /// </summary>
    private void RepositionMessages()
    {
        for (int i = 0; i < activeMessages.Count; i++)
        {
            RectTransform rt = activeMessages[i].textObject.rectTransform;
            Vector2 position = rt.anchoredPosition;
            position.y = -i * messageSpacing;
            rt.anchoredPosition = position;
        }
    }

    /// <summary>
    /// Elimina los mensajes que han expirado
    /// </summary>
    private void CleanExpiredMessages()
    {
        for (int i = activeMessages.Count - 1; i >= 0; i--)
        {
            if (Time.time > activeMessages[i].expireTime && activeMessages[i].fadeCoroutine == null)
            {
                Destroy(activeMessages[i].textObject.gameObject);
                activeMessages.RemoveAt(i);
                
                // Reposicionar los mensajes restantes
                RepositionMessages();
            }
        }
    }

    /// <summary>
    /// Corrutina para aplicar efectos de desvanecimiento y deslizamiento a los mensajes
    /// </summary>
    private IEnumerator FadeMessage(MessageData msgData)
    {
        TextMeshProUGUI textObj = msgData.textObject;
        RectTransform rt = textObj.rectTransform;
        
        // Aparecer
        float elapsed = 0;
        if (fadeOutMessages)
        {
            textObj.alpha = 0;
            while (elapsed < messageFadeTime)
            {
                textObj.alpha = Mathf.Lerp(0, 1, elapsed / messageFadeTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            textObj.alpha = 1;
        }
        
        // Esperar hasta que sea hora de desaparecer
        yield return new WaitForSeconds(messageDuration - messageFadeTime * 2);
        
        // Desaparecer
        elapsed = 0;
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(slideMessages ? 100 : 0, 0);
        
        while (elapsed < messageFadeTime)
        {
            if (fadeOutMessages)
                textObj.alpha = Mathf.Lerp(1, 0, elapsed / messageFadeTime);
                
            if (slideMessages)
                rt.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / messageFadeTime);
                
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Eliminar el mensaje
        int index = activeMessages.IndexOf(msgData);
        if (index >= 0)
        {
            activeMessages.RemoveAt(index);
            Destroy(textObj.gameObject);
            
            // Reposicionar los mensajes restantes
            RepositionMessages();
        }
        
        msgData.fadeCoroutine = null;
    }

    /// <summary>
    /// Obtiene el emoji correspondiente a un tipo de recurso
    /// </summary>
    private string GetResourceEmoji(string resourceName)
    {
        resourceName = resourceName.ToLower();
        
        if (resourceName.Contains("madera") || resourceName.Contains("leña"))
            return "🪵";
        else if (resourceName.Contains("comida") || resourceName.Contains("alimento"))
            return "🍗";
        else if (resourceName.Contains("salud") || resourceName.Contains("vida"))
            return "❤️";
        else if (resourceName.Contains("muni") || resourceName.Contains("bala"))
            return "🔫";
        else
            return "📦";
    }
}