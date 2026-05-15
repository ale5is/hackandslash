using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class datosJugador : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMax = 100;
    public int vida = 100;

    [Header("Magia")]
    public int magiaMax = 100;
    public int magia = 50;

    [Header("Corrupción")]
    public float corrupcion = 0f;

    [Header("Escalado")]
    public int corrupcionPorNivel = 100;
    public int nivelMaximo = 999;

    [Header("Nivel")]
    public int nivel = 1;

    [Header("UI Sliders")]
    public Slider vidaSlider;
    public Slider magiaSlider;

    [Header("UI Textos")]
    public TMP_Text vidaTexto;
    public TMP_Text magiaTexto;
    public TMP_Text corrupcionTexto;
    public TMP_Text nivelTexto;

    void Start()
    {
        ActualizarNivel();
        ActualizarUI();
    }

    void Update()
    {
        // DAÑO
        if (Input.GetKeyDown(KeyCode.H))
        {
            vida -= 10;

            if (vida < 0)
                vida = 0;

            ActualizarUI();
        }

        // CURAR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            vida += 10;

            if (vida > vidaMax)
                vida = vidaMax;

            ActualizarUI();
        }

        // GANAR MAGIA
        if (Input.GetKeyDown(KeyCode.J))
        {
            magia += 10;

            if (magia > magiaMax)
                magia = magiaMax;

            ActualizarUI();
        }

        // GASTAR MAGIA
        if (Input.GetKeyDown(KeyCode.U))
        {
            magia -= 10;

            if (magia < 0)
                magia = 0;

            ActualizarUI();
        }

        // GANAR CORRUPCIÓN
        if (Input.GetKeyDown(KeyCode.K))
        {
            AgregarCorrupcion(25);
        }
    }

    public void AgregarCorrupcion(float cantidad)
    {
        corrupcion += cantidad;

        ActualizarNivel();
        ActualizarUI();
    }

    void ActualizarNivel()
    {
        // Cada 100 corrupción = 1 nivel
        nivel = Mathf.FloorToInt(corrupcion / corrupcionPorNivel) + 1;

        // Limitar nivel máximo
        if (nivel > nivelMaximo)
            nivel = nivelMaximo;
    }

    void ActualizarUI()
    {
        // VIDA
        vidaSlider.maxValue = vidaMax;
        vidaSlider.value = vida;

        vidaTexto.text = vida + " / " + vidaMax;

        // MAGIA
        magiaSlider.maxValue = magiaMax;
        magiaSlider.value = magia;

        magiaTexto.text = magia + " / " + magiaMax;

        // CORRUPCIÓN
        corrupcionTexto.text = "Corrupción: " + corrupcion;

        // NIVEL
        nivelTexto.text = "Nivel: " + nivel;
    }
}