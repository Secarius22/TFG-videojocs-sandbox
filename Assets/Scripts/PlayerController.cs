using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    private Vector2 direccion;
    private Rigidbody2D rb;
    private Animator animatorHijo; 
    
    private PlayerCombat playerCombat;
    private Vector2 ultimaDireccion = Vector2.down; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animatorHijo = GetComponentInChildren<Animator>();
        playerCombat = GetComponentInChildren<PlayerCombat>();
    }

    public void OnMove(InputValue value)
    {
        direccion = value.Get<Vector2>();
        
        if (direccion.sqrMagnitude > 0.01f)
        {
            ultimaDireccion = direccion;
            if (playerCombat != null)
            {
                playerCombat.ActualizarPosicionAtaque(direccion);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direccion * velocidad * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (animatorHijo != null) 
            {
                animatorHijo.SetTrigger("Atacar");
            }
            
            if (playerCombat != null)
            {
                playerCombat.DetectarImpacto();
            }
        }
        
        if (animatorHijo != null)
        {
            if (direccion.sqrMagnitude > 0.01f)
            {
                animatorHijo.SetFloat("Horizontal", direccion.x);
                animatorHijo.SetFloat("Vertical", direccion.y);
                animatorHijo.SetFloat("LastHorizontal", direccion.x);
                animatorHijo.SetFloat("LastVertical", direccion.y);
                animatorHijo.SetBool("IsMoving", true);
            }
            else
            {
                animatorHijo.SetBool("IsMoving", false);
            }
        }
    }
}