using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiarEscena : MonoBehaviour
{
    [Header("Nombre de la escena")]
    public string nombreEscena;

    Renderer puertaRenderer;

    void Start()
    {
        puertaRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (llave.puertasAbiertas)
        {
            puertaRenderer.material.color = Color.green;
        }
        else
        {
            puertaRenderer.material.color = Color.black;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && llave.puertasAbiertas)
        {
            llave.puertasAbiertas = false;
            SceneManager.LoadScene(nombreEscena);
        }
    }
}