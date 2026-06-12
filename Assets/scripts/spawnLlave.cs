using UnityEngine;

public class spawnLlave : MonoBehaviour
{
    public EnemigoIa enemigo;
    public GameObject llave;

    public bool creada = false;

    void Update()
    {
        if (!creada && enemigo.vida <= 0)
        {
            creada = true;
            Debug.Log(1);
            Instantiate(
                llave,
                enemigo.transform.position,
                Quaternion.identity
            );
        }
    }
}