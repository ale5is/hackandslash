using UnityEngine;

public class spawn : MonoBehaviour
{
    [Header("Objeto")]
    public GameObject objeto;

    void Start()
    {
        // MOVER JUGADOR A ESTE PUNTO
        objeto.transform.position = transform.position;

        // OPCIONAL:
        objeto.transform.rotation = transform.rotation;
    }
}