using UnityEngine;

/// <summary>
/// DiceManager - Singleton que te permite lanzar dados desde cualquier script.
/// </summary>
public class DiceManager : MonoBehaviour
{
    // Instancia única del manager (pattern Singleton básico)
    public static DiceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// Lanza un dado con la cantidad de caras especificada.
    /// </summary>
    /// <param name="sides">Número de caras del dado (mínimo 2).</param>
    /// <returns>Resultado del dado (entre 1 y sides inclusive).</returns>
public int RollDice(int sides)
{
    if (sides < 2)
    {
        Debug.LogWarning("DiceManager: El dado debe tener al menos 2 caras. Usando 2 por defecto.");
        sides = 2;
    }

    int result = Random.Range(1, sides + 1);
    Debug.Log($"🎲 DiceManager: Lanzado un dado de {sides} caras. Resultado: {result}");
    
    return result;
}


    public int RollDiceForResource(int sides)
{
    int result = RollDice(sides);
    DiceUIManager.Instance?.ShowResourceDice(sides, result);
    return result;
}

public int RollDiceForDamage(int sides)
{
    int result = RollDice(sides);
    DiceUIManager.Instance?.ShowDamageDice(sides, result);
    return result;
}

}
