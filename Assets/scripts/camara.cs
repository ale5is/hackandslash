using UnityEngine;

public class camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador;

    [Header("Lock On")]
    public string tagEnemigo = "Enemy";
    public float rangoBusqueda = 15f;

    private Transform enemigoFijado;

    [Header("Distancia")]
    public Vector3 offset = new Vector3(0, 6, -8);

    [Header("Suavizado")]
    public float suavizado = 8f;

    [Header("Rotación")]
    public float sensibilidadMouse = 3f;

    [Header("Límites Verticales")]
    public float minY = -20f;
    public float maxY = 70f;

    private float rotacionX;
    private float rotacionY;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angulos = transform.eulerAngles;

        rotacionX = angulos.y;
        rotacionY = angulos.x;

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // CLICK RUEDA MOUSE
        if (Input.GetMouseButtonDown(2))
        {
            if (enemigoFijado == null)
            {
                BuscarEnemigo();
            }
            else
            {
                enemigoFijado = null;
                jugador.GetComponent<movimiento>().objetivoLock = null;
            }
        }

        // =========================
        // LOCK ON
        // =========================
        if (enemigoFijado != null)
        {
            // Dirección hacia enemigo
            Vector3 direccion =
                (jugador.position - enemigoFijado.position).normalized;

            direccion.y = 0;

            // Rotar jugador hacia enemigo
            Vector3 dirJugador =
                enemigoFijado.position - jugador.position;

            dirJugador.y = 0;

            if (dirJugador != Vector3.zero)
            {
                Quaternion rotJugador =
                    Quaternion.LookRotation(dirJugador);

                jugador.rotation = Quaternion.Slerp(
                    jugador.rotation,
                    rotJugador,
                    10f * Time.deltaTime
                );
            }

            // Posición cámara detrás jugador
            Vector3 posicionLock =
                jugador.position +
                direccion * Mathf.Abs(offset.z) +
                Vector3.up * offset.y;

            transform.position = Vector3.Lerp(
                transform.position,
                posicionLock,
                suavizado * Time.deltaTime
            );

            // Punto medio entre ambos
            Vector3 puntoMedio =
                (jugador.position + enemigoFijado.position) / 2f;

            // Mirar al medio
            transform.LookAt(
                puntoMedio + Vector3.up * 1.5f
            );
        }
        else
        {
            // =========================
            // CÁMARA NORMAL
            // =========================

            rotacionX += Input.GetAxis("Mouse X") * sensibilidadMouse;
            rotacionY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;

            rotacionY = Mathf.Clamp(rotacionY, minY, maxY);

            Quaternion rotacion =
                Quaternion.Euler(rotacionY, rotacionX, 0);

            Vector3 posicionDeseada =
                jugador.position + rotacion * offset;

            transform.position = Vector3.Lerp(
                transform.position,
                posicionDeseada,
                suavizado * Time.deltaTime
            );

            transform.LookAt(
                jugador.position + Vector3.up * 1.5f
            );
        }
    }

    void BuscarEnemigo()
    {
        GameObject[] enemigos =
            GameObject.FindGameObjectsWithTag(tagEnemigo);

        float distanciaMinima = Mathf.Infinity;
        Transform mejorObjetivo = null;

        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector3.Distance(
                jugador.position,
                enemigo.transform.position
            );

            if (distancia < distanciaMinima &&
                distancia <= rangoBusqueda)
            {
                distanciaMinima = distancia;
                mejorObjetivo = enemigo.transform;
            }
        }

        enemigoFijado = mejorObjetivo;

        if (enemigoFijado != null)
        {
            jugador.GetComponent<movimiento>().objetivoLock =
                enemigoFijado;
        }
    }

    public void ResetearCamara()
    {
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        Vector3 angulos = rotacionInicial.eulerAngles;

        rotacionX = angulos.y;
        rotacionY = angulos.x;
    }
}