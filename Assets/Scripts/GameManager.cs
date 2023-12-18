using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    PlayerPaddle player;

    // Ball stuff
    public Rigidbody2D ball;
    public float ballSpeed = 4f;
    private Vector2[] ballEffect = new Vector2[3];
    private Transform[] ballEffectObj = new Transform[3];
    private float ballTimer = 0.1f;

    // Block stuff
    public Sprite[] blockSprites;
    public Color[] blockColors;
    public GameObject block;

    public Transform level;
    private int curLevel = 0;
    private float growTimer = 0.5f;

    // UI
    public TMP_Text scoreTxt, comboTxt;

    private float screenShakeTimer, screenShakeMag;
    public int combo, score;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerPaddle>();
        ball.velocity = ballSpeed * Vector2.down;
        CreateLevel();

        for(int i = 0; i < ballEffectObj.Length; i++)
        {
            GameObject newObj = Instantiate(ball.gameObject);
            Destroy(newObj.GetComponent<Rigidbody2D>());
            Destroy(newObj.GetComponent<CircleCollider2D>());

            newObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f - (0.025f * i));
            newObj.GetComponent<SpriteRenderer>().sortingOrder = -100 - i;

            ballEffectObj[i] = newObj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, 0, -10);
        if (screenShakeTimer != 0)
        {
            transform.position = new Vector3(0 + Random.Range(-screenShakeMag, screenShakeMag), 0 + Random.Range(-screenShakeMag, screenShakeMag), -10);
            screenShakeTimer = Mathf.Max(screenShakeTimer - Time.deltaTime, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Detta är debug grejer men jag lämnar kvar det så att man kan enklare se alla delar av spelet
        {
            foreach (Transform c in level)
            {
                Destroy(c.gameObject);
            }
        }

        if(level.childCount == 0)
        {
            CreateLevel();
        }

        level.localScale = new Vector3(1f - (growTimer * 2), 1f - (growTimer * 2), 1);
        if (growTimer != 0)
        {
            growTimer = Mathf.Max(growTimer - Time.deltaTime, 0);
            foreach (Transform c in level)
            {
                c.GetComponent<BoxCollider2D>().enabled = false;
                c.GetComponent<Block>().canShake = false;
                c.localPosition = c.GetComponent<Block>().ogPos / level.localScale;
            }

            if (growTimer == 0)
            {
                foreach(Transform c in level)
                {
                    c.GetComponent<BoxCollider2D>().enabled = true;
                    c.GetComponent<Block>().canShake = true;
                }
            }
        }

        comboTxt.text = "+ " + (combo * (9 + combo) * 10) + " (x" + combo + ")";
        scoreTxt.text = Mathf.Ceil(Mathf.Lerp(int.Parse(scoreTxt.text), score, Time.deltaTime * 8)).ToString();

        for (int i = 0; i < ballEffectObj.Length; i++)
        {
            ballEffectObj[i].position = ballEffect[i];
        }

        if(ballTimer <= 0)
        {
            ballTimer += 0.1f;
            ballEffect[2] = ballEffect[1];
            ballEffect[1] = ballEffect[0];
            ballEffect[0] = ball.position;
        }
        ballTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (ball.velocity.normalized.y == 0)
            ball.velocity = Vector2.down;
        ball.velocity = ball.velocity.normalized * ballSpeed;
    }

    private void CreateLevel()
    {
        curLevel++;
        growTimer = 0.5f;
        level.localScale = new Vector3(1, 1, 1);

        for(int e = 0; e < 5; e++)
        {
            if (curLevel <= e)
                break;

            for (int i = 0; i < 5; i++)
            {
                GameObject blockObj = Instantiate(block);
                blockObj.transform.parent = level;
                blockObj.transform.localPosition = new Vector2(-10 + (5 * i), 7f - (e * 1.5f));

                if (curLevel <= 1 + e)
                    blockObj.GetComponent<Block>().health = 1;
                else if (curLevel <= 3 + e)
                    blockObj.GetComponent<Block>().health = 2;
                else if (curLevel <= 5 + e)
                    blockObj.GetComponent<Block>().health = 3;
                else
                    blockObj.GetComponent<Block>().health = 4;

                blockObj.GetComponent<Block>().InitBlock();
            }
        }

        foreach (Transform c in level)
        {
            c.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (curLevel != 1)
        {
            ballSpeed += 0.2f;
            player.speed += 0.1f;
        }
    }

    public void ScreenShake(float magnitude, float time)
    {
        screenShakeMag = magnitude;
        screenShakeTimer = time;
    }

    public void CashCombo()
    {
        score += combo * (9 + combo) * 10;
        combo = 0;
        print(score);
    }
}
