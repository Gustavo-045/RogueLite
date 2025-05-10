using UnityEngine;

public class Nodo : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnMouseDown()
    {
        if (player == null) return;

        Vector2 nodoPos = transform.position;
        Vector2 playerPos = player.transform.position;

        float distanceX = Mathf.Abs(nodoPos.x - playerPos.x);
        float distanceY = Mathf.Abs(nodoPos.y - playerPos.y);

        // Solo si está adyacente (horizontal o vertical)
        if ((distanceX == 1f && distanceY == 0f) || (distanceY == 1f && distanceX == 0f))
        {
            player.MoveTo(nodoPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el objeto que entra en el trigger es el jugador y tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            // Si el nodo tiene el tag "Island" o "Enemy", acercamos la cámara
            if (CompareTag("Island") || CompareTag("Enemy"))
            {
                Debug.Log("penesote");
                player.ChangeCameraSize(true);  // Llamamos a ChangeCameraSize para acercar la cámara
            }

            // Si el nodo tiene el tag "Fog" entra en contacto con el jugador, se destruye
            if (CompareTag("Fog"))
            {
                Destroy(gameObject);  // Eliminar la niebla
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Si el objeto que sale del trigger es el jugador y tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            // Si el jugador sale de la casilla especial, restauramos la cámara
            if (CompareTag("Island") || CompareTag("Enemy"))
            {
                player.ChangeCameraSize(false);  // Llamamos a ChangeCameraSize para restaurar el tamaño original de la cámara
            }
        }
    }
}
