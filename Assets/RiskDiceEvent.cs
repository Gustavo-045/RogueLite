using UnityEngine;

public abstract class RiskDiceEvent : MonoBehaviour
{
    public enum RiskChoice
    {
        Normal,
        Risky,
        Flee
    }

    protected RiskChoice playerChoice = RiskChoice.Normal;

    /// <summary>
    /// Llama al panel de decisión de riesgo.
    /// </summary>
    protected void AskPlayerChoice(System.Action<RiskChoice> onDecisionMade)
    {
        if (RiskDiceUIManager.Instance == null)
        {
            Debug.LogError("❌ RiskDiceUIManager no encontrado en la escena.");
            onDecisionMade(RiskChoice.Normal); // fallback seguro
            return;
        }

        RiskDiceUIManager.Instance.ShowRiskChoice(choice =>
        {
            playerChoice = choice;
            if (choice == RiskChoice.Flee)
                Debug.Log("🐔 Jugador decidió huir.");
            else
                Debug.Log(choice == RiskChoice.Risky ? "🔥 Jugador eligió arriesgarse." : "😐 Jugador eligió lanzar normal.");

            onDecisionMade?.Invoke(playerChoice);
        });
    }

    protected int AdjustBasedOnRisk(int baseAmount)
    {
        return playerChoice == RiskChoice.Risky ? baseAmount * 2 : baseAmount;
    }
}
