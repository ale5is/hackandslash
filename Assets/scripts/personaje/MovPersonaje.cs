using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovPersonaje : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float gravedad = -20f;
    public float velocidadRotacion = 10f;

    [Header("Dash")]
    public float fuerzaDash = 12f;
    public float duracionDash = 0.2f;
    public float cooldownDash = 1f;

    [Header("Animaciones")]
    public Animator animator;

    private CharacterController controller;

    private Vector3 velocidadVertical;
    private Vector3 direccionMovimiento;
    private Vector3 movimientoFinal;

    private bool haciendoDash = false;
    private bool atacando = false;
    private bool cubriendose = false;

    private float tiempoDash;
    private float cooldownActual;

    private Camera cam;

    [HideInInspector]
    public Transform objetivoLock;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        movimientoFinal = Vector3.zero;

        // ATAQUE
        if (Input.GetMouseButtonDown(0))
        {
            atacando = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            atacando = false;
        }

        // CUBRIRSE
        if (Input.GetMouseButtonDown(1))
        {
            cubriendose = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            cubriendose = false;
        }

        Mover();
        AplicarGravedad();
        Dash();

        controller.Move(movimientoFinal * Time.deltaTime);

        ActualizarAnimaciones();
    }

    void Mover()
    {
        if (haciendoDash) return;

        // BLOQUEAR MOVIMIENTO AL ATACAR O CUBRIRSE
        if (atacando || cubriendose)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (objetivoLock != null)
        {
            if (!objetivoLock.gameObject.activeInHierarchy)
            {
                objetivoLock = null;
                return;
            }

            Vector3 direccionEnemigo =
                objetivoLock.position - transform.position;

            direccionEnemigo.y = 0f;

            if (direccionEnemigo.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(direccionEnemigo);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacionObjetivo,
                        velocidadRotacion * Time.deltaTime
                    );
            }

            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            direccionMovimiento =
                (camForward * vertical +
                 camRight * horizontal).normalized;

            movimientoFinal +=
                direccionMovimiento * velocidad;
        }
        else
        {
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            direccionMovimiento =
                (forward * vertical +
                 right * horizontal).normalized;

            movimientoFinal +=
                direccionMovimiento * velocidad;

            if (direccionMovimiento.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(
                        direccionMovimiento
                    );

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacionObjetivo,
                        velocidadRotacion * Time.deltaTime
                    );
            }
        }
    }

    void AplicarGravedad()
    {
        if (controller.isGrounded &&
            velocidadVertical.y < 0f)
        {
            velocidadVertical.y = -2f;
        }

        velocidadVertical.y +=
            gravedad * Time.deltaTime;

        movimientoFinal += velocidadVertical;
    }

    void Dash()
    {
        if (cooldownActual > 0f)
        {
            cooldownActual -= Time.deltaTime;

            if (cooldownActual < 0f)
                cooldownActual = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            cooldownActual <= 0f &&
            !atacando &&
            !cubriendose)
        {
            haciendoDash = true;

            tiempoDash = duracionDash;

            cooldownActual = cooldownDash;

            if (direccionMovimiento.sqrMagnitude < 0.01f)
            {
                direccionMovimiento =
                    -transform.forward;
            }

            direccionMovimiento.Normalize();
        }

        if (haciendoDash)
        {
            movimientoFinal +=
                direccionMovimiento * fuerzaDash;

            tiempoDash -= Time.deltaTime;

            if (tiempoDash <= 0f)
            {
                haciendoDash = false;
            }
        }
    }

    void ActualizarAnimaciones()
    {
        if (animator == null) return;

        bool caminando =
            direccionMovimiento.sqrMagnitude > 0.01f &&
            !atacando &&
            !cubriendose &&
            !haciendoDash;

        animator.SetBool("Caminar", caminando);
        animator.SetBool("Atacar", atacando);
        animator.SetBool("Dash", haciendoDash);
        animator.SetBool("Cubrirse", cubriendose);
    }
}