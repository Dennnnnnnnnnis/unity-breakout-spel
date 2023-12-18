using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int health = 1;
    private BoxCollider2D col;
    private SpriteRenderer sr;

    Vector2 offset = new Vector2();
    [HideInInspector] public Vector2 ogPos = new Vector2();
    [HideInInspector] public bool canShake = false;
    float shakeTime;
    int maxHp;

    GameManager gm;

    // Start is called before the first frame update
    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0f)
        {
            offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            shakeTime -= Time.deltaTime;
        }
        else
            offset = new Vector2();

        if(canShake)
            transform.localPosition = ogPos + (offset * 0.1f);
        col.offset = offset * -0.1f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            health--;
            shakeTime = 0.2f;
            gm.combo++;

            if (health <= 0)
            {
                gm.ScreenShake(0.1f, 0.15f);
                Destroy(gameObject);
            }
            else
            {
                gm.ScreenShake(0.025f, 0.1f);
                sr.sprite = gm.blockSprites[gm.blockSprites.Length - Mathf.RoundToInt(((float)health / (float)maxHp) * gm.blockSprites.Length)];
            }
        }
    }

    public void InitBlock()
    {
        ogPos = transform.localPosition;
        maxHp = health;

        sr.color = gm.blockColors[maxHp - 1];
    }
}
