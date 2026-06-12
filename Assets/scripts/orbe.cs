using UnityEngine;

public class orbe : MonoBehaviour
{
    public static bool puertasAbiertas = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puertasAbiertas = true;
            Destroy(gameObject);
        }
    }
}