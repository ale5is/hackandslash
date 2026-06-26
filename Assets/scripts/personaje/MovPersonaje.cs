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

    [Header("Ataque")]
    public float impulsoAtaque = 2.5f;
    public float duracionImpulso = 0.15f;

    [Header("Especial")]
    public float impulsoEspecial = 4f;
    public float duracionImpulsoEspecial = 0.15f;

    [Header("Animaciones")]
    public Animator animator;

    private CharacterController controller;
    private Camera cam;

    private Vector3 velocidadVertical;
    private Vector3 direccionMovimiento;
    private Vector3 movimientoFinal;

    private bool haciendoDash = false;
    private bool atacando = false;
    private bool cubriendose = false;
    private bool especialActivo = false;

    private float tiempoDash;
    private float cooldownActual;

    private float tiempoImpulso;
    private bool impulsoActivo;

    private float tiempoImpulsoEspecial;
    private bool impulsoEspecialActivo;

    private Vector3 direccionEspecial;

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

        // =========================
        // ATAQUE NORMAL
        // =========================
        if (Input.GetMouseButtonDown(0))
        {
            atacando = true;
            impulsoActivo = true;
            tiempoImpulso = duracionImpulso;
        }

        if (Input.GetMouseButtonUp(0))
        {
            atacando = false;
        }

        // =========================
        // ESPECIAL (Q) - DASH ATRAVESANDO ENEMIGO
        // =========================
        if (Input.GetKeyDown(KeyCode.Q))
        {
            especialActivo = true;

            impulsoEspecialActivo = true;
            tiempoImpulsoEspecial = duracionImpulsoEspecial;

            Vector3 direccion;

            if (objetivoLock != null)
            {
                direccion = (objetivoLock.position - transform.position);
                direccion.y = 0f;
                direccion.Normalize();

                // 🔥 va directo al enemigo y lo atraviesa
                direccionEspecial = direccion;
            }
            else
            {
                direccionEspecial = transform.forward;
                direccionEspecial.y = 0f;
                direccionEspecial.Normalize();
            }
        }

        Mover();
        AplicarGravedad();
        Dash();
        AplicarImpulsoAtaque();
        AplicarImpulsoEspecial();

        controller.Move(movimientoFinal * Time.deltaTime);

        ActualizarAnimaciones();
    }

    void Mover()
    {
        if (haciendoDash) return;

        if (atacando || cubriendose || especialActivo)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        direccionMovimiento =
            (forward * vertical + right * horizontal).normalized;

        movimientoFinal += direccionMovimiento * velocidad;

        if (direccionMovimiento.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccionMovimiento);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rotacionObjetivo,
                    velocidadRotacion * Time.deltaTime
                );
        }
    }

    void AplicarGravedad()
    {
        if (controller.isGrounded && velocidadVertical.y < 0f)
            velocidadVertical.y = -2f;

        velocidadVertical.y += gravedad * Time.deltaTime;
        movimientoFinal += velocidadVertical;
    }

    void Dash()
    {
        if (cooldownActual > 0f)
            cooldownActual -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) &&
            cooldownActual <= 0f &&
            !atacando &&
            !cubriendose &&
            !especialActivo)
        {
            haciendoDash = true;
            tiempoDash = duracionDash;
            cooldownActual = cooldownDash;

            if (direccionMovimiento.sqrMagnitude < 0.01f)
                direccionMovimiento = transform.forward;

            direccionMovimiento.Normalize();
        }

        if (haciendoDash)
        {
            movimientoFinal += direccionMovimiento * fuerzaDash;

            tiempoDash -= Time.deltaTime;

            if (tiempoDash <= 0f)
                haciendoDash = false;
        }
    }

    void AplicarImpulsoAtaque()
    {
        if (!impulsoActivo) return;

        movimientoFinal += transform.forward * impulsoAtaque;

        tiempoImpulso -= Time.deltaTime;

        if (tiempoImpulso <= 0f)
            impulsoActivo = false;
    }

    // =========================
    // ESPECIAL (IMPULSO)
    // =========================
    void AplicarImpulsoEspecial()
    {
        if (!impulsoEspecialActivo) return;

        movimientoFinal += direccionEspecial * impulsoEspecial;

        tiempoImpulsoEspecial -= Time.deltaTime;

        if (tiempoImpulsoEspecial <= 0f)
        {
            impulsoEspecialActivo = false;
            especialActivo = false;
        }
    }

    void ActualizarAnimaciones()
    {
        if (animator == null) return;

        bool caminando =
            direccionMovimiento.sqrMagnitude > 0.01f &&
            !atacando &&
            !cubriendose &&
            !haciendoDash &&
            !especialActivo;

        animator.SetBool("Caminar", caminando);
        animator.SetBool("Atacar", atacando);
        animator.SetBool("Dash", haciendoDash);
        animator.SetBool("Cubrirse", cubriendose);
        animator.SetBool("Especial", especialActivo);
    }
}