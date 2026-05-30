using UnityEngine;

public class IA_Agresiva : MonoBehaviour
{
    public Transform jugador;

    [Header("Detección")]
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.2f;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2.5f;

    private EsqueletoPatrulla patrulla;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 ultimaDireccion = Vector2.down;

    void Start()
    {
        patrulla = GetComponent<EsqueletoPatrulla>();
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

        if (distancia <= rangoDeteccion)
        {
            patrulla.enModoPersecucion = true;
            Vector2 direccion = (jugador.position - transform.position).normalized;
            if (direccion != Vector2.zero) ultimaDireccion = direccion;

            if (distancia > rangoAtaque)
            {
                rb.linearVelocity = direccion * velocidadPersecucion;
                
                anim.SetBool("Atacando", false);
                anim.SetBool("IsMoving", true);
                
                anim.SetFloat("Horizontal", direccion.x);
                anim.SetFloat("Vertical", direccion.y);
                anim.SetFloat("LastHorz", direccion.x);
                anim.SetFloat("LastVert", direccion.y);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                
                anim.SetBool("IsMoving", false);
                anim.SetBool("Atacando", true);

                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 0);
                anim.SetFloat("LastHorz", 0);
                anim.SetFloat("LastVert", 0);

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Combate"))
                {
                    anim.Play("Combate", 0, 0f);
                    anim.Update(0); 
                }

                float h = 0;
                float v = 0;

                if (Mathf.Abs(ultimaDireccion.y) > Mathf.Abs(ultimaDireccion.x))
                {
                    v = (ultimaDireccion.y > 0) ? 1f : -1f;
                }
                else
                {
                    h = (ultimaDireccion.x > 0) ? 1f : -1f;
                }

                anim.SetFloat("AtkHorz", h);
                anim.SetFloat("AtkVert", v);
            }
        }
        else
        {
            patrulla.enModoPersecucion = false;
            anim.SetBool("Atacando", false);
            anim.SetFloat("AtkHorz", 0);
            anim.SetFloat("AtkVert", 0);
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
}