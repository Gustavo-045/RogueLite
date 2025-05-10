using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;  // Prefab del jugador
    public int mapWidth;        // Ancho del mapa
    public int mapHeight;       // Altura del mapa

    public MapGenerator mapGenerator;

    

    private GameObject player;

    void Start()
    {
        SpawnPlayer();
        mapGenerator = GameObject.FindWithTag("MapGenerator").GetComponent<MapGenerator>();
    }

    void SpawnPlayer()
    {
        // Calculamos la posición del centro del mapa
        Vector3 spawnPosition = mapGenerator.GetCenter();

        
        // Instanciamos el jugador en la posición central
        player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        
        // Opcional: Asignar el script de PlayerController al jugador
        PlayerController playerController = player.AddComponent<PlayerController>();
        mapHeight = mapGenerator.GetMapHeight();
        mapWidth = mapGenerator.GetMapWidth();

        playerController.Initialize(mapWidth, mapHeight);  // Le pasamos el tamaño del mapa al jugador
    }
}
