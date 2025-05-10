using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject waterPrefab;   // Prefab para el mar
    public GameObject islandPrefab;  // Prefab para la isla
    public GameObject enemyPrefab;   // Prefab para los enemigos
    public GameObject fogPrefab;     // Prefab para la niebla (ya lo creaste)

    // Tamaño del mapa
    public int mapWidth = 30;
    public int mapHeight = 30;

    // Probabilidades de aparición de islas y enemigos
    [Range(0f, 1f)]
    public float islandProbability = 0.05f; // 5% de probabilidad
    [Range(0f, 1f)]
    public float enemyProbability = 0.03f;  // 3% de probabilidad

    private List<GameObject> fogTiles = new List<GameObject>();  // Lista para la niebla

    void Start()
    {
        GenerateMap();
    }

    public Vector3 GetCenter()
    {
        Vector3 center = new Vector3(mapWidth / 2, mapHeight / 2, 0);
        return center;
    }

    public int GetMapHeight()
    {
        return mapHeight;
    }

    public int GetMapWidth()
    {
        return mapWidth;
    }

    void GenerateMap()
    {
        // Generamos el mapa sin usar un Grid ni Tilemap
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 position = new Vector3(x, y, 0); // Posición en el mundo

                // Decidir qué tipo de tile colocar en la posición (x, y)
                float rand = Random.value;

                GameObject newTile = null;

                if (rand < enemyProbability && enemyPrefab != null)
                {
                    newTile = Instantiate(enemyPrefab, position, Quaternion.identity, transform); // Instanciamos enemigo
                }
                else if (rand < islandProbability + enemyProbability && islandPrefab != null)
                {
                    newTile = Instantiate(islandPrefab, position, Quaternion.identity, transform); // Instanciamos isla
                }
                else
                {
                    newTile = Instantiate(waterPrefab, position, Quaternion.identity, transform); // Instanciamos agua
                }

                // Instanciamos niebla en todas las casillas excepto la zona central donde está el jugador
                if (Mathf.Abs(x - (mapWidth / 2)) > 1 || Mathf.Abs(y - (mapHeight / 2)) > 1)
                {
                    GameObject fogTile = Instantiate(fogPrefab, position, Quaternion.identity, transform);
                    fogTiles.Add(fogTile); // Añadimos la niebla a la lista
                }
            }
        }
    }

    // Método para eliminar la niebla

}
