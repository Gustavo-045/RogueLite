using UnityEngine;
using UnityEngine.UI;
using System;

public class RiskDiceUIManager : MonoBehaviour
{
    public static RiskDiceUIManager Instance;

    [Header("Referencias UI")]
    public GameObject riskChoicePanel;
    public Button buttonRisky;
    public Button buttonFlee;

    private Action<RiskDiceEvent.RiskChoice> onChoiceMade;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        riskChoicePanel.SetActive(false);
    }

    public void ShowRiskChoice(Action<RiskDiceEvent.RiskChoice> callback)
    {
        onChoiceMade = callback;
        riskChoicePanel.SetActive(true);

        buttonRisky.onClick.RemoveAllListeners();
        buttonFlee.onClick.RemoveAllListeners();

        buttonRisky.onClick.AddListener(() => MakeChoice(RiskDiceEvent.RiskChoice.Risky));
        buttonFlee.onClick.AddListener(() => MakeChoice(RiskDiceEvent.RiskChoice.Flee));
    }

    private void MakeChoice(RiskDiceEvent.RiskChoice choice)
    {
        riskChoicePanel.SetActive(false);
        onChoiceMade?.Invoke(choice);
    }
}
