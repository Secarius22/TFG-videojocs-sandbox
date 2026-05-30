using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider barraVida;
    public float vidaMaxima = 125f;
    public float vidaActual;
    private bool esJugador;

    [Header("Configuración de Audio")]
    [Tooltip("Arrastra aquí el sonido de muerte específico de este personaje")]
    public AudioClip sonidoMuerte;

    void Start()
    {
        vidaActual = vidaMaxima;
        esJugador = gameObject.CompareTag("Player");

        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
            if (!esJugador) barraVida.gameObject.SetActive(false);
        }
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual < 0) vidaActual = 0;

        if (barraVida != null)
        {
            if (!esJugador) barraVida.gameObject.SetActive(true);
            barraVida.value = vidaActual;
        }

        Debug.Log(gameObject.name + " recibió " + cantidad + " de daño. Vida: " + vidaActual);

        if (vidaActual <= 0) Morir();
    }

    void Morir()
    {

        if (sonidoMuerte != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position);
        }

        Destroy(gameObject);
    }
}