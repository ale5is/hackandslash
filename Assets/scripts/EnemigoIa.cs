using UnityEngine;

public class EnemigoIa : MonoBehaviour
{
    public Transform player;

    [Header("Movimiento")]
    public float speed = 3f;
    public float detectionRange = 10f;

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

            // evita mover en Y
            direction.y = 0f;

            direction.Normalize();

            transform.position += direction * speed * Time.deltaTime;

            // mirar al jugador
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("arma")) return;

        if (hitCooldownTimer > 0f) return;

        // dirección del golpe
        knockDirection = (transform.position - other.transform.position).normalized;

        // evita movimiento vertical
        knockDirection.y = 0f;

        isKnockedBack = true;
        knockbackTimer = knockbackTime;

        hitCooldownTimer = hitCooldown;
    }
}