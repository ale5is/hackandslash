using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// POST PROCESSING
using UnityEngine.Rendering.PostProcessing;

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

    [Header("Nivel")]
    public int nivel = 1;
    public int nivelMaximo = 10;

    [Header("Daño enemigo")]
    public int dañoEnemigo = 10;

    [Header("Invulnerabilidad")]
    public float tiempoInvulnerable = 1f;
    private bool invulnerable = false;

    [Header("POST PROCESS")]
    public PostProcessVolume postProcessVolume;

    private Vignette vignette;

    [Range(0f, 1f)]
    public float intensidadMaxima = 0.6f;

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
        // OBTENER VIGNETTE
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out vignette);
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemigo") && !invulnerable)
        {
            EnemigoIa enemigo =
                other.gameObject.GetComponent<EnemigoIa>();

            if (enemigo != null && enemigo.embistiendo)
            {
                Debug.Log("El jugador fue golpeado");

                vida -= dañoEnemigo;

                if (vida < 0)
                    vida = 0;

                ActualizarUI();

                StartCoroutine(Invulnerabilidad());
            }
        }
    }

    IEnumerator Invulnerabilidad()
    {
        invulnerable = true;

        yield return new WaitForSeconds(tiempoInvulnerable);

        invulnerable = false;
    }

    public void AgregarCorrupcion(float cantidad)
    {
        corrupcion += cantidad;

        ActualizarNivel();
        ActualizarUI();
    }

    void ActualizarNivel()
    {
        // CALCULAR NIVEL
        nivel =
            Mathf.FloorToInt(
                corrupcion / corrupcionPorNivel
            ) + 1;

        // LIMITAR NIVEL
        nivel = Mathf.Clamp(
            nivel,
            1,
            nivelMaximo
        );

        // VIGNETTE
        if (vignette != null)
        {
            // NIVEL 1 = 0
            // NIVEL 10 = 1
            float porcentaje =
                (float)(nivel - 1) /
                (nivelMaximo - 1);

            // CALCULAR INTENSIDAD
            float intensidad =
                porcentaje * intensidadMaxima;

            // APLICAR
            vignette.intensity.overrideState = true;
            vignette.intensity.value = intensidad;

            vignette.smoothness.overrideState = true;
            vignette.smoothness.value = 1f;

            vignette.rounded.overrideState = true;
            vignette.rounded.value = true;

            Debug.Log(
                "Nivel: " + nivel +
                " | Intensidad: " + intensidad
            );
        }
    }

    void ActualizarUI()
    {
        // VIDA
        if (vidaSlider != null)
        {
            vidaSlider.maxValue = vidaMax;
            vidaSlider.value = vida;
        }

        if (vidaTexto != null)
        {
            vidaTexto.text =
                vida + " / " + vidaMax;
        }

        // MAGIA
        if (magiaSlider != null)
        {
            magiaSlider.maxValue = magiaMax;
            magiaSlider.value = magia;
        }

        if (magiaTexto != null)
        {
            magiaTexto.text =
                magia + " / " + magiaMax;
        }

        // CORRUPCIÓN
        if (corrupcionTexto != null)
        {
            corrupcionTexto.text =
                "Corrupción: " + corrupcion;
        }

        // NIVEL
        if (nivelTexto != null)
        {
            nivelTexto.text =
                "Nivel: " + nivel;
        }
    }
}