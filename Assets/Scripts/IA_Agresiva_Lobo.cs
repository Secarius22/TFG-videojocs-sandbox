using UnityEngine;

public class IA_Agresiva_Lobo : MonoBehaviour
{
    public Transform jugador;

    [Header("Detección")]
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.2f;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2.5f;

    private LoboPatrulla patrulla;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 ultimaDireccion = Vector2.down;

    void Start()
    {
        patrulla = GetComponent<LoboPatrulla>();
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
                
                anim.SetBool("Atacar", false);
                anim.SetBool("IsMoving", true);
                
                anim.SetFloat("Horizontal", direccion.x);
                anim.SetFloat("Vertical", direccion.y);
            }
            else
            {
  
                rb.linearVelocity = Vector2.zero;
                
                anim.SetBool("IsMoving", false);
                anim.SetBool("Atacar", true);

                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 0);

                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Atacar"))
                {
                    anim.Play("Atacar", 0, 0f);
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
            anim.SetBool("Atacar", false);
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