using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    // MEMORIA ESTÁTICA

    public static Dictionary<Vector2, int> mapaVisitadoEstatico =
        new Dictionary<Vector2, int>();

    public static Vector2 salaActualEstatica = Vector2.zero;

    public static bool datosGuardados = false;

    // REFERENCIAS

    [Header("Referencias de Tiles")]
    public Tilemap tilemap;
    public Tilemap tilemapColisiones;

    public TileBase tileHierba;
    public TileBase tileTierra;

    public TileBase paredArriba;
    public TileBase paredAbajo;
    public TileBase paredIzq;
    public TileBase paredDer;

    public TileBase esquinaSupIzq;
    public TileBase esquinaSupDer;
    public TileBase esquinaInfIzq;
    public TileBase esquinaInfDer;

    [Header("Objetos")]
    public GameObject prefabPortal;
    public GameObject[] decoracionesPrefabs;

    [Header("Configuración Enemigos")]
    public GameObject[] enemigosPrefabs;
    public int cantidadEnemigosPorSala = 3;

    [Header("Cabaña Bruja")]
    public GameObject prefabCabaña;

    [Range(0, 100)]
    public float probabilidadCabaña = 30f;

    [Header("Mapa")]
    public int width = 50;
    public int height = 50;

    public float scale = 10f;

    public Transform jugador;

    // RUNTIME

    public Dictionary<Vector2, int> mapaVisitado =
        new Dictionary<Vector2, int>();

    public HashSet<Vector2> posicionesOcupadas =
        new HashSet<Vector2>();

    public Vector2 salaActual = Vector2.zero;

    private bool generandoSala = false;

    // START

    void Start()
    {
        StartCoroutine(InicializarMundo());
    }

    IEnumerator InicializarMundo()
    {
        yield return null;

        if (jugador == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
        }

        if (datosGuardados)
        {
            mapaVisitado = mapaVisitadoEstatico;
            salaActual = salaActualEstatica;
        }

        yield return StartCoroutine(GenerarSalaCoroutine(salaActual));

        yield return null;
        yield return new WaitForEndOfFrame();

        // POSICIONAR JUGADOR

        if (jugador != null)
        {

            if (CambioEscena.regresandoDeDungeon)
            {
                GameObject cabañaObj =
                    GameObject.FindGameObjectWithTag("Cabaña");

                if (cabañaObj != null)
                {
                    Vector3 posicionPuerta =
                        cabañaObj.transform.position +
                        new Vector3(0f, -5f, 0f);

                    Rigidbody2D rb =
                        jugador.GetComponent<Rigidbody2D>();

                    Collider2D col =
                        jugador.GetComponent<Collider2D>();

                    if (col != null)
                    {
                        col.enabled = false;
                    }

                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                        rb.position = posicionPuerta;
                    }
                    else
                    {
                        jugador.position = posicionPuerta;
                    }

                    yield return new WaitForSeconds(0.3f);

                    if (col != null)
                    {
                        col.enabled = true;
                    }
                }
                else
                {
                    jugador.position =
                        new Vector3(width / 2, height / 2, 0);
                }

                CambioEscena.regresandoDeDungeon = false;
            }

            // INICIO NORMAL

            else if (!SaveManager.cargandoPartida)
            {
                Rigidbody2D rb =
                    jugador.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.position =
                        new Vector2(width / 2, height / 2);
                }
                else
                {
                    jugador.position =
                        new Vector3(width / 2, height / 2, 0);
                }
            }
        }

        Debug.Log("WORLD GENERATOR INICIALIZADO");
    }

    // CAMBIO DE SALA

    public void IrASala(Vector2 direccion)
    {
        if (jugador == null || generandoSala)
        {
            return;
        }

        StartCoroutine(IrASalaCoroutine(direccion));
    }

    IEnumerator IrASalaCoroutine(Vector2 direccion)
    {
        generandoSala = true;

        Collider2D col = jugador.GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        salaActual += direccion;

        yield return StartCoroutine(
            GenerarSalaCoroutine(salaActual)
        );

        float newX =
            (direccion.x > 0) ? 6f :
            (direccion.x < 0) ? 43f :
            width / 2;

        float newY =
            (direccion.y > 0) ? 6f :
            (direccion.y < 0) ? 43f :
            height / 2;

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(newX, newY);
        }
        else
        {
            jugador.position =
                new Vector3(newX, newY, 0);
        }

        yield return new WaitForSeconds(0.2f);

        if (col != null)
        {
            col.enabled = true;
        }

        generandoSala = false;
    }

    // GENERACIÓN DE SALA

    IEnumerator GenerarSalaCoroutine(Vector2 posicion)
    {
        if (tilemap == null || tilemapColisiones == null)
        {
            yield break;
        }

        tilemap.ClearAllTiles();
        tilemapColisiones.ClearAllTiles();

        posicionesOcupadas.Clear();

        yield return null;

        // Limpiar objetos
        LimpiarObjetosConTag("Portal");
        LimpiarObjetosConTag("Decoracion");
        LimpiarObjetosConTag("Enemigo");
        LimpiarObjetosConTag("Cabaña");

        // SEMILLA FIJA

        int semilla;

        if (mapaVisitado.ContainsKey(posicion))
        {
            semilla = mapaVisitado[posicion];
        }
        else
        {
            semilla = Random.Range(0, 100000);

            mapaVisitado.Add(posicion, semilla);
        }

        Random.State oldState = Random.state;

        Random.InitState(semilla);

        float xOffset = Random.Range(0f, 1000f);
        float yOffset = Random.Range(0f, 1000f);

        GenerateMap(xOffset, yOffset);

        yield return null;

        GenerarCabañaAleatoria();

        ColocarPortales();

        GenerarDecoracionAleatoria();

        GenerarEnemigos();

        Random.state = oldState;
        mapaVisitadoEstatico = mapaVisitado;
        salaActualEstatica = salaActual;
        datosGuardados = true;

        yield return null;
    }

    // LIMPIEZA

    void LimpiarObjetosConTag(string tag)
    {
        GameObject[] objetos =
            GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objetos)
        {
            Destroy(obj);
        }
    }

    // CABAÑA

    void GenerarCabañaAleatoria()
    {
        if (prefabCabaña == null)
        {
            return;
        }

        if (Random.Range(0f, 100f) > probabilidadCabaña)
        {
            return;
        }

        Vector2 posCabaña = Vector2.zero;

        bool posicionValida = false;

        int intentos = 0;

        while (!posicionValida && intentos < 100)
        {
            intentos++;

            int rx = Random.Range(10, width - 10);
            int ry = Random.Range(10, height - 10);

            if (
                Mathf.Abs(rx - width / 2) > 4 &&
                Mathf.Abs(ry - height / 2) > 4
            )
            {
                posCabaña = new Vector2(rx, ry);
                posicionValida = true;
            }
        }

        if (!posicionValida)
        {
            return;
        }

        GameObject cabaña =
            Instantiate(
                prefabCabaña,
                new Vector3(posCabaña.x, posCabaña.y, 0),
                Quaternion.identity
            );

        cabaña.tag = "Cabaña";

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                posicionesOcupadas.Add(
                    new Vector2(
                        posCabaña.x + x,
                        posCabaña.y + y
                    )
                );
            }
        }
    }

    // ENEMIGOS

    void GenerarEnemigos()
    {
        if (
            enemigosPrefabs == null ||
            enemigosPrefabs.Length == 0
        )
        {
            return;
        }

        for (int i = 0; i < cantidadEnemigosPorSala; i++)
        {
            Vector2 pos =
                new Vector2(
                    Random.Range(4, width - 4),
                    Random.Range(4, height - 4)
                );

            if (!posicionesOcupadas.Contains(pos))
            {
                GameObject prefab =
                    enemigosPrefabs[
                        Random.Range(
                            0,
                            enemigosPrefabs.Length
                        )
                    ];

                if (prefab != null)
                {
                    GameObject e =
                        Instantiate(
                            prefab,
                            new Vector3(pos.x, pos.y, 0),
                            Quaternion.identity
                        );

                    e.tag = "Enemigo";

                    posicionesOcupadas.Add(pos);
                }
            }
        }
    }

    // PORTALES

    void ColocarPortales()
    {
        CrearPortal(
            new Vector3(46, height / 2, 0),
            new Vector2(1, 0)
        );

        CrearPortal(
            new Vector3(3, height / 2, 0),
            new Vector2(-1, 0)
        );

        CrearPortal(
            new Vector3(width / 2, 46, 0),
            new Vector2(0, 1)
        );

        CrearPortal(
            new Vector3(width / 2, 3, 0),
            new Vector2(0, -1)
        );
    }

    void CrearPortal(Vector3 pos, Vector2 dir)
    {
        if (prefabPortal == null)
        {
            return;
        }

        GameObject p =
            Instantiate(prefabPortal, pos, Quaternion.identity);

        p.tag = "Portal";

        posicionesOcupadas.Add(new Vector2(pos.x, pos.y));

        DetectorSalida detector =
            p.GetComponent<DetectorSalida>();

        if (detector != null)
        {
            detector.direccion = dir;
        }
    }

    // DECORACIÓN

    void GenerarDecoracionAleatoria()
    {
        if (
            decoracionesPrefabs == null ||
            decoracionesPrefabs.Length == 0
        )
        {
            return;
        }

        int cantidadObjetos =
            Random.Range(5, 15);

        for (int i = 0; i < cantidadObjetos; i++)
        {
            Vector2 pos =
                new Vector2(
                    Random.Range(3, width - 3),
                    Random.Range(3, height - 3)
                );

            if (!posicionesOcupadas.Contains(pos))
            {
                GameObject prefab =
                    decoracionesPrefabs[
                        Random.Range(
                            0,
                            decoracionesPrefabs.Length
                        )
                    ];

                if (prefab != null)
                {
                    GameObject obj =
                        Instantiate(
                            prefab,
                            new Vector3(pos.x, pos.y, 0),
                            Quaternion.identity
                        );

                    obj.tag = "Decoracion";

                    posicionesOcupadas.Add(pos);
                }
            }
        }
    }

    // TILEMAP

    void GenerateMap(float xOffset, float yOffset)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool esBorde =
                    (
                        x == 0 ||
                        x == width - 1 ||
                        y == 0 ||
                        y == height - 1
                    );

                if (esBorde)
                {
                    TileBase tileAUsar = paredArriba;

                    if (x == 0 && y == 0)
                        tileAUsar = esquinaInfIzq;

                    else if (x == 0 && y == height - 1)
                        tileAUsar = esquinaSupIzq;

                    else if (x == width - 1 && y == 0)
                        tileAUsar = esquinaInfDer;

                    else if (x == width - 1 && y == height - 1)
                        tileAUsar = esquinaSupDer;

                    else if (x == 0)
                        tileAUsar = paredIzq;

                    else if (x == width - 1)
                        tileAUsar = paredDer;

                    else if (y == 0)
                        tileAUsar = paredAbajo;

                    else if (y == height - 1)
                        tileAUsar = paredArriba;

                    tilemapColisiones.SetTile(
                        new Vector3Int(x, y, 0),
                        tileAUsar
                    );

                    posicionesOcupadas.Add(
                        new Vector2(x, y)
                    );
                }
                else
                {
                    float noise =
                        Mathf.PerlinNoise(
                            (x + xOffset) / scale,
                            (y + yOffset) / scale
                        );

                    tilemap.SetTile(
                        new Vector3Int(x, y, 0),
                        noise > 0.5f
                            ? tileHierba
                            : tileTierra
                    );
                }
            }
        }
    }

    // GETTERS / SETTERS


    public Dictionary<Vector2, int> GetMapaVisitado()
    {
        return mapaVisitado;
    }

    public void SetMapaVisitado(
        Dictionary<Vector2, int> nuevoMapa
    )
    {
        mapaVisitado = nuevoMapa;
    }

    public Vector2 GetSalaActual()
    {
        return salaActual;
    }

    public void SetSalaActual(Vector2 nuevaSala)
    {
        salaActual = nuevaSala;
    }

    public void ForzarGeneracionSalaActual()
    {
        StartCoroutine(
            GenerarSalaCoroutine(salaActual)
        );
    }
}