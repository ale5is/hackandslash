using UnityEngine;

public class camara : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador;

    [Header("Lock On")]
    public string tagEnemigo = "Enemy";
    public float rangoBusqueda = 15f;

    public float distanciaMinimaLock = 2.5f; // 🔴 se suelta
    public float distanciaReLock = 4f;       // 🟢 vuelve a fijar

    private Transform enemigoFijado;
    private Transform enemigoGuardado;

    private bool lockActivo = false; // 🔥 control interno

    [Header("Cámara")]
    public Vector3 offset = new Vector3(0, 6, -8);
    public float suavizado = 8f;
    public float sensibilidadMouse = 3f;

    [Header("Colisión")]
    public LayerMask capasColision;
    public float distanciaMinimaCamara = 2f;

    [Header("Rotación")]
    public float minY = -20f;
    public float maxY = 70f;

    float rotX;
    float rotY;

    Vector3 posicionInicial;
    Quaternion rotacionInicial;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 euler = transform.eulerAngles;
        rotX = euler.y;
        rotY = euler.x;

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void LateUpdate()
    {
        if (!jugador) return;

        if (Input.GetMouseButtonDown(2))
            ToggleLock();

        if (lockActivo)
            AutoLockSystem();

        if (enemigoFijado && lockActivo)
            CamaraLock();
        else
            CamaraLibre();
    }

    // =========================
    // 🎯 TOGGLE (RUEDA)
    // =========================
    void ToggleLock()
    {
        if (!lockActivo)
        {
            BuscarEnemigo();
            enemigoGuardado = enemigoFijado;
            lockActivo = true;
        }
        else
        {
            DesactivarLock(); // 🔥 usa método compatible
        }
    }

    // =========================
    // 🔥 AUTO LOCK
    // =========================
    void AutoLockSystem()
    {
        if (!lockActivo) return;

        if (enemigoFijado != null)
        {
            float dist = Vector3.Distance(jugador.position, enemigoFijado.position);

            if (dist <= distanciaMinimaLock)
            {
                enemigoFijado = null;
                return;
            }

            return;
        }

        if (enemigoGuardado != null)
        {
            float dist = Vector3.Distance(jugador.position, enemigoGuardado.position);

            if (dist >= distanciaReLock && dist <= rangoBusqueda)
            {
                enemigoFijado = enemigoGuardado;
            }
        }
    }

    // =========================
    // 🔓 FREE CAMERA
    // =========================
    void CamaraLibre()
    {
        rotX += Input.GetAxis("Mouse X") * sensibilidadMouse;
        rotY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotY = Mathf.Clamp(rotY, minY, maxY);

        Quaternion rot = Quaternion.Euler(rotY, rotX, 0);

        Vector3 pos = jugador.position + rot * offset;
        pos = AjustarColision(pos);

        transform.position = Vector3.Lerp(transform.position, pos, suavizado * Time.deltaTime);
        transform.LookAt(jugador.position + Vector3.up * 1.5f);
    }

    // =========================
    // 🎯 LOCK CAMERA
    // =========================
    void CamaraLock()
    {
        if (!enemigoFijado) return;

        float dist = Vector3.Distance(jugador.position, enemigoFijado.position);

        Vector3 dir = enemigoFijado.position - jugador.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            jugador.rotation = Quaternion.Slerp(jugador.rotation, rot, 10f * Time.deltaTime);
        }

        if (dist > distanciaMinimaLock)
        {
            Vector3 dirCam = (jugador.position - enemigoFijado.position).normalized;
            dirCam.y = 0;

            Vector3 pos = jugador.position + dirCam * Mathf.Abs(offset.z);
            pos.y += offset.y;

            pos = AjustarColision(pos);

            transform.position = Vector3.Lerp(transform.position, pos, suavizado * Time.deltaTime);
        }

        Vector3 mid = (jugador.position + enemigoFijado.position) * 0.5f;
        transform.LookAt(mid + Vector3.up * 1.5f);
    }

    // =========================
    // 🔧 COLISIÓN
    // =========================
    Vector3 AjustarColision(Vector3 target)
    {
        Vector3 origin = jugador.position + Vector3.up * 1.5f;
        Vector3 dir = target - origin;

        float dist = dir.magnitude;
        dir.Normalize();

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, capasColision))
        {
            float finalDist = Mathf.Clamp(
                hit.distance - 0.2f,
                distanciaMinimaCamara,
                dist
            );

            return origin + dir * finalDist;
        }

        return target;
    }

    // =========================
    // 🎯 COMPATIBLE CON ARMAS
    // =========================
    public void DesactivarLock()
    {
        enemigoFijado = null;
        enemigoGuardado = null;
        lockActivo = false;

        var mov = jugador.GetComponent<MovPersonaje>();
        if (mov) mov.objetivoLock = null;
    }

    // =========================
    // 🎯 BUSCAR ENEMIGO
    // =========================
    void BuscarEnemigo()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(tagEnemigo);

        float bestDist = Mathf.Infinity;
        Transform best = null;

        foreach (var e in enemigos)
        {
            float d = Vector3.Distance(jugador.position, e.transform.position);

            if (d < bestDist && d <= rangoBusqueda)
            {
                bestDist = d;
                best = e.transform;
            }
        }

        enemigoFijado = best;

        var mov = jugador.GetComponent<MovPersonaje>();
        if (mov) mov.objetivoLock = enemigoFijado;
    }

    public void ResetearCamara()
    {
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        Vector3 e = rotacionInicial.eulerAngles;
        rotX = e.y;
        rotY = e.x;
    }
}