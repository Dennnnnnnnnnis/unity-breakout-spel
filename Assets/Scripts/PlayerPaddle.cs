using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class PlayerPaddle : MonoBehaviour
{
    private GameManager gm;
    private Rigidbody2D rb;
    public float speed = 5f;

    // Input
    private float moveX = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * speed * 100f * Time.fixedDeltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            gm.CashCombo();

            float off = Mathf.Clamp((collision.gameObject.transform.position.x - transform.position.x) * 0.5f, -1f, 1f);
            Rigidbody2D rbBall = collision.gameObject.GetComponent<Rigidbody2D>();

            rbBall.velocity = new Vector2(rbBall.velocity.x + (off * 2f) + (rb.velocity.x * 0.5f), rbBall.velocity.y);
            print(off);
        }
    }
}
