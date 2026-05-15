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
    private bool embistiendo = false;

    private float timerPreparacion = 0f;
    private float timerEmbestida = 0f;

    private Vector3 direccionEmbestida;

    [Header("Vida")]
    public int vida = 3;

    [Header("Drop")]
    public GameObject orbeXpPrefab;
    public int cantidadOrbes = 5;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackTime = 0.2f;
    public float hitCooldown = 0.5f;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector3 knockDirection;

    private float hitCooldownTimer = 0f;

    void Update()
    {
        if (player == null) return;

        // =========================
        // COOLDOWN
        // =========================
        if (hitCooldownTimer > 0f)
            hitCooldownTimer -= Time.deltaTime;

        // =========================
        // KNOCKBACK
        // =========================
        if (isKnockedBack)
        {
            transform.position +=
                knockDirection *
                knockbackForce *
                Time.deltaTime;

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }

            return;
        }

        // =========================
        // EMBESTIDA
        // =========================
        if (embistiendo)
        {
            transform.position +=
                direccionEmbestida *
                velocidadEmbestida *
                Time.deltaTime;

            timerEmbestida -= Time.deltaTime;

            if (timerEmbestida <= 0f)
            {
                embistiendo = false;
            }

            return;
        }

        // =========================
        // PREPARAR ATAQUE
        // =========================
        if (preparandoAtaque)
        {
            timerPreparacion -= Time.deltaTime;

            // mirar jugador mientras carga
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
        // SI ESTÁ CERCA -> PREPARA EMBESTIDA
        // =========================
        if (distance <= rangoAtaque)
        {
            preparandoAtaque = true;

            timerPreparacion = tiempoPreparacion;

            return;
        }

        // =========================
        // SI ESTÁ LEJOS -> SEGUIR
        // =========================
        if (distance <= detectionRange)
        {
            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            transform.position +=
                direction *
                speed *
                Time.deltaTime;

            // mirar jugador
            if (direction != Vector3.zero)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("arma")) return;

        if (hitCooldownTimer > 0f) return;

        // daño
        vida--;

        // knockback
        knockDirection =
            (transform.position -
            other.transform.position).normalized;

        knockDirection.y = 0f;

        isKnockedBack = true;

        knockbackTimer = knockbackTime;

        hitCooldownTimer = hitCooldown;

        // cancelar ataque
        preparandoAtaque = false;
        embistiendo = false;

        // morir
        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // invoca orbes tipo minecraft
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