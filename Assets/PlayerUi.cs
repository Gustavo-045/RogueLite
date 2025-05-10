using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUi : MonoBehaviour
{
    public Text woodText;
    public Text foodText;
    public Text ammoText;

    public PlayerResources playerResources;

    void Start()
    {
        // Buscar al jugador en la escena
        
        
    }

    void Update()
    {

                playerResources = GameObject.FindWithTag("Player").GetComponent<PlayerResources>();

        if (playerResources != null)
        {
            woodText.text = "Leña: " + playerResources.GetWood();
            foodText.text = "Alimento: " + playerResources.GetFood();
            ammoText.text = "Munición: " + playerResources.GetAmmo();
        }
    }
}
