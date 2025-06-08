using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float distancia = 5f;
    public LayerMask capaObjetivo;
    public float velocidadX = 5f;
    public float fuerzaSalto = 10f;
    public float suavidadRotacion = 10f;
    private Rigidbody2D rb2d;
    private bool enSuelo;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Moving();

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
    }

    void Moving()
    {
        rb2d.velocity = new Vector2(velocidadX, rb2d.velocity.y);
    }

    void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, fuerzaSalto);
    }
}
