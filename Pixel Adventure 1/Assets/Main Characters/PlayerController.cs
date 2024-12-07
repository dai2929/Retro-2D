using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度
    public float jumpForce = 5f;   //ジャンプ力
    public Rigidbody2D rb;  //Rigidbody2D型の変数
    public bool isGrounded; //接地判定
    private Vector2 movement;   //移動方向

    public Transform groundCheck;   //接地判定用オブジェクト
    public float groundCheckRadius = 0.2f;   //接地判定用の円の半径
    public LayerMask groundLayer;   //接地判定対象のレイヤー

    //アニメーション
    Animator animator;  //アニメーターを使用可能に
    public string idleAnime = "Idle";
    public string runAnime = "Run";
    public string jumpAnime = "Jump";
    public string goalAnime = "Goal";
    public string hitAnime = "Hit";

    string nowAnime = "";
    string oldAnime = "";

    public static string gameState = "playing"; //ゲームの状態


    void Awake()
    {
        // Rigidbody2Dを自動取得
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //アニメ関連
        animator = GetComponent<Animator>();
        nowAnime = idleAnime;
        oldAnime = idleAnime;

        gameState = "playing";  //ゲーム中にする
    }

    void Update()
    {
        if (gameState != "playing")
        {
            return;
        }

        // 入力を取得
        movement.x = Input.GetAxisRaw("Horizontal"); // 左右移動 (A/Dキー、矢印キー)



        //ジャンプ入力を取得
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        //ゴールやゲームオーバー時には処理を停止
        //if(gameState != "playing")
        //    return;

        //接地判定を更新
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //空中での慣性をつける
        //地面の上or速度が0ではない場合は速度を更新
        if (isGrounded || movement.x != 0)
        {
            //移動処理
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }

        //Debug.Log($"Horizontal Input:{movement.x}");    //デバッグ 移動入力
        //Debug.Log($"Is Grounded:{isGrounded}");     //デバッグ 接地判定

        //アニメーション更新
        if (isGrounded)
        {
            //地面の上
            if (movement.x == 0)
            {
                nowAnime = idleAnime;    //停止中のアニメ
            }
            else
            {
                nowAnime = runAnime;    //移動中のアニメ
            }
        }
        else
        {
            //空中
            nowAnime = jumpAnime;   //ジャンプアニメ
        }
        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);    //アニメーション再生
        }

        //左右移動適正なアニメーションにする
        if (movement.x > 0)
        {
            //右移動
            transform.localScale = new Vector2(1, 1);   //絵を右向き
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);   //絵を左向き
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    //接触開始
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal(); //ゴール
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver(); //ゲームオーバー
        }
    }

    //ゴールメソッド
    public void Goal()
    {
        animator.Play(goalAnime);

        gameState = "gameclear";
        GameStop(); //ゲーム停止

        //上に跳ね上げる(その後消えてクリアにしたい)
        rb.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);

        //StartCoroutinを使ってアニメーション終了後の処理を実行 ★GPTより
        StartCoroutine(GoalAnimationEnd());
    }

    //ゴールアニメーション終了後にキャラクターを消すメソッド ★GPTより
    IEnumerator GoalAnimationEnd()
    {
        //現在のアニメーションが終了するのを待つ
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f || animator.GetCurrentAnimatorStateInfo(0).IsName(goalAnime) == false)
        {
            yield return null;  //1フレーム待機
        }

        //キャラクターを消す(非アクティブ化)
        gameObject.SetActive(false);
    }

    //ゲームオーバー
    public void GameOver()
    {
        animator.Play(hitAnime);

        gameState = "gameover";
        GameStop();

        //ゲームオーバー演出
        //プレイヤーの当たり判定を消す
        GetComponent<CapsuleCollider2D>().enabled = false;
        //プレイヤーを左上に跳ね上げる
        rb.AddForce(new Vector2(-3, 5), ForceMode2D.Impulse);

    }

    //ゲーム停止
    void GameStop()
    {
        rb.velocity = new Vector2(0, 0);
    }

    private void OnDrawGizmosSelected()
    {
        //接地判定の確認用Gizmoを描画
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}