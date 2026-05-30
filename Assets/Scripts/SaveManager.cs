using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private string rutaGuardado;
    public static bool debeCargarAlIniciar = false;
    public static bool cargandoPartida = false;

    [Header("UI Al Cargar Partida")]
    public GameObject panelGameOver;

    [System.Serializable]
    public class DatosGuardado
    {
        public string nombreEscena;
        public float playerX;
        public float playerY;
        public int playerHealth;
        public float salaX;
        public float salaY;
        public List<Vector2> mapaVisitadoKeys = new List<Vector2>();
        public List<int> mapaVisitadoValues = new List<int>();
        public bool esCabaña = false;
    }

    void Start()
    {
        rutaGuardado = Path.Combine(Application.persistentDataPath, "partida.json");
        if (debeCargarAlIniciar)
        {
            debeCargarAlIniciar = false;
            StartCoroutine(CargarPartidaConRetraso());
        }
    }

    public void GuardarPartida()
    {
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj == null) return;

        HealthPlayer hp = jugadorObj.GetComponent<HealthPlayer>();
        if (hp == null) return;

        DatosGuardado datos = new DatosGuardado();
        datos.nombreEscena = SceneManager.GetActiveScene().name;
        datos.playerX = jugadorObj.transform.position.x;
        datos.playerY = jugadorObj.transform.position.y;
        datos.playerHealth = (int)hp.vidaActual;

        WorldGenerator wg = FindAnyObjectByType<WorldGenerator>();

        if (wg != null && datos.nombreEscena == "Nivel_Procedural")
        {
            datos.esCabaña = false;
            datos.salaX = wg.GetSalaActual().x;
            datos.salaY = wg.GetSalaActual().y;
            datos.mapaVisitadoKeys.Clear();
            datos.mapaVisitadoValues.Clear();
            foreach (KeyValuePair<Vector2, int> par in wg.GetMapaVisitado())
            {
                datos.mapaVisitadoKeys.Add(par.Key);
                datos.mapaVisitadoValues.Add(par.Value);
            }
        }
        else
        {
            datos.esCabaña = true;
        }

        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaGuardado, json);
    }

    public void CargarPartida()
    {
        if (!File.Exists(rutaGuardado)) return;
        string json = File.ReadAllText(rutaGuardado);
        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        cargandoPartida = true;
        debeCargarAlIniciar = true;
        SceneManager.LoadScene(datos.nombreEscena);
    }

    IEnumerator CargarPartidaConRetraso()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(EjecutarCargaDeDatos());
    }

    IEnumerator EjecutarCargaDeDatos()
    {
        if (!File.Exists(rutaGuardado)) yield break;

        string json = File.ReadAllText(rutaGuardado);
        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        if (SceneManager.GetActiveScene().name != datos.nombreEscena)
        {
            debeCargarAlIniciar = true;
            SceneManager.LoadScene(datos.nombreEscena);
            yield break;
        }

        if (!datos.esCabaña)
        {
            WorldGenerator wg = FindAnyObjectByType<WorldGenerator>();
            if (wg != null)
            {
                Dictionary<Vector2, int> mapaReconstruido = new Dictionary<Vector2, int>();
                for (int i = 0; i < datos.mapaVisitadoKeys.Count; i++)
                    mapaReconstruido[datos.mapaVisitadoKeys[i]] = datos.mapaVisitadoValues[i];

                wg.SetMapaVisitado(mapaReconstruido);
                wg.SetSalaActual(new Vector2(datos.salaX, datos.salaY));
                wg.ForzarGeneracionSalaActual();
                
                yield return new WaitForSeconds(0.6f); 
            }
        }

        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            Rigidbody2D rb = jugadorObj.GetComponent<Rigidbody2D>();
            Collider2D col = jugadorObj.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            if (rb != null) rb.simulated = false;

            Vector3 posCarga = new Vector3(datos.playerX, datos.playerY + 0.5f, 0);
            jugadorObj.transform.position = posCarga;

            Physics2D.SyncTransforms(); 

            yield return new WaitForEndOfFrame();

            if (rb != null) rb.simulated = true;
            if (col != null) col.enabled = true;

            HealthPlayer hp = jugadorObj.GetComponent<HealthPlayer>();
            if (hp != null) hp.CargarVida(datos.playerHealth);
        }

        Time.timeScale = 1f;
        ControlPausa scriptPausa = FindAnyObjectByType<ControlPausa>();
        if (scriptPausa != null) scriptPausa.Reanudar();
        if (panelGameOver != null) panelGameOver.SetActive(false);

        cargandoPartida = false;
    }
}