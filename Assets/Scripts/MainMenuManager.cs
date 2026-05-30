using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject manualPanel;

    [Header("Configuración Pantalla Completa")]
    public Button fullScreenButton; 
    private bool isFullScreen = false;


    public void Jugar()
    {
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("Nivel_Procedural"); 
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void AbrirOpciones()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CerrarOpciones()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void AbrirManual()
    {
        mainMenuPanel.SetActive(false);
        manualPanel.SetActive(true);
    }

    public void CerrarManual()
    {
        mainMenuPanel.SetActive(true);
        manualPanel.SetActive(false);
        Debug.Log("¡El botón Volver del manual ha sido pulsado correctamente!");
    mainMenuPanel.SetActive(true);
    manualPanel.SetActive(false);
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen; 
        Screen.fullScreen = isFullScreen;
    }
    
}