using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class datosJugador : MonoBehaviour
{
    [Header("Estadísticas")]
    public int vida = 100;
    public int nivel = 1;

    [Header("Sistema")]
    public float corrupcion = 0f;
    public int magia = 0;

    void Start()
    {
        Debug.Log("Vida: " + vida);
        Debug.Log("Nivel: " + nivel);
        Debug.Log("Corrupción: " + corrupcion);
        Debug.Log("Magia: " + magia);
    }

    void Update()
    {

    }
}