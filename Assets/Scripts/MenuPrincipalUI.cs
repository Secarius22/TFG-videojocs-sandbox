using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuPrincipalUI : MonoBehaviour
{
    [Header("Configuración Nueva Partida")]
    public string escenaNuevaPartida = "Nivel_Procedural"; 

    [System.Serializable]
    private class DatosEscenaRapidos { public string nombreEscena; }

    public void BotonNuevaPartida()
    {
        SaveManager.debeCargarAlIniciar = false;
        SceneManager.LoadScene(escenaNuevaPartida);
    }

    public void BotonCargarPartida()
    {
        string ruta = Path.Combine(Application.persistentDataPath, "partida.json");

        if (File.Exists(ruta))
        {
            string jsonString = File.ReadAllText(ruta);
            DatosEscenaRapidos datos = JsonUtility.FromJson<DatosEscenaRapidos>(jsonString);
            
            SaveManager.debeCargarAlIniciar = true;
            SceneManager.LoadScene(datos.nombreEscena);
        }
        else
        {
            Debug.LogWarning("¡No hay ninguna partida guardada!");
        }
    }

    public void BotonSalir()
    {
        Application.Quit();
    }
}