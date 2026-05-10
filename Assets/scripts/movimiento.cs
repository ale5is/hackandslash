using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movimiento  : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float gravedad = -20f;
    public float velocidadRotacion = 10f;

    [Header("Dash")]
    public float fuerzaDash = 12f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private Vector3 direccionMovimiento;

    private bool haciendoDash = false;
    private float tiempoDash;
    private float cooldownActual;
    private arma atacar;

    [HideInInspector] public Transform objetivoLock;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        atacar = GetComponent<arma>();
    }

    void Update()
    {
        Mover();
        AplicarGravedad();
        Dash();
    }

    void Mover()
    {
        if (haciendoDash) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0, vertical).normalized;

        if (input.magnitude < 0.1f) return;

        // ===== LOCK ON =====
        if (objetivoLock != null)
        {
            // Mirar enemigo
            Vector3 direccionEnemigo =
                objetivoLock.position - transform.position;

            direccionEnemigo.y = 0;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direccionEnemigo),
                velocidadRotacion * Time.deltaTime
            );

            // Dirección desde cámara
            Camera cam = Camera.main;

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            // Movimiento relativo cámara
            direccionMovimiento =
                (camForward * vertical + camRight * horizontal).normalized;

            // Mover personaje
            controller.Move(
                direccionMovimiento * velocidad * Time.deltaTime
            );
        }
        else
        {
            // ===== MOVIMIENTO NORMAL =====

            Camera cam = Camera.main;

            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            direccionMovimiento =
                forward * vertical + right * horizontal;

            controller.Move(
                direccionMovimiento.normalized *
                velocidad *
                Time.deltaTime
            );

            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccionMovimiento);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }
    }

    void AplicarGravedad()
    {
        if (controller.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    void Dash()
    {
        if (!atacar.atacando)
        {
            cooldownActual -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && cooldownActual <= 0)
            {
                haciendoDash = true;
                tiempoDash = duracionDash;
                cooldownActual = cooldownDash;
            }

            if (haciendoDash)
            {
                controller.Move(transform.forward * fuerzaDash * Time.deltaTime);

                tiempoDash -= Time.deltaTime;

                if (tiempoDash <= 0)
                {
                    haciendoDash = false;
                }
            }
        }
            
    }
}