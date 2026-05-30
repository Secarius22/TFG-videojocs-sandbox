using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{
    public float vidaMaxima = 125f;
    public float vidaActual;
    public Slider barraVida; 

    [Header("Regeneración de Vida")]
    [Tooltip("Cantidad de puntos de vida que recupera el personaje por segundo")]
    public float regeneracionPorSegundo = 1.5f; 

    [Header("Menu de Muerte")]
    public GameObject panelGameOver; 

    private bool estaMuerto = false;

    void Start()
    {

        Time.timeScale = 1f; 

        if (vidaActual <= 0)
        {
            vidaActual = vidaMaxima;
        }
        
        estaMuerto = (vidaActual <= 0); 
        
        
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual; 
        }
    }


    void Update()
    {

        if (!estaMuerto && vidaActual < vidaMaxima)
        {

            vidaActual += regeneracionPorSegundo * Time.deltaTime;


            if (vidaActual > vidaMaxima) 
            {
                vidaActual = vidaMaxima;
            }

            if (barraVida != null) 
            {
                barraVida.value = vidaActual;
            }
        }
    }

    public void RecibirDaño(float cantidad)
    {
        if (estaMuerto) return; 

        vidaActual -= cantidad;
        Debug.Log("¡El jugador recibió " + cantidad + " de daño! Vida restante: " + vidaActual);

        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            estaMuerto = true; 
            Debug.Log("El jugador ha muerto.");
            
            if (panelGameOver != null)
            {
                panelGameOver.SetActive(true); 
            }
            Time.timeScale = 0f; 
        }
    }

    public void CargarVida(float vidaCargada)
    {
        vidaActual = vidaCargada;

        if (vidaActual > 0)
        {
            estaMuerto = false;
        }
        else
        {
            estaMuerto = true;
        }

        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }
    }
}