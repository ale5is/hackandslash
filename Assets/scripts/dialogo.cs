using System.Collections;
using UnityEngine;
using TMPro;

public class dialogo : MonoBehaviour
{
    [Header("Jugador")]
    public datosJugador jugador;

    [Header("UI")]
    public TMP_Text textoDialogo;

    [Header("Tiempo")]
    public float tiempoEntreDialogos = 10f;
    public float tiempoVisible = 4f;

    [Header("Animación Letras")]
    public float velocidadLetras = 0.05f;

    private bool mostrandoDialogo = false;

    // GUARDAR ULTIMA FRASE
    private string ultimaFrase = "";

    void Start()
    {
        textoDialogo.text = "";

        StartCoroutine(ControlDialogos());
    }

    IEnumerator ControlDialogos()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                tiempoEntreDialogos
            );

            MostrarDialogo();
        }
    }

    void MostrarDialogo()
    {
        if (mostrandoDialogo)
            return;

        int nivel = jugador.nivel;

        string[] frases = null;

        // NIVEL 1
        if (nivel == 1)
        {
            frases = new string[]
            {
                "Todo se siente normal...",
                "Hace frío...",
                "El silencio es extraño..."
            };
        }

        // NIVEL 2
        else if (nivel == 2)
        {
            frases = new string[]
            {
                "¿Escuchaste eso?",
                "Creo que vi algo...",
                "No estoy solo..."
            };
        }

        // NIVEL 3
        else if (nivel == 3)
        {
            frases = new string[]
            {
                "Algo no está bien...",
                "Las paredes se sienten raras...",
                "Siento una presencia..."
            };
        }

        // NIVEL 4
        else if (nivel == 4)
        {
            frases = new string[]
            {
                "Siento que alguien me observa.",
                "Hay ojos en la oscuridad.",
                "No quiero mirar atrás..."
            };
        }

        // NIVEL 5
        else if (nivel == 5)
        {
            frases = new string[]
            {
                "Las sombras se mueven...",
                "Eso no estaba ahí antes.",
                "Algo me sigue..."
            };
        }

        // NIVEL 6
        else if (nivel == 6)
        {
            frases = new string[]
            {
                "No puedo confiar en mis ojos.",
                "La realidad cambia...",
                "Estoy viendo cosas..."
            };
        }

        // NIVEL 7
        else if (nivel == 7)
        {
            frases = new string[]
            {
                "Escucho voces...",
                "Ellos me hablan...",
                "No paran de susurrar..."
            };
        }

        // NIVEL 8
        else if (nivel == 8)
        {
            frases = new string[]
            {
                "Ellos vienen...",
                "Ya están aquí...",
                "No hay salida..."
            };
        }

        // NIVEL 9
        else if (nivel == 9)
        {
            frases = new string[]
            {
                "Ya no puedo detenerlo...",
                "La corrupción crece...",
                "Me estoy perdiendo..."
            };
        }

        // NIVEL 10
        else if (nivel >= 10)
        {
            frases = new string[]
            {
                "Ya eres parte de la corrupción.",
                "Tu mente ya no te pertenece.",
                "No queda nada humano..."
            };
        }

        // ELEGIR FRASE ALEATORIA
        string frase =
            frases[Random.Range(0, frases.Length)];

        // EVITAR REPETIR LA MISMA
        if (frase == ultimaFrase &&
            frases.Length > 1)
        {
            frase =
                frases[Random.Range(0, frases.Length)];
        }

        ultimaFrase = frase;

        StartCoroutine(
            MostrarTextoAnimado(frase)
        );
    }

    IEnumerator MostrarTextoAnimado(string frase)
    {
        mostrandoDialogo = true;

        textoDialogo.text = "";

        // ESCRIBIR LETRAS
        for (int i = 0; i < frase.Length; i++)
        {
            textoDialogo.text += frase[i];

            yield return new WaitForSeconds(
                velocidadLetras
            );
        }

        // ESPERAR
        yield return new WaitForSeconds(
            tiempoVisible
        );

        // BORRAR TEXTO
        textoDialogo.text = "";

        mostrandoDialogo = false;
    }
}