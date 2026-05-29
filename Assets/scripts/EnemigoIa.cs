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

    [Header("Recuperación")]
    public float velocidadRecuperacion = 2f;

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
        // BUSCAR PLAYER
        GameObject jugador =
            GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            player = jugador.transform;
        }

        // RENDER
        if (renderEnemigo == null)
        {
            renderEnemigo = GetComponent<Renderer>();
        }

        // MATERIAL
        mat = renderEnemigo.material;

        rotacionOriginal = transform.rotation;
    }

    void Update()
    {
        // =========================
        // SEGURIDAD
        // =========================
        if (vida <= 0) return;

        // REBUSCAR PLAYER SI SE PERDIÓ
        if (player == null)
        {
            GameObject jugador =
                GameObject.FindGameObjectWithTag("Player");

            if (jugador != null)
            {
                player = jugador.transform;
            }

            return;
        }

        // =========================
        // COOLDOWN HIT
        // =========================
        if (hitCooldownTimer > 0f)
        {
            hitCooldownTimer -= Time.deltaTime;
        }

        // =========================
        // COOLDOWN ATAQUE
        // =========================
        if (enCooldownAtaque)
        {
            mat.color = Color.yellow;

            timerCooldownAtaque -= Time.deltaTime;

            RecuperarRotacion();

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
            mat.color = Color.yellow;

            knockVelocity.y = 0f;

            // MOVIMIENTO
            transform.position +=
                knockVelocity *
                Time.deltaTime;

            // DESACELERAR
            knockVelocity =
                Vector3.Lerp(
                    knockVelocity,
                    Vector3.zero,
                    8f * Time.deltaTime
                );

            // ROTACIÓN GOLPE
            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rotacionGolpe,
                    10f * Time.deltaTime
                );

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;

                knockVelocity = Vector3.zero;

                // RECUPERACIÓN
                enCooldownAtaque = true;
                timerCooldownAtaque = 0.5f;
            }

            return;
        }

        // =========================
        // EMBESTIDA
        // =========================
        if (embistiendo)
        {
            mat.color = Color.red;

            transform.position +=
                direccionEmbestida *
                velocidadEmbestida *
                Time.deltaTime;

            timerEmbestida -= Time.deltaTime;

            if (timerEmbestida <= 0f)
            {
                embistiendo = false;

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
            mat.color = Color.red;

            timerPreparacion -= Time.deltaTime;

            Vector3 mirar =
                player.position - transform.position;

            mirar.y = 0f;

            if (mirar.sqrMagnitude > 0.01f)
            {
                Quaternion baseRotacion =
                    Quaternion.LookRotation(mirar);

                Quaternion inclinacion =
                    baseRotacion *
                    Quaternion.Euler(
                        inclinacionPreparacion,
                        0f,
                        0f
                    );

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

                Vector3 dir =
                    player.position - transform.position;

                dir.y = 0f;

                if (dir.sqrMagnitude > 0.01f)
                {
                    direccionEmbestida =
                        dir.normalized;
                }

                transform.rotation =
                    Quaternion.LookRotation(
                        direccionEmbestida
                    ) *
                    Quaternion.Euler(
                        inclinacionAtaque,
                        0f,
                        0f
                    );

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
        if (distance <= rangoAtaque)
        {
            preparandoAtaque = true;

            timerPreparacion = tiempoPreparacion;

            return;
        }

        // =========================
        // SEGUIR PLAYER
        // =========================
        if (distance <= detectionRange)
        {
            mat.color = Color.green;

            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            transform.position +=
                direction *
                speed *
                Time.deltaTime;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion rotacionObjetivo =
                    Quaternion.LookRotation(direction);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        rotacionObjetivo,
                        rotationSpeed * Time.deltaTime
                    );

                rotacionOriginal =
                    rotacionObjetivo;
            }
        }
        else
        {
            mat.color = Color.white;

            // ENDEREZARSE SOLO CUANDO ESTÁ QUIETO
            RecuperarRotacion();
        }
    }

    void RecuperarRotacion()
    {
        Quaternion rotacionEnderezada =
            Quaternion.Euler(
                0f,
                transform.eulerAngles.y,
                0f
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionEnderezada,
                velocidadRecuperacion *
                Time.deltaTime
            );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("arma")) return;

        // EVITAR MULTIHIT
        if (hitCooldownTimer > 0f) return;

        // DAÑO
        vida--;

        hitCooldownTimer = hitCooldown;

        // CANCELAR ATAQUES
        preparandoAtaque = false;
        embistiendo = false;

        timerPreparacion = 0f;
        timerEmbestida = 0f;

        // DIRECCIÓN KNOCKBACK
        Vector3 dir =
            (transform.position -
            other.transform.position);

        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            dir.Normalize();
        }

        // ROTACIÓN GOLPE
        rotacionGolpe =
            Quaternion.LookRotation(dir) *
            Quaternion.Euler(
                inclinacionGolpe,
                0f,
                0f
            );

        // FUERZA
        knockVelocity =
            dir * knockbackForce;

        isKnockedBack = true;

        knockbackTimer = knockbackTime;

        // MORIR
        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // ORBES
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