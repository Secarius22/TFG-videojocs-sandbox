using UnityEngine;

public class WitchIA : MonoBehaviour
{
    public Transform jugador;

    [Header("Detección")]
    public float rangoAtaque = 2.5f; 

    [Header("Configuración Animator")]
    [SerializeField] private string parametroAtaque = "isAttacking"; 

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        BuscarJugadorVivo();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (jugador == null || jugador.GetComponent<HealthPlayer>() == null || jugador.GetComponent<HealthPlayer>().vidaActual <= 0)
        {
            BuscarJugadorVivo();
            if (jugador == null) return; 
        }

        if (anim == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoAtaque)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero; 
            anim.SetBool(parametroAtaque, true);
        }
        else
        {
            anim.SetBool(parametroAtaque, false);
        }
    }

    void BuscarJugadorVivo()
    {
        HealthPlayer[] todosLosJugadores = FindObjectsOfType<HealthPlayer>();
        foreach (HealthPlayer hp in todosLosJugadores)
        {
            if (hp.vidaActual > 0)
            {
                jugador = hp.transform;
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}