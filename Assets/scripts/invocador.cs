using UnityEngine;

public class invocador : MonoBehaviour
{
    [Header("Prefab a invocar")]
    public GameObject prefab;

    [Header("Partículas")]
    public ParticleSystem particulas;

    [Header("Tiempo antes de aparecer")]
    public float tiempoInvocacion = 2f;

    [Header("Altura arriba del objeto")]
    public float altura = 2f;

    private float timer;
    private bool aparecio = false;

    void Start()
    {
        // Reproducir partículas
        if (particulas != null)
        {
            particulas.Play();
        }
    }

    void Update()
    {
        if (aparecio) return;

        timer += Time.deltaTime;

        if (timer >= tiempoInvocacion)
        {
            // Posición ARRIBA del objeto que tiene este script
            Vector3 posicionSpawn = transform.position + Vector3.up * altura;

            // Invocar prefab
            Instantiate(prefab, posicionSpawn, transform.rotation);

            aparecio = true;
        }
    }
}