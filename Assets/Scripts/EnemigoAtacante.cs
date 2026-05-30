using UnityEngine;

public class EnemigoAtacante : MonoBehaviour
{
    public float daño = 10f;
    public float tiempoEntreAtaques = 1.5f;
    private float tiempoUltimoAtaque;

   
    private void OnCollisionStay2D(Collision2D collision)
    {
        ComprobarJugador(collision.gameObject);
    }

  
    private void OnTriggerStay2D(Collider2D collision)
    {
        ComprobarJugador(collision.gameObject);
    }

    private void ComprobarJugador(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                Health saludJugador = obj.GetComponent<Health>();
                if (saludJugador != null)
                {
                    saludJugador.RecibirDaño(daño);
                    tiempoUltimoAtaque = Time.time;
                    Debug.Log("¡Daño realizado al jugador!");
                }
            }
        }
    }
}