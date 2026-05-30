using UnityEngine;

public class RomperPiedra : MonoBehaviour
{
    private Animator anim;
    private Collider2D col;

    [Header("Configuración de Audio")]
    [Tooltip("Arrastra aquí el sonido de la piedra rompiéndose")]
    public AudioClip sonidoRuptura;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void EjecutarRuptura()
    {

        if (col != null) col.enabled = false;
        if (sonidoRuptura != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRuptura, transform.position);
        }

        if (anim != null) anim.SetTrigger("Ruptura");

        Destroy(gameObject, 0.5f); 
    }
}