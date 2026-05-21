using UnityEngine;

public class EnemigoIa : MonoBehaviour
{
    public Transform player;

    [Header("Movimiento")]
    public float speed = 3f;
    public float detectionRange = 10f;

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

    [Header("Visual")]
    public Renderer renderEnemigo;

    void Start()
    {
        if (renderEnemigo == null)
            renderEnemigo = GetComponent<Renderer>();
    }

    void Update()
    {
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
            renderEnemigo.material.color = Color.yellow;

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
            // AMARILLO = STUNEADO
            renderEnemigo.material.color = Color.yellow;

            // MOVIMIENTO SUAVE
            transform.position +=
                knockVelocity *
                Time.deltaTime;

            // DESACELERACIÓN
            knockVelocity = Vector3.Lerp(
                knockVelocity,
                Vector3.zero,
                8f * Time.deltaTime
            );

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;

                knockVelocity = Vector3.zero;
            }

            return;
        }

        // =========================
        // EMBESTIDA
        // =========================
        if (embistiendo)
        {
            // ROJO ATACANDO
            renderEnemigo.material.color = Color.red;

            transform.position +=
                direccionEmbestida *
                velocidadEmbestida *
                Time.deltaTime;

            timerEmbestida -= Time.deltaTime;

            if (timerEmbestida <= 0f)
            {
                embistiendo = false;

                // EMPIEZA DESCANSO
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
            renderEnemigo.material.color = Color.red;

            timerPreparacion -= Time.deltaTime;

            Vector3 mirar =
                player.position - transform.position;

            mirar.y = 0f;

            if (mirar != Vector3.zero)
            {
                transform.rotation =
                    Quaternion.LookRotation(mirar);
            }

            if (timerPreparacion <= 0f)
            {
                preparandoAtaque = false;

                direccionEmbestida =
                    (player.position - transform.position)
                    .normalized;

                direccionEmbestida.y = 0f;

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
        // SEGUIR JUGADOR
        // =========================
        if (distance <= detectionRange)
        {
            // VERDE MOVIÉNDOSE
            renderEnemigo.material.color = Color.green;

            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            transform.position +=
                direction *
                speed *
                Time.deltaTime;

            // MIRAR JUGADOR
            if (direction != Vector3.zero)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction);
            }
        }
        else
        {
            // BLANCO QUIETO
            renderEnemigo.material.color = Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("arma")) return;

        if (hitCooldownTimer > 0f) return;

        // DAÑO
        vida--;

        // DIRECCIÓN KNOCKBACK
        Vector3 dir =
            (transform.position -
            other.transform.position).normalized;

        dir.y = 0f;

        // FUERZA INICIAL
        knockVelocity = dir * knockbackForce;

        isKnockedBack = true;

        knockbackTimer = knockbackTime;

        hitCooldownTimer = hitCooldown;

        // CANCELAR ATAQUE
        preparandoAtaque = false;
        embistiendo = false;

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