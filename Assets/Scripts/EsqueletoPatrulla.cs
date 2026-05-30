using UnityEngine;

public class EsqueletoPatrulla : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 2f;
    public float tiempoEspera = 2f;
    public bool enModoPersecucion = false; 

    private Vector2 direccionActual;
    private float timer;
    private bool estaMoviendo;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponentInChildren<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
        
        enModoPersecucion = false;
        CambiarDireccion();
    }

  void Update()
{
    if (Time.timeScale == 0f) return;
    if (!enModoPersecucion)
    {
        timer -= Time.deltaTime;
        if (timer <= 0) CambiarDireccion();

        rb.linearVelocity = direccionActual * velocidad;

        if (anim != null)
        {
            anim.SetBool("IsMoving", estaMoviendo);
            if (estaMoviendo)
            {
                anim.SetFloat("Horizontal", direccionActual.x);
                anim.SetFloat("Vertical", direccionActual.y);
            }
        }
    }

    else
    {
     
        rb.linearVelocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("IsMoving", false);
        }
      
        timer = tiempoEspera; 
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