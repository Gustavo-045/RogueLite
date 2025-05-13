using UnityEngine;
using TMPro;
using System.Collections;

public class DiceUIManager : MonoBehaviour
{
    public TextMeshProUGUI diceResultText;
    public float showDuration = 3f;

    private Coroutine currentDisplay;

    public static DiceUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDiceResult(int sides, int result)
    {
        if (currentDisplay != null)
            StopCoroutine(currentDisplay);

        currentDisplay = StartCoroutine(ShowText($"🎲 Dado de {sides} caras → <b>{result}</b>"));
    }

    private IEnumerator ShowText(string text)
    {
        diceResultText.text = text;
        diceResultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(showDuration);

        diceResultText.gameObject.SetActive(false);
        diceResultText.text = "";
        currentDisplay = null;
    }
}
