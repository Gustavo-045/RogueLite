using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DiceUIManager : MonoBehaviour
{
    public TextMeshProUGUI diceResultText;
    public float showDuration = 3f;

    public GameObject[] diceFaceImages; // Asigna manualmente o en Start()

    private Coroutine currentDisplay;

    public static DiceUIManager Instance { get; private set; }


    public GameObject[] diceFacesResource;
    public GameObject[] diceFacesDamage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Si no están asignadas por el Inspector, intenta buscarlas automáticamente
        if (diceFaceImages == null || diceFaceImages.Length == 0)
        {
            Transform panel = GameObject.Find("DiceImagePanel")?.transform;
            if (panel != null)
            {
                diceFaceImages = new GameObject[panel.childCount];
                for (int i = 0; i < panel.childCount; i++)
                    diceFaceImages[i] = panel.GetChild(i).gameObject;
            }
        }
    }

    public void ShowResourceDice(int sides, int result)
{
    if (currentDisplay != null)
        StopCoroutine(currentDisplay);

    currentDisplay = StartCoroutine(ShowDiceVisuals(sides, result, diceFacesResource));
}

public void ShowDamageDice(int sides, int result)
{
    if (currentDisplay != null)
        StopCoroutine(currentDisplay);

    currentDisplay = StartCoroutine(ShowDiceVisuals(sides, result, diceFacesDamage));
}


    public void ShowDiceResult(int sides, int result)
    {
        if (currentDisplay != null)
            StopCoroutine(currentDisplay);

        currentDisplay = StartCoroutine(ShowDiceVisuals(sides, result));
    }

// Versión por defecto que usa diceFaceImages
private IEnumerator ShowDiceVisuals(int sides, int result)
{
    return ShowDiceVisuals(sides, result, diceFaceImages);
}

// Versión que permite especificar un set de imágenes (para daño o recursos)
private IEnumerator ShowDiceVisuals(int sides, int result, GameObject[] diceSet)
{
    // Mostrar texto
    diceResultText.text = $"🎲 Dado de {sides} caras → <b>{result}</b>";
    diceResultText.gameObject.SetActive(true);

    // Mostrar imagen del dado
    int index = Mathf.Clamp(result - 1, 0, diceSet.Length - 1);
    for (int i = 0; i < diceSet.Length; i++)
    {
        diceSet[i].SetActive(i == index);
    }

    // Esperar
    yield return new WaitForSeconds(showDuration);

    // Ocultar texto
    diceResultText.gameObject.SetActive(false);
    diceResultText.text = "";

    // Ocultar imágenes
    foreach (var img in diceSet)
        img.SetActive(false);

    currentDisplay = null;
}

}
