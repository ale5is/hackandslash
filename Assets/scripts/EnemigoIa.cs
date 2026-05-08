using UnityEngine;

public class EnemigoIa : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float detectionRange = 10f;

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

        // cooldown del golpe
        if (hitCooldownTimer > 0f)
            hitCooldownTimer -= Time.deltaTime;

        // knockback
        if (isKnockedBack)
        {
            transform.position += knockDirection * knockbackForce * Time.deltaTime;

            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false;

            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arma") && hitCooldownTimer <= 0f)
        {
            knockDirection = (transform.position - other.transform.position).normalized;

            isKnockedBack = true;
            knockbackTimer = knockbackTime;

            hitCooldownTimer = hitCooldown;
        }
    }
}