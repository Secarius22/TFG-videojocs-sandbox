using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    public Transform puntoAtaque; 
    public float radioAtaque = 1f;
    public float daño = 10f;
    public float cadenciaAtaque = 1.5f;
    
    private float tiempoSiguienteAtaque;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Time.time >= tiempoSiguienteAtaque)
        {
           
            Collider2D[] objetosTocados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque);
            
            foreach (Collider2D obj in objetosTocados)
            {
                
                if (obj.CompareTag("Player"))
                {
                    
                    HealthPlayer saludJugador = obj.GetComponent<HealthPlayer>();
                    
                    if (saludJugador != null)
                    {
                        saludJugador.RecibirDaño(daño);
                        tiempoSiguienteAtaque = Time.time + cadenciaAtaque;
                        break; 
                    }
                }
            }
        }
    }

    
    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);
        }
    }
}