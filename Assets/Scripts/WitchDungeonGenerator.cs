using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class WitchDungeonGenerator : MonoBehaviour
{
    [Header("Tiles")]
    public Tilemap tilemapSuelo;
    public TileBase tileSueloBruja;

    [Header("Tamaño Mazmorra")]
    public int minWidth = 16;
    public int maxWidth = 26;

    public int minHeight = 9;
    public int maxHeight = 14;

    [Header("Prefabs Decoración")]
    public GameObject prefabCaldero;
    public GameObject prefabMesa;
    public GameObject prefabCalavera;

    [Header("Elementos Principales")]
    public GameObject prefabBruja;
    public GameObject prefabAlfombraSalida;

    private int width;
    private int height;

    private Transform jugador;

    private Vector3 posicionSpawnJugador;

    private bool dungeonGenerada = false;

    // INICIO

    void Start()
    {
        StartCoroutine(InicializarDungeon());
    }

    IEnumerator InicializarDungeon()
    {
        yield return null;

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            jugador = playerObj.transform;
        }

        yield return StartCoroutine(GenerarMazmorra());

        dungeonGenerada = true;

        Debug.Log("WITCH DUNGEON GENERADA");
    }

    // GENERACIÓN PRINCIPAL

    IEnumerator GenerarMazmorra()
    {
        if (tilemapSuelo == null || tileSueloBruja == null)
        {
            Debug.LogError("Faltan referencias del Tilemap.");
            yield break;
        }

        // TAMAÑO PROCEDURAL

        width = Random.Range(minWidth, maxWidth + 1);
        height = Random.Range(minHeight, maxHeight + 1);

        tilemapSuelo.ClearAllTiles();

        yield return null;

        // GENERAR SUELO

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemapSuelo.SetTile(
                    new Vector3Int(x, y, 0),
                    tileSueloBruja
                );
            }
        }

        yield return null;

        // MUROS INVISIBLES

        CrearMuroInvisible(
            "Muro_Abajo",
            new Vector2(width / 2f, -0.5f),
            new Vector2(width, 1f)
        );

        CrearMuroInvisible(
            "Muro_Arriba",
            new Vector2(width / 2f, height + 0.5f),
            new Vector2(width, 1f)
        );

        CrearMuroInvisible(
            "Muro_Izquierda",
            new Vector2(-0.5f, height / 2f),
            new Vector2(1f, height)
        );

        CrearMuroInvisible(
            "Muro_Derecha",
            new Vector2(width + 0.5f, height / 2f),
            new Vector2(1f, height)
        );

        yield return null;

        // POSICIÓN SEGURA DEL JUGADOR

        posicionSpawnJugador =
            new Vector3(width / 2f, 1.5f, 0f);

        if (jugador != null)
        {
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
                rb.angularVelocity = 0f;

                rb.position =
                    (Vector2)posicionSpawnJugador;
            }
            else
            {
                jugador.position = posicionSpawnJugador;
            }

            yield return new WaitForSeconds(0.2f);

            if (col != null)
            {
                col.enabled = true;
            }
        }

        // ALFOMBRA SALIDA

        if (prefabAlfombraSalida != null)
        {
            GameObject alfombra =
                Instantiate(
                    prefabAlfombraSalida,
                    new Vector3(width / 2f, 0.5f, 0f),
                    Quaternion.identity
                );

            SpriteRenderer sr =
                alfombra.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sortingOrder = -950;
            }
        }

        yield return null;

        // CALDERO

        if (prefabCaldero != null)
        {
            float xCaldero =
                Random.Range(
                    1.5f,
                    (width / 2f) - 1.5f
                );

            float yCaldero =
                Random.Range(
                    height * 0.4f,
                    height - 1.5f
                );

            Instantiate(
                prefabCaldero,
                new Vector3(xCaldero, yCaldero, 0f),
                Quaternion.identity
            );
        }

        // MESA

        if (prefabMesa != null)
        {
            float xMesa =
                Random.Range(
                    (width / 2f) + 1.5f,
                    width - 1.5f
                );

            float yMesa =
                Random.Range(
                    height * 0.4f,
                    height - 1.5f
                );

            Instantiate(
                prefabMesa,
                new Vector3(xMesa, yMesa, 0f),
                Quaternion.identity
            );
        }

        // BRUJA

        if (prefabBruja != null)
        {
            float xBruja =
                Random.Range(2f, width - 2f);

            float yBruja =
                Random.Range(
                    height * 0.6f,
                    height - 2f
                );

            Instantiate(
                prefabBruja,
                new Vector3(xBruja, yBruja, 0f),
                Quaternion.identity
            );
        }

        yield return null;

        // CALAVERAS

        if (prefabCalavera != null)
        {
            int calaverasCreadas = 0;

            int intentosMaximos = 30;

            while (
                calaverasCreadas < 4 &&
                intentosMaximos > 0
            )
            {
                intentosMaximos--;

                float xRandom =
                    Random.Range(
                        1.5f,
                        width - 1.5f
                    );

                float yRandom =
                    Random.Range(
                        1.5f,
                        height - 1.5f
                    );

                Vector3 posicionCandidata =
                    new Vector3(
                        xRandom,
                        yRandom,
                        0f
                    );

                if (
                    Vector3.Distance(
                        posicionCandidata,
                        posicionSpawnJugador
                    ) > 2.5f
                )
                {
                    Instantiate(
                        prefabCalavera,
                        posicionCandidata,
                        Quaternion.identity
                    );

                    calaverasCreadas++;
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    // MUROS INVISIBLES

    void CrearMuroInvisible(
        string nombre,
        Vector2 posicion,
        Vector2 tamaño
    )
    {
        Transform muroViejo =
            transform.Find(nombre);

        if (muroViejo != null)
        {
            Destroy(muroViejo.gameObject);
        }

        GameObject muro =
            new GameObject(nombre);

        muro.transform.parent = transform;

        muro.transform.position = posicion;

        BoxCollider2D col =
            muro.AddComponent<BoxCollider2D>();

        col.size = tamaño;
    }

    // DEBUG

    public bool DungeonGenerada()
    {
        return dungeonGenerada;
    }
}