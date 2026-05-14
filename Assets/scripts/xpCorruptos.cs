using UnityEngine;

public class xpCorruptos : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Corrupción")]
    public float corrupcionQueDa = 5f;

    private Transform jugador;

    void Start()
    {
        // Busca al jugador por tag
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");

        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    void Update()
    {
        // Seguir al jugador
        if (jugador != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                jugador.position,
                velocidad * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta al jugador
        if (other.CompareTag("Player"))
        {
            // Obtiene el script datosJugador
            datosJugador datos = other.GetComponent<datosJugador>();

            if (datos != null)
            {
                // Suma corrupción
                datos.corrupcion += corrupcionQueDa;
            }

            // Destruye la orbe
            Destroy(gameObject);
        }
    }
}