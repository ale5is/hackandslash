using UnityEngine;

public class EnemigoIa : MonoBehaviour
{
    public Transform player;

    [Header("Movimiento")]
    public float speed = 3f;
    public float detectionRange = 10f;

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

        // cooldown
        if (hitCooldownTimer > 0f)
            hitCooldownTimer -= Time.deltaTime;

        // knockback
        if (isKnockedBack)
        {
            transform.position += knockDirection * knockbackForce * Time.deltaTime;

            knockbackTimer -= Time.deltaTime;

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }

            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector3 direction = player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            transform.position += direction * speed * Time.deltaTime;

            // mirar jugador
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
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
        knockDirection = (transform.position - other.transform.position).normalized;
        knockDirection.y = 0f;

        isKnockedBack = true;
        knockbackTimer = knockbackTime;

        hitCooldownTimer = hitCooldown;

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
            Vector3 randomOffset = new Vector3(
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