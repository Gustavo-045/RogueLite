using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public int wood = 100;
    public int food = 100;
    public int ammo = 100;

    public int health = 100;

    // Métodos públicos para obtener los valores actuales
    public int GetWood() => wood;
    public int GetFood() => food;
    public int GetAmmo() => ammo;

    public int GetHealth() => health;

    // Método para consumir leña
    public void ConsumeWood(int amount)
    {
        wood = Mathf.Max(wood - amount, 0);
    }

    // (Opcional) Métodos futuros:
    public void ConsumeFood(int amount)
    {
        food = Mathf.Max(food - amount, 0);
    }

    public void ConsumeAmmo(int amount)
    {
        ammo = Mathf.Max(ammo - amount, 0);
    }

    public void ConsumeHealth(int amount)
    {
        health = Mathf.Max(health - amount,0);
    }

    // También puedes agregar AddResource si quieres recolección más adelante
    public void AddWood(int amount)
    {
        wood += amount;
    }

    public void AddFood(int amount)
    {
        food += amount;
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
    }

    public void AddHealth(int amount)
    {
        health += amount;
    }
}
