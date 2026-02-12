using UnityEngine;

public class FallingPlatformClase : MonoBehaviour
{
    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    [SerializeField] private float velocidad = 2.0f;
    [SerializeField] private GameObject destino;

    [SerializeField] private float retardoAntesDeCaer = 0.75f; // entre 0.5 y 1
    [SerializeField] private float esperaAbajo = 0.75f;        // espera antes de volver
    [SerializeField] private float umbralNormalArriba = 0.5f;  // normal.y > 0.5 = viene desde arriba

    private Rigidbody rb;

    // Estados
    private enum Estado { Idle, CuentaAtras, Cayendo, EsperandoAbajo, Volviendo }
    private Estado estado = Estado.Idle;

    private float tiempoInicioCuentaAtras = -1f;
    private float tiempoInicioEsperaAbajo = -1f;

    private void Start()
    {
        posicionInicial = transform.position;
        posicionFinal = destino.transform.position;

        
        var mr = destino.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        rb = GetComponent<Rigidbody>();

        // Recomendado para plataformas: que no le afecte la gravedad ni empuje cosas raras
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        switch (estado)
        {
            case Estado.CuentaAtras:
                // La cuenta atrás sigue aunque el jugador se vaya
                if (Time.time - tiempoInicioCuentaAtras >= retardoAntesDeCaer)
                {
                    estado = Estado.Cayendo;
                }
                break;

            case Estado.Cayendo:
            {
                Vector3 newPosition = Vector3.MoveTowards(
                    transform.position,
                    posicionFinal,
                    velocidad * Time.fixedDeltaTime
                );

                rb.MovePosition(newPosition);

                if (Vector3.Distance(transform.position, posicionFinal) < 0.01f)
                {
                    estado = Estado.EsperandoAbajo;
                    tiempoInicioEsperaAbajo = Time.time;
                }
                break;
            }

            case Estado.EsperandoAbajo:
                if (Time.time - tiempoInicioEsperaAbajo >= esperaAbajo)
                {
                    estado = Estado.Volviendo;
                }
                break;

            case Estado.Volviendo:
            {
                Vector3 newPosition = Vector3.MoveTowards(
                    transform.position,
                    posicionInicial,
                    velocidad * Time.fixedDeltaTime
                );

                rb.MovePosition(newPosition);

                if (Vector3.Distance(transform.position, posicionInicial) < 0.01f)
                {
                    // Resetea para que vuelva a funcionar como al principio
                    estado = Estado.Idle;
                    tiempoInicioCuentaAtras = -1f;
                    tiempoInicioEsperaAbajo = -1f;
                }
                break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (estado != Estado.Idle) return;
        if (!collision.collider.CompareTag("Player")) return;


        // Iniciar cuenta atrás usando Time.time
        tiempoInicioCuentaAtras = Time.time;
        estado = Estado.CuentaAtras;
    }
}