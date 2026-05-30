using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform puntoAtaque; 
    public float radioAtaque = 1f; 
    public LayerMask capasInteractuables; 

    [Header("Configuración de Combate")]
    public float tiempoEntreAtaques = 0.5f; 
    private float siguienteAtaqueTiempo = 0f;

    [Header("Configuración de Audio")]
    [Tooltip("Arrastra aquí el componente AudioSource de tu Caballero")]
    public AudioSource miAudioSource;
    [Tooltip("Arrastra aquí el sonido .wav o .mp3 del espadazo/ataque")]
    public AudioClip sonidoAtaque;

    public void ActualizarPosicionAtaque(Vector2 direccion)
    {
        if (puntoAtaque != null)
        {
            puntoAtaque.localPosition = direccion.normalized * 0.5f;
        }
    }

    public void DetectarImpacto()
    {
        if (Time.time < siguienteAtaqueTiempo)
        {
            return;
        }

        siguienteAtaqueTiempo = Time.time + tiempoEntreAtaques;

        if (miAudioSource != null && sonidoAtaque != null)
        {
            miAudioSource.PlayOneShot(sonidoAtaque);
        }

        Collider2D[] objetosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque, capasInteractuables);
        
        foreach (Collider2D obj in objetosGolpeados)
        {
            Health enemigo = obj.GetComponent<Health>() 
                          ?? obj.GetComponentInChildren<Health>() 
                          ?? obj.GetComponentInParent<Health>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(10f); 
                Debug.Log("¡Golpeaste a un enemigo! Vida actual: " + enemigo.vidaActual);
                continue; 
            }

            RomperPiedra piedra = obj.GetComponent<RomperPiedra>() 
                               ?? obj.GetComponentInChildren<RomperPiedra>() 
                               ?? obj.GetComponentInParent<RomperPiedra>();
            
            if (piedra != null)
            {
                piedra.EjecutarRuptura();
            }
            else
            {
                Debug.LogWarning("Golpeaste a: " + obj.name + " pero no tiene script de daño ni de ruptura");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.yellow; 
            Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);
        }
    }
}