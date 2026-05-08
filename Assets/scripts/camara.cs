using UnityEngine;

public class camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador;

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

    // Guarda transform inicial
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angulos = transform.eulerAngles;

        rotacionX = angulos.y;
        rotacionY = angulos.x;

        // Guarda posición y rotación inicial
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // Movimiento mouse
        rotacionX += Input.GetAxis("Mouse X") * sensibilidadMouse;
        rotacionY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;

        // Límites verticales
        rotacionY = Mathf.Clamp(rotacionY, minY, maxY);

        // Rotación deseada
        Quaternion rotacion =
            Quaternion.Euler(rotacionY, rotacionX, 0);

        // Posición deseada
        Vector3 posicionDeseada =
            jugador.position + rotacion * offset;

        // Movimiento suave
        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        // Mirar jugador
        transform.LookAt(jugador.position + Vector3.up * 1.5f);
    }

    // Reinicia EXACTAMENTE al inicio
    public void ResetearCamara()
    {
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        Vector3 angulos = rotacionInicial.eulerAngles;

        rotacionX = angulos.y;
        rotacionY = angulos.x;
    }
}