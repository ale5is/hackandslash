using UnityEngine;

public class arma : MonoBehaviour
{
    [Header("Ataque")]
    public float velocidadAtaque = 8f;
    public float anguloAtaque = 120f;

    [Header("Cooldown")]
    public float tiempoEntreAtaques = 0.5f;

    private bool atacando = false;
    private bool regresando = false;

    private float progreso = 0f;
    private float cooldown = 0f;

    private Quaternion rotacionInicial;
    private Quaternion rotacionObjetivo;

    public Collider col;

    void Start()
    {
        
        col.enabled = false; // desactivado al inicio
    }

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)
            && !atacando
            && cooldown <= 0)
        {
            atacando = true;
            regresando = false;
            progreso = 0f;

            cooldown = tiempoEntreAtaques;

            rotacionInicial = transform.localRotation;

            rotacionObjetivo =
                rotacionInicial *
                Quaternion.Euler(0, anguloAtaque, 0);

            // 🔥 ACTIVA collider al atacar
            col.enabled = true;
        }

        if (atacando)
            AnimarAtaque();
    }

    void AnimarAtaque()
    {
        progreso += Time.deltaTime * velocidadAtaque;

        if (!regresando)
        {
            transform.localRotation =
                Quaternion.Slerp(rotacionInicial, rotacionObjetivo, progreso);

            if (progreso >= 1f)
            {
                progreso = 0f;
                regresando = true;
            }
        }
        else
        {
            transform.localRotation =
                Quaternion.Slerp(rotacionObjetivo, rotacionInicial, progreso);

            if (progreso >= 1f)
            {
                atacando = false;
                transform.localRotation = rotacionInicial;

                // ❌ DESACTIVA collider al terminar ataque
                col.enabled = false;
            }
        }
    }
}