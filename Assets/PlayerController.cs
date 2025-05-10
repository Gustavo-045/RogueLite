using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private int mapWidth;
    private int mapHeight;
    private MapGenerator mapGenerator;  // Referencia al generador de mapa

    public float moveSpeed = 5f;  // Velocidad de movimiento
    public Camera playerCamera;  // Referencia a la cámara del jugador
    public float zoomedSize = 0.5f;  // Tamaño de la cámara cuando entra en casillas especiales
    public float defaultSize = 2f;  // Tamaño predeterminado de la cámara

    public PlayerResources resources;  // Referencia al script de recursos

    public void Initialize(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;

        // Obtener referencias necesarias
        mapGenerator = GameObject.FindWithTag("MapGenerator").GetComponent<MapGenerator>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        resources = this.GetComponent<PlayerResources>();
    }


    void Start()
    {
       
        // Obtener referencias necesarias
        mapGenerator = GameObject.FindWithTag("MapGenerator").GetComponent<MapGenerator>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        resources = this.GetComponent<PlayerResources>();
    }

    // Este método será llamado desde el script Nodo
    public void MoveTo(Vector2 newPosition)
    {
        // Si hay leña, podemos movernos
        if (resources != null && resources.wood > 0)
        {
            resources.ConsumeWood(1); // Restar una unidad de leña al moverse
            StartCoroutine(MovePlayer(newPosition));
        }
        else
        {
            Debug.Log("¡No tienes leña para moverte!");
        }
    }

    // Corutina para mover al jugador suavemente hacia la nueva posición
    private IEnumerator MovePlayer(Vector2 targetPosition)
    {
        float distance = Vector2.Distance(transform.position, targetPosition);  // Distancia al objetivo
        while (distance > 0.1f)  // Continuamos mientras no estemos lo suficientemente cerca
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            distance = Vector2.Distance(transform.position, targetPosition);
            yield return null;
        }

        transform.position = targetPosition;
    }

    // Este método será llamado desde el script Nodo para manejar el zoom de la cámara
    public void ChangeCameraSize(bool zoomIn)
    {
        if (playerCamera == null) return;

       // playerCamera.orthographicSize = zoomIn ? zoomedSize : defaultSize;
    }
}
