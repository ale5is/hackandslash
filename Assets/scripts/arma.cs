using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class arma : MonoBehaviour
{
    [Header("Ataque")]
    public float velocidadAtaque = 8f;
    public float anguloAtaque = 120f;

    // 🔥 AVANCE AL GOLPEAR
    public float avanceAtaque = 3f;

    [Header("Daño")]
    public int dañoBase = 10;
    public int dañoActual = 10;

    [Header("Habilidad Buff")]
    public float multiplicadorDaño = 2f;
    public float duracionBuff = 5f;
    public KeyCode teclaBuff = KeyCode.Q;

    private bool buffActivo = false;
    private float tiempoBuff;

    [Header("Ultimate Cargable")]
    public float fuerzaUlti = 25f;
    public float duracionUlti = 0.2f;
    public KeyCode teclaUlti = KeyCode.R;

    public Slider sliderUlti;
    public TMP_Text porcentajeTexto;

    public float tiempoMaxCarga = 2f;

    public float dañoMinUlti = 20f;
    public float dañoMaxUlti = 100f;

    private bool cargandoUlti = false;
    private float tiempoCarga = 0f;

    private float dañoUltiActual;

    private bool usandoUlti = false;
    private float tiempoUlti;

    private CharacterController controller;
    private movimiento movimientoJugador;

    private Vector3 direccionAtaque;

    [Header("Cooldown")]
    public float tiempoEntreAtaques = 0.5f;

    public bool atacando = false;
    private bool regresando = false;

    private float progreso = 0f;
    private float cooldown = 0f;

    private Quaternion rotacionInicial;
    private Quaternion rotacionObjetivo;

    public Collider col;
    public Renderer armaRenderer;

    // =========================
    // ZOOM ULTI
    // =========================
    [Header("Zoom Ulti")]
    public float fovNormal = 60f;
    public float fovUlti = 40f;
    public float velocidadZoom = 10f;

    private Camera cam;

    // =========================
    // CÁMARA SCRIPT
    // =========================
    private camara scriptCamara;

    void Start()
    {
        controller =
            GetComponentInParent<CharacterController>();

        movimientoJugador =
            GetComponentInParent<movimiento>();

        dañoActual = dañoBase;

        col.enabled = false;

        // EMISSION APAGADO
        armaRenderer.material.DisableKeyword("_EMISSION");

        // CONFIG SLIDER
        sliderUlti.gameObject.SetActive(false);

        sliderUlti.minValue = 0;
        sliderUlti.maxValue = tiempoMaxCarga;
        sliderUlti.value = 0;

        // TEXTO %
        porcentajeTexto.gameObject.SetActive(false);

        // CÁMARA
        cam = Camera.main;

        if (cam != null)
        {
            cam.fieldOfView = fovNormal;

            // OBTENER SCRIPT CÁMARA
            scriptCamara =
                cam.GetComponent<camara>();
        }
    }

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        HabilidadBuff();
        UltimateDash();

        if (Input.GetMouseButtonDown(0)
            && !atacando
            && cooldown <= 0)
        {
            atacando = true;
            regresando = false;
            progreso = 0f;

            cooldown = tiempoEntreAtaques;

            // GUARDAR DIRECCIÓN
            direccionAtaque =
                movimientoJugador.transform.forward;

            rotacionInicial =
                transform.localRotation;

            rotacionObjetivo =
                rotacionInicial *
                Quaternion.Euler(0, anguloAtaque, 0);

            col.enabled = true;
        }

        if (atacando)
            AnimarAtaque();
    }

    void AnimarAtaque()
    {
        progreso +=
            Time.deltaTime * velocidadAtaque;

        // AVANZAR AL ATACAR
        controller.Move(
            direccionAtaque.normalized *
            avanceAtaque *
            Time.deltaTime
        );

        if (!regresando)
        {
            transform.localRotation =
                Quaternion.Slerp(
                    rotacionInicial,
                    rotacionObjetivo,
                    progreso
                );

            if (progreso >= 1f)
            {
                progreso = 0f;
                regresando = true;
            }
        }
        else
        {
            transform.localRotation =
                Quaternion.Slerp(
                    rotacionObjetivo,
                    rotacionInicial,
                    progreso
                );

            if (progreso >= 1f)
            {
                atacando = false;

                transform.localRotation =
                    rotacionInicial;

                col.enabled = false;
            }
        }
    }

    // =========================
    // BUFF DE DAÑO
    // =========================
    void HabilidadBuff()
    {
        if (Input.GetKeyDown(teclaBuff)
            && !buffActivo)
        {
            // ACTIVAR EMISSION
            armaRenderer.material.EnableKeyword("_EMISSION");

            buffActivo = true;
            tiempoBuff = duracionBuff;

            dañoActual = Mathf.RoundToInt(
                dañoBase *
                multiplicadorDaño
            );

            Debug.Log("BUFF ACTIVADO");
        }

        if (buffActivo)
        {
            tiempoBuff -= Time.deltaTime;

            if (tiempoBuff <= 0)
            {
                // APAGAR EMISSION
                armaRenderer.material.DisableKeyword("_EMISSION");

                buffActivo = false;

                dañoActual = dañoBase;

                Debug.Log("BUFF TERMINADO");
            }
        }
    }

    // =========================
    // ULTIMATE CARGABLE
    // =========================
    void UltimateDash()
    {
        // EMPEZAR CARGA
        if (Input.GetKeyDown(teclaUlti))
        {
            cargandoUlti = true;

            tiempoCarga = 0f;

            sliderUlti.gameObject.SetActive(true);

            porcentajeTexto.gameObject.SetActive(true);
        }

        // CARGANDO
        if (cargandoUlti)
        {
            tiempoCarga += Time.deltaTime;

            // ZOOM MIENTRAS CARGA
            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(
                    cam.fieldOfView,
                    fovUlti,
                    velocidadZoom * Time.deltaTime
                );
            }

            // LIMITAR
            if (tiempoCarga > tiempoMaxCarga)
                tiempoCarga = tiempoMaxCarga;

            // ACTUALIZAR SLIDER
            sliderUlti.value = tiempoCarga;

            // PORCENTAJE
            int porcentajeUI =
                Mathf.RoundToInt(
                    (tiempoCarga / tiempoMaxCarga) * 100
                );

            porcentajeTexto.text =
                porcentajeUI + "%";
        }
        else
        {
            // VOLVER AL FOV NORMAL
            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(
                    cam.fieldOfView,
                    fovNormal,
                    velocidadZoom * Time.deltaTime
                );
            }
        }

        // SOLTAR BOTÓN
        if (Input.GetKeyUp(teclaUlti)
            && cargandoUlti)
        {
            cargandoUlti = false;

            // DESACTIVAR LOCK ON
            if (scriptCamara != null)
            {
                scriptCamara.DesactivarLock();
            }

            sliderUlti.gameObject.SetActive(false);

            porcentajeTexto.gameObject.SetActive(false);

            // CALCULAR DAÑO
            float porcentaje =
                tiempoCarga / tiempoMaxCarga;

            dañoUltiActual = Mathf.Lerp(
                dañoMinUlti,
                dañoMaxUlti,
                porcentaje
            );

            usandoUlti = true;
            tiempoUlti = duracionUlti;

            Debug.Log(
                "Daño Ulti: " +
                dañoUltiActual
            );
        }

        // HACER DASH
        if (usandoUlti)
        {
            Vector3 direccion =
                cam.transform.forward;

            // IGNORAR ALTURA
            direccion.y = 0;

            direccion.Normalize();

            controller.Move(
                direccion *
                fuerzaUlti *
                Time.deltaTime
            );

            tiempoUlti -= Time.deltaTime;

            if (tiempoUlti <= 0)
            {
                usandoUlti = false;
            }
        }
    }
}