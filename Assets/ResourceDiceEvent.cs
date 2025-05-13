using UnityEngine;

public class ResourceDiceEvent : RiskDiceEvent
{
    [Header("Configuración de evento")]
    public int minImpact = 5;
    public int maxImpact = 50;
    public bool canBePositive = true;
    public float tileSize = 1f;

    private Transform playerTransform;
    private bool hasTriggered = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("❗ No se encontró un objeto con tag 'Player1'");
    }

    void Update()
    {
        if (hasTriggered || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= tileSize)
        {
            hasTriggered = true;
            Debug.Log("✨ Jugador entró en rango de evento de recursos");

            AskPlayerChoice(choice =>
            {
                if (choice == RiskChoice.Flee) return;

                int diceResult = DiceManager.Instance.RollDice(6);
                float impactRatio = (diceResult - 1) / 5f;
                int impactAmount = Mathf.RoundToInt(Mathf.Lerp(minImpact, maxImpact, impactRatio));
                impactAmount = AdjustBasedOnRisk(impactAmount);

                bool isPositive = canBePositive && Random.value > 0.5f;
                int resourceIndex = Random.Range(0, 4);

                PlayerResources playerResources = playerTransform.GetComponent<PlayerResources>();
                if (playerResources == null)
                {
                    Debug.LogWarning("❗ No se encontró PlayerResources en el jugador.");
                    return;
                }

                switch (resourceIndex)
                {
                    case 0:
                        ApplyResourceChange(playerResources, "Madera", impactAmount, isPositive, playerResources.AddWood, playerResources.ConsumeWood);
                        break;
                    case 1:
                        ApplyResourceChange(playerResources, "Comida", impactAmount, isPositive, playerResources.AddFood, playerResources.ConsumeFood);
                        break;
                    case 2:
                        ApplyResourceChange(playerResources, "Munición", impactAmount, isPositive, playerResources.AddAmmo, playerResources.ConsumeAmmo);
                        break;
                    case 3:
                        ApplyResourceChange(playerResources, "Salud", impactAmount, isPositive, playerResources.AddHealth, playerResources.ConsumeHealth);
                        break;
                }
            });
        }
    }

private void ApplyResourceChange(PlayerResources resources, string resourceName, int amount, bool positive, System.Action<int> addMethod, System.Action<int> consumeMethod)
{
    if (positive)
    {
        addMethod(amount);
        Debug.Log($"🪵 Ganaste {amount} de {resourceName}.");
        
        // Mostrar el mensaje en la UI
        GameEventReporter.Instance.ReportResourceChange(resourceName, amount, resources.GetWood()); // Cambia GetWood() por el método adecuado para obtener el total
    }
    else
    {
        consumeMethod(amount);
        Debug.Log($"💀 Perdiste {amount} de {resourceName}.");
        
        // Mostrar el mensaje en la UI
        GameEventReporter.Instance.ReportResourceChange(resourceName, -amount, resources.GetWood()); // Cambia GetWood() por el método adecuado para obtener el total
    }
}

}
