using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    [Header("Configuración de Viaje")]
    public string nombreEscenaDestino; 
    public bool esVolviendoAlMundo = false; 

    public static bool regresandoDeDungeon = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (esVolviendoAlMundo)
            {
                regresandoDeDungeon = true;
            }

            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}