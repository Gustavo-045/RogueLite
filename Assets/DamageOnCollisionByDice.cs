using UnityEngine;

public class DamageOnTriggerByDice : RiskDiceEvent
{
    [Header("Configuración de daño")]
    public int minDamage = 5;
    public int maxDamage = 30;

    [Header("Distancia del trigger")]
    public float tileSize = 1f;

    [Header("Configuración de loot")]
    public int minLoot = 5;
    public int maxLoot = 20;

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
            Debug.Log("💥 Jugador entró en rango del evento de riesgo");

            AskPlayerChoice(choice =>
            {
                int diceResult = DiceManager.Instance.RollDiceForDamage(6);
                PlayerResources resources = playerTransform.GetComponent<PlayerResources>();

                if (resources == null)
                {
                    Debug.LogWarning("❗ No se encontró PlayerResources en el jugador.");
                    return;
                }

                if (choice == RiskChoice.Flee)
                {
                    TriggerAmmoLoss(diceResult);
                    DropLoot(resources); // Aun si huye, puede llevarse algo
                    return;
                }

                if (diceResult <= 1)
                {
                    float damageRatio = (diceResult - 1) / 5f;
                    int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, damageRatio));
                    damage = AdjustBasedOnRisk(damage);

                    resources.ConsumeHealth(damage);
                    Debug.Log($"💀 Fallaste con un {diceResult}, recibiste {damage} de daño.");
                }
                else
                {
                    TriggerAmmoLoss(diceResult);
                    DropLoot(resources); // Solo se da loot si no te moriste feo
                }
            });
        }
    }

    private void TriggerAmmoLoss(int diceBonus)
    {
        PlayerResources resources = playerTransform.GetComponent<PlayerResources>();
        if (resources == null) return;

        int maxAmmoLoss = 6;
        int ammoLoss = Mathf.Clamp(maxAmmoLoss - diceBonus, 1, maxAmmoLoss);
        ammoLoss = AdjustBasedOnRisk(ammoLoss);

        resources.ConsumeAmmo(ammoLoss);
        Debug.Log($"🔫 Te salvaste del daño, pero perdiste {ammoLoss} de munición por tirar un {diceBonus}.");
    }

    private void DropLoot(PlayerResources resources)
    {
        int lootType = Random.Range(0, 3); // 0: Madera, 1: Comida, 2: Salud
        int lootAmount = Random.Range(minLoot, maxLoot + 1);
        lootAmount = AdjustBasedOnRisk(lootAmount);

        switch (lootType)
        {
            case 0:
                resources.AddWood(lootAmount);
                Debug.Log($"🪵 Loot recibido: {lootAmount} de Madera");
                break;
            case 1:
                resources.AddFood(lootAmount);
                Debug.Log($"🍗 Loot recibido: {lootAmount} de Comida");
                break;
            case 2:
                resources.AddHealth(lootAmount);
                Debug.Log($"❤️ Loot recibido: {lootAmount} de Salud");
                break;
        }
    }
}
