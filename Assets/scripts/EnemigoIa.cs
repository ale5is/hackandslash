using UnityEngine;

public class EnemigoIa : MonoBehaviour
{
    public Transform player;

    [Header("Movimiento")]
    public float speed = 3f;
    public float detectionRange = 10f;

    [Header("Rotación")]
    public float rotationSpeed = 10f;

    [Header("Ataque Embestida")]
    public float rangoAtaque = 3f;
    public float tiempoPreparacion = 0.7f;

    public float velocidadEmbestida = 12f;
    public float duracionEmbestida = 0.3f;

    private bool preparandoAtaque = false;
    public bool embistiendo = false;

    private float timerPreparacion = 0f;
    private float timerEmbestida = 0f;

    private Vector3 direccionEmbestida;

    [Header("Cooldown ataque")]
    public float tiempoEntreAtaques = 2f;

    private bool enCooldownAtaque = false;
    private float timerCooldownAtaque = 0f;

    [Header("Vida")]
    public int vida = 3;

    [Header("Drop")]
    public GameObject orbeXpPrefab;
    public int cantidadOrbes = 5;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackTime = 0.25f;
    public float hitCooldown = 0.5f;

    private bool isKnockedBack = false;

    private float knockbackTimer = 0f;

    private Vector3 knockVelocity;

    private float hitCooldownTimer = 0f;

    [Header("Animación golpe")]
    public float inclinacionGolpe = 25f;
    public float velocidadRecuperacion = 8f;

    [Header("Animación ataque")]
    public float inclinacionPreparacion = -25f;
    public float inclinacionAtaque = 35f;

    private Quaternion rotacionOriginal;
    private Quaternion rotacionGolpe;

    [Header("Visual")]
    public Renderer renderEnemigo;

    private Material mat;

    void Start()
    {
        if (renderEnemigo == null)
            renderEnemigo = GetComponent<Renderer>();

        mat = renderEnemigo.material;

        rotacionOriginal = transform.rotation;
    }

    void Update()
    {
        // SEGURIDAD
        if (vida <= 0) return;

        if (player == null) return;

        // =========================
        // COOLDOWN HIT
        // =========================
        if (hitCooldownTimer > 0f)
            hitCooldownTimer -= Time.deltaTime;

        // =========================
        // COOLDOWN ATAQUE
        // =========================
        if (enCooldownAtaque)
        {
            // AMARILLO DESCANSANDO
            mat.color = Color.yellow;

            timerCooldownAtaque -= Time.deltaTime;

            if (timerCooldownAtaque <= 0f)
            {
                enCooldownAtaque = false;
            }

            return;
        }

        // =========================
        // KNOCKBACK
        // =========================
        if (isKnockedBack)
        {
            // AMARILLO STUN
            mat.color = Color.yellow;

            knockVelocity.y = 0f;

            // EMPUJE
            transform.position +=
                knockVelocity *
                Time.deltaTime;

            // DESACELERAR
            knockVelocity = Vector3.Lerp(
                knockVelocity,
                Vector3.zero,
                8f * Time.deltaTime
            );

            // ROTACIÓN HACIA ATRÁS
            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rotacionGolpe,
                    15f * Time.deltaTime
                );

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;

                knockVelocity = Vector3.zero;

                // DESCANSO
                enCooldownAtaque = true;
                timerCooldownAtaque = 0.5f;
            }

            return;
        }

        // =========================
        // RECUPERAR ROTACIÓN
        // =========================
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionOriginal,
                velocidadRecuperacion * Time.deltaTime
            );

        // =========================
        // EMBESTIDA
        // =========================
        if (embistiendo)
        {
            // ROJO ATACANDO
            mat.color = Color.red;

            transform.position +=
                direccionEmbestida *
                velocidadEmbestida *
                Time.deltaTime;

            timerEmbestida -= Time.deltaTime;

            if (timerEmbestida <= 0f)
            {
                embistiendo = false;

                // DESCANSO POST ATAQUE
                enCooldownAtaque = true;
                timerCooldownAtaque = tiempoEntreAtaques;
            }

            return;
        }

        // =========================
        // PREPARAR ATAQUE
        // =========================
        if (preparandoAtaque)
        {
            // ROJO CARGANDO
            mat.color = Color.red;

            timerPreparacion -= Time.deltaTime;

            Vector3 mirar =
                player.position - transform.position;

            mirar.y = 0f;

            if (mirar != Vector3.zero)
            {
                Quaternion baseRotacion =
                    Quaternion.LookRotation(mirar);

                // INCLINARSE HACIA ATRÁS
                Quaternion inclinacion =
                    baseRotacion *
                    Quaternion.Euler(inclinacionPreparacion, 0f, 0f);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        inclinacion,
                        rotationSpeed * Time.deltaTime
                    );
            }

            if (timerPreparacion <= 0f)
            {
                preparandoAtaque = false;

                if (player == null) return;

                direccionEmbestida =
                    (player.position - transform.position)
                    .normalized;

                direccionEmbestida.y = 0f;

                // INCLINARSE HACIA ADELANTE AL ATACAR
                transform.rotation =
                    Quaternion.LookRotation(direccionEmbestida) *
                    Quaternion.Euler(inclinacionAtaque, 0f, 0f);

                embistiendo = true;

                timerEmbestida = duracionEmbestida;
            }

            return;
        }

        // =========================
        // DISTANCIA
        // =========================
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // =========================
        // CERCA = PREPARA ATAQUE
        // =========================
        if (distance <= rangoAtaque && !preparandoAtaque)
        {
            preparandoAtaque = true;

            timerPreparacion = tiempoPreparacion;

            return;
        }

        // =========================
        // SEGUIR JUGADOR
        // =========================
        if (distance <= detectionRange)
        {
            // VERDE MOVIÉNDOSE
            mat.color = Color.green;

            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            transform.position +=
                direction *
                speed *
                Time.deltaTime;

            // ROTACIÓN SUAVE
            if (direction != Vector3.zero)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(direction);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacionObjetivo,
                        rotationSpeed * Time.deltaTime
                    );

                // GUARDAR ROTACIÓN NORMAL
                rotacionOriginal = rotacionObjetivo;
            }
        }
        else
        {
            // BLANCO QUIETO
            mat.color = Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("arma")) return;

        bool puedeRecibirDaño = hitCooldownTimer <= 0f;

        if (puedeRecibirDaño)
        {
            // DAÑO
            vida--;

            hitCooldownTimer = hitCooldown;
        }

        // DIRECCIÓN KNOCKBACK
        Vector3 dir =
            (transform.position -
            other.transform.position).normalized;

        dir.y = 0f;

        // ROTACIÓN GOLPE
        rotacionGolpe =
            Quaternion.LookRotation(dir) *
            Quaternion.Euler(inclinacionGolpe, 0f, 0f);

        // FUERZA INICIAL
        knockVelocity = dir * knockbackForce;

        isKnockedBack = true;

        knockbackTimer = knockbackTime;

        // CANCELAR ATAQUE
        if (embistiendo)
        {
            embistiendo = false;
            timerEmbestida = 0f;
        }

        // MORIR
        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // DROPEAR ORBES
        for (int i = 0; i < cantidadOrbes; i++)
        {
            Vector3 randomOffset =
                new Vector3(
                    Random.Range(-1f, 1f),
                    0.5f,
                    Random.Range(-1f, 1f)
                );

            Instantiate(
                orbeXpPrefab,
                transform.position + randomOffset,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}