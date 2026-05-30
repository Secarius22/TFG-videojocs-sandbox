using UnityEngine;

public class EnemigoPatrulla : MonoBehaviour
{
    public float velocidad = 2f;
    public float tiempoEspera = 2f;
    private Vector2 direccionActual;
    private float timer;
    private bool estaMoviendo;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponentInChildren<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        CambiarDireccion();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            CambiarDireccion();
        }

        rb.linearVelocity = direccionActual * velocidad;

        anim.SetBool("IsMoving", estaMoviendo);

        if (estaMoviendo)
        {
            anim.SetFloat("Horizontal", direccionActual.x);
            anim.SetFloat("Vertical", direccionActual.y);

            anim.SetFloat("LastHorz", direccionActual.x);
            anim.SetFloat("LastVert", direccionActual.y);
        }
    }

    void CambiarDireccion()
    {
        estaMoviendo = !estaMoviendo;
        timer = tiempoEspera;

        if (estaMoviendo)
        {
            int opcion = Random.Range(0, 4);
            switch(opcion)
            {
                case 0: direccionActual = Vector2.right; break;
                case 1: direccionActual = Vector2.left; break;
                case 2: direccionActual = Vector2.up; break;
                case 3: direccionActual = Vector2.down; break;
            }
        }
        else
        {
            direccionActual = Vector2.zero;
        }
    }
}