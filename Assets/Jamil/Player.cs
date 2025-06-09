using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayingField
    {
        Ground,
        Sky,

    }

    public PlayingField currentField=PlayingField.Ground;
    public float distancia = 5f;
    public LayerMask capaObjetivo;
    public float velocidadX = 5f;
    public float fuerzaSalto = 10f;
    public float suavidadRotacion = 10f;
    public Vector2 impulsoInicialAire;
    public float gananciaVelocidadAireY;
    public float fuerzaDeSaltoEnAire;
    private Rigidbody2D rb2d;
    private bool enSuelo;
    bool corrutinaDetectandoSalto=false;
    public float maxVelocidadVertical;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(currentField== PlayingField.Ground)
        {
            Moving();
            DetectJump();
        }
       
        else if(currentField== PlayingField.Sky)
        {
            FlightMode();
        }
       

        
    }
    void ChangePlayingField(PlayingField playingField)
    {
        currentField = playingField;
        Debug.Log("Entrando a modo:" + currentField);
    }

    void Moving()
    {
        rb2d.velocity = new Vector2(velocidadX, rb2d.velocity.y);
    }

    void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, fuerzaSalto);
    }

    void DetectJump()
    {
        Debug.DrawRay(transform.position, Vector2.down * distancia, Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distancia, capaObjetivo);

        enSuelo = hit.collider != null;

        if (enSuelo)
        {
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            Quaternion rotacionObjetivo = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionObjetivo, Time.deltaTime * suavidadRotacion);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        else
        {
            if (!corrutinaDetectandoSalto)
            {
               StartCoroutine(DetectJumpLongPress());
            }
        }

    }

    IEnumerator DetectJumpLongPress()
    {
        corrutinaDetectandoSalto = true;
        float timer = 0f;
        bool saltoDetectado = false;

        while (Input.GetKey(KeyCode.Space))
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                Debug.Log("Salto mantenido por más de 0.2 segundos.");
                saltoDetectado = true;
                break;
            }
            yield return null;
        }

        if (saltoDetectado)
        {
            GainMomentum(impulsoInicialAire);
            saltoDetectado = false;
        }

        corrutinaDetectandoSalto = false;
    }


    void GainMomentum(Vector2 impulso)
    {
        UpdateVelocityY(0);
        rb2d.AddForce(impulso, ForceMode2D.Impulse);

        if (currentField == PlayingField.Ground)
        {
            ChangePlayingField(PlayingField.Sky);
        }    
    }

    void UpdateVelocityY(float nuevaVelocidadY)
    {
       Vector2 velocidad= new Vector2(rb2d.velocity.x,nuevaVelocidadY);
       rb2d.velocity=velocidad;
    }

    void FlightMode()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateVelocityY(gananciaVelocidadAireY);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Vector2 impulso = new Vector2(0,fuerzaDeSaltoEnAire);
          
            if (rb2d.velocity.y < maxVelocidadVertical)
            {
                rb2d.AddForce(impulso);
            }
        }
    }

   
}
