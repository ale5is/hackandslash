using UnityEngine;

public class spawnJugador : MonoBehaviour
{
    void Start()
    {
        // BUSCAR PLAYER POR TAG
        GameObject jugador =
            GameObject.FindGameObjectWithTag("Player");

        // SI EXISTE
        if (jugador != null)
        {
            jugador.transform.position =
                transform.position;

            jugador.transform.rotation =
                transform.rotation;
        }
    }
}