using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUi : MonoBehaviour
{
    public Text woodText;
    public Text foodText;
    public Text ammoText;

    public Text healthText;

    public PlayerResources playerResources;

    void Update()
    {

        playerResources = GameObject.FindWithTag("Player1").GetComponent<PlayerResources>();

        if (playerResources != null)
        {
            woodText.text = "Leña: " + playerResources.GetWood();
            foodText.text = "Alimento: " + playerResources.GetFood();
            ammoText.text = "Munición: " + playerResources.GetAmmo();
            healthText.text = "Health: " + playerResources.GetHealth();
        }
    }
}
