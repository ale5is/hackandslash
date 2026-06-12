using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class arma : MonoBehaviour
{
    [Header("Ataque")]
    public float velocidadAtaque = 8f;
    public float anguloAtaque = 120f;
    public float avanceAtaque = 3f;
    public float tiempoEntreAtaques = 0.5f;

    [Header("Daño")]
    public int dañoBase = 10;
    public int dañoActual = 10;

    [Header("Buff")]
    public KeyCode teclaBuff = KeyCode.Q;
    public float multiplicadorDaño = 2f;
    public float duracionBuff = 5f;

    [Header("Ultimate")]
    public KeyCode teclaUlti = KeyCode.R;
    public float fuerzaUlti = 25f;
    public float duracionUlti = 0.2f;
    public float tiempoMaxCarga = 2f;
    public float dañoMinUlti = 20f;
    public float dañoMaxUlti = 100f;

    [Header("UI")]
    public Slider sliderUlti;
    public TMP_Text porcentajeTexto;

    [Header("Zoom")]
    public float fovNormal = 60f;
    public float fovUlti = 40f;
    public float velocidadZoom = 10f;

    public Collider col;
    public Renderer armaRenderer;

    CharacterController controller;
    movimiento movimientoJugador;
    camara scriptCamara;
    Camera cam;

    bool buffActivo;
    float tiempoBuff;

    bool cargandoUlti;
    bool usandoUlti;
    float tiempoCarga;
    float tiempoUlti;
    float dañoUltiActual;

    public bool atacando;
    bool regresando;
    bool esperandoRegreso;

    float progreso;
    float cooldown;

    Vector3 direccionAtaque;

    Quaternion rotInicial;
    Quaternion rotFinal;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        movimientoJugador = GetComponentInParent<movimiento>();

        dañoActual = dañoBase;
        col.enabled = false;

        armaRenderer.material.DisableKeyword("_EMISSION");

        sliderUlti.gameObject.SetActive(false);
        porcentajeTexto.gameObject.SetActive(false);

        sliderUlti.minValue = 0;
        sliderUlti.maxValue = tiempoMaxCarga;

        cam = Camera.main;

        if (cam)
        {
            cam.fieldOfView = fovNormal;
            scriptCamara = cam.GetComponent<camara>();
        }
    }

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        Ataque();
        Buff();
        Ultimate();
    }

    void Ataque()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Golpe de ida
            if (!atacando && !esperandoRegreso && cooldown <= 0)
            {
                atacando = true;
                regresando = false;
                progreso = 0;

                cooldown = tiempoEntreAtaques;

                rotInicial = Quaternion.identity;
                rotFinal = Quaternion.Euler(0, anguloAtaque, 0);

                col.enabled = true;
            }
            // Golpe de vuelta
            else if (!atacando && esperandoRegreso)
            {
                atacando = true;
                regresando = true;
                progreso = 0;

                col.enabled = true;
            }
        }

        if (!atacando)
            return;

        progreso += Time.deltaTime * velocidadAtaque;

        // Avanza siempre hacia donde mira el jugador
        controller.Move(
            movimientoJugador.transform.forward *
            avanceAtaque *
            Time.deltaTime
        );

        // Ida
        if (!regresando)
        {
            transform.localRotation =
                Quaternion.Slerp(rotInicial, rotFinal, progreso);

            if (progreso >= 1f)
            {
                atacando = false;
                esperandoRegreso = true;

                transform.localRotation = rotFinal;

                col.enabled = false;
            }
        }
        // Vuelta
        else
        {
            transform.localRotation =
                Quaternion.Slerp(rotFinal, rotInicial, progreso);

            if (progreso >= 1f)
            {
                atacando = false;
                esperandoRegreso = false;

                transform.localRotation = Quaternion.identity;

                col.enabled = false;
            }
        }
    }

    void Buff()
    {
        if (Input.GetKeyDown(teclaBuff) && !buffActivo)
        {
            buffActivo = true;
            tiempoBuff = duracionBuff;

            dañoActual =
                Mathf.RoundToInt(dañoBase * multiplicadorDaño);

            armaRenderer.material.EnableKeyword("_EMISSION");
        }

        if (!buffActivo) return;

        tiempoBuff -= Time.deltaTime;

        if (tiempoBuff <= 0)
        {
            buffActivo = false;
            dañoActual = dañoBase;

            armaRenderer.material.DisableKeyword("_EMISSION");
        }
    }

    void Ultimate()
    {
        if (Input.GetKeyDown(teclaUlti))
        {
            cargandoUlti = true;
            tiempoCarga = 0;

            sliderUlti.gameObject.SetActive(true);
            porcentajeTexto.gameObject.SetActive(true);
        }

        if (cargandoUlti)
        {
            tiempoCarga += Time.deltaTime;
            tiempoCarga = Mathf.Clamp(tiempoCarga, 0, tiempoMaxCarga);

            sliderUlti.value = tiempoCarga;

            porcentajeTexto.text =
                Mathf.RoundToInt(
                    tiempoCarga / tiempoMaxCarga * 100
                ) + "%";

            if (cam)
            {
                cam.fieldOfView = Mathf.Lerp(
                    cam.fieldOfView,
                    fovUlti,
                    velocidadZoom * Time.deltaTime
                );
            }
        }
        else if (cam)
        {
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                fovNormal,
                velocidadZoom * Time.deltaTime
            );
        }

        if (Input.GetKeyUp(teclaUlti) && cargandoUlti)
        {
            cargandoUlti = false;

            sliderUlti.gameObject.SetActive(false);
            porcentajeTexto.gameObject.SetActive(false);

            if (scriptCamara)
                scriptCamara.DesactivarLock();

            float porcentaje = tiempoCarga / tiempoMaxCarga;

            dañoUltiActual = Mathf.Lerp(
                dañoMinUlti,
                dañoMaxUlti,
                porcentaje
            );

            usandoUlti = true;
            tiempoUlti = duracionUlti;
        }

        if (!usandoUlti) return;

        Vector3 dir = cam.transform.forward;
        dir.y = 0;
        dir.Normalize();

        controller.Move(
            dir *
            fuerzaUlti *
            Time.deltaTime
        );

        tiempoUlti -= Time.deltaTime;

        if (tiempoUlti <= 0)
            usandoUlti = false;
    }
}