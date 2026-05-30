using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlPausa : MonoBehaviour
{
    public GameObject objetoMenu; 
    private bool estaPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        objetoMenu.SetActive(true); 
        Time.timeScale = 0f;        
        estaPausado = true;
    }

    public void Reanudar()
    {
        objetoMenu.SetActive(false); 
        Time.timeScale = 1f;         
        estaPausado = false;
    }

    public void Salir()
    {
        Application.Quit(); 
        Debug.Log("Saliendo del juego..."); 
    }
    
public void VolverAlMenu()
    {
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("MainMenu"); 
    }
}

