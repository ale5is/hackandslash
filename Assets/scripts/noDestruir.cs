using UnityEngine;

public class noDestruir : MonoBehaviour
{
    void Awake()
    {
        // BUSCAR TODOS LOS OBJETOS CON ESTE SCRIPT
        noDestruir[] objetos =
            FindObjectsOfType<noDestruir>();

        foreach (noDestruir obj in objetos)
        {
            // SI EXISTE OTRO CON EL MISMO NOMBRE
            if (obj != this &&
                obj.gameObject.name == gameObject.name)
            {
                Destroy(gameObject);
                return;
            }
        }

        // NO DESTRUIR ENTRE ESCENAS
        DontDestroyOnLoad(gameObject);
    }
}