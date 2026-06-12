using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movimiento : MonoBehaviour
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
    private Vector3 movimientoFinal;

    private bool haciendoDash = false;

    private float tiempoDash;
    private float cooldownActual;

    public arma atacar;

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
        // SEGURIDAD
        if (cam == null)
        {
            cam = Camera.main;

            if (cam == null) return;
        }

        // RESET MOVIMIENTO
        movimientoFinal = Vector3.zero;

        // MOVIMIENTO
        Mover();

        // GRAVEDAD
        AplicarGravedad();

        // DASH
        Dash();

        // APLICAR TODO JUNTO
        controller.Move(movimientoFinal * Time.deltaTime);
    }

    void Mover()
    {
        // NO MOVER DURANTE ATAQUE
        if (atacar != null && atacar.atacando)
            return;

        // NO MOVER DURANTE DASH
        if (haciendoDash) return;

        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        Vector3 input =
            new Vector3(horizontal, 0f, vertical);

        // ===== LOCK ON =====
        if (objetivoLock != null)
        {
            // SI EL OBJETIVO SE DESTRUYÓ
            if (!objetivoLock.gameObject.activeInHierarchy)
            {
                objetivoLock = null;
                return;
            }

            // MIRAR ENEMIGO
            Vector3 direccionEnemigo =
                objetivoLock.position -
                transform.position;

            direccionEnemigo.y = 0f;

            if (direccionEnemigo.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(
                        direccionEnemigo
                    );

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacionObjetivo,
                        velocidadRotacion *
                        Time.deltaTime
                    );
            }

            // DIRECCIÓN RELATIVA A CÁMARA
            Vector3 camForward =
                cam.transform.forward;

            Vector3 camRight =
                cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            direccionMovimiento =
                (
                    camForward * vertical +
                    camRight * horizontal
                ).normalized;

            movimientoFinal +=
                direccionMovimiento *
                velocidad;
        }
        else
        {
            // ===== MOVIMIENTO NORMAL =====

            Vector3 forward =
                cam.transform.forward;

            Vector3 right =
                cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            direccionMovimiento =
                (
                    forward * vertical +
                    right * horizontal
                ).normalized;

            movimientoFinal +=
                direccionMovimiento *
                velocidad;

            // ROTAR SOLO SI HAY DIRECCIÓN
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
                        velocidadRotacion *
                        Time.deltaTime
                    );
            }
        }
    }

    void AplicarGravedad()
    {
        // PEGAR AL PISO
        if (
            controller.isGrounded &&
            velocidadVertical.y < 0f
        )
        {
            velocidadVertical.y = -2f;
        }

        // GRAVEDAD
        velocidadVertical.y +=
            gravedad * Time.deltaTime;

        movimientoFinal += velocidadVertical;
    }

    void Dash()
    {
        // SEGURIDAD
        if (atacar == null) return;

        // COOLDOWN
        if (cooldownActual > 0f)
        {
            cooldownActual -= Time.deltaTime;

            if (cooldownActual < 0f)
            {
                cooldownActual = 0f;
            }
        }

        // NO DASH DURANTE ATAQUE
        if (atacar.atacando) return;

        // INICIAR DASH
        if (
            Input.GetKeyDown(KeyCode.Space) &&
            cooldownActual <= 0f
        )
        {
            haciendoDash = true;

            tiempoDash = duracionDash;

            cooldownActual = cooldownDash;

            // SI ESTÁ QUIETO
            if (direccionMovimiento.sqrMagnitude < 0.01f)
            {
                direccionMovimiento =
                    -transform.forward;
            }

            direccionMovimiento.Normalize();
        }

        // HACER DASH
        if (haciendoDash)
        {
            movimientoFinal +=
                direccionMovimiento *
                fuerzaDash;

            tiempoDash -= Time.deltaTime;

            if (tiempoDash <= 0f)
            {
                haciendoDash = false;
            }
        }
    }
}