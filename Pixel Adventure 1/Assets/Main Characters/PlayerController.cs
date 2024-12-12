using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度
    public float jumpForce = 5f; // ジャンプ力
    private Rigidbody2D rb; // Rigidbody2D型の変数
    private Vector2 movement; // 移動方向

    public Transform groundCheck; // 接地判定用オブジェクト
    public float groundCheckRadius = 0.2f; // 接地判定用の円の半径
    public LayerMask groundLayer; // 接地判定対象のレイヤー
    public LayerMask platformLayer; // 足場床のレイヤー

    private bool isOnPlatform = false; // 足場床にいるかどうか

    // アニメーション
    private Animator animator;
    public string idleAnime = "Idle";
    public string runAnime = "Run";
    public string jumpAnime = "Jump";
    public string goalAnime = "Goal";
    public string hitAnime = "Hit";

    private string nowAnime = "";
    private string oldAnime = "";

    public static string gameState = "playing"; // ゲームの状態

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        nowAnime = idleAnime;
        oldAnime = idleAnime;

        gameState = "playing"; // ゲーム中
    }

    void Update()
    {
        // 入力を取得
        movement.x = Input.GetAxisRaw("Horizontal");

        // 下キーが押された場合、足場床を通過
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isOnPlatform) // 足場床にいる場合のみ
            {
                Debug.Log("Down key pressed. Attempting to disable platform collision.");
                StartCoroutine(DisablePlatformCollision()); // 通過処理開始
            }
        }

        // ジャンプ
        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded())
            {
                Jump();
            }
        }

        // ゴール判定
        if (gameState == "playing" && Input.GetKeyDown(KeyCode.G)) // 仮にGキーでゴール判定
        {
            Goal();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        UpdateAnimation();
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private bool IsGrounded()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | (1 << LayerMask.NameToLayer("OneWayPlatform")));

        if (groundCollider != null)
        {
            // isOnPlatformはOneWayPlatformレイヤーにいるときだけtrueに
            isOnPlatform = groundCollider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform");
            Debug.Log("Ground detected: " + groundCollider.gameObject.name + " on layer " + LayerMask.LayerToName(groundCollider.gameObject.layer));
        }
        else
        {
            isOnPlatform = false;
        }

        return groundCollider != null;
    }

    private void UpdateAnimation()
    {
        if (IsGrounded())
        {
            nowAnime = (movement.x == 0) ? idleAnime : runAnime;
        }
        else
        {
            nowAnime = jumpAnime;
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }

        if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Goal()
    {
        gameState = "goal";
        animator.Play(goalAnime);
    }

    // 足場床の衝突を無効化
    IEnumerator DisablePlatformCollision()
    {
        // 足場床との衝突を無効化
        Debug.Log("Disabling platform collision...");
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("OneWayPlatform"), true);
        yield return new WaitForSeconds(0.2f); // 0.2秒待つことでキャラクターが下に通過する時間を確保
        // 通過後、衝突を再度有効化
        Debug.Log("Re-enabling platform collision.");
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("OneWayPlatform"), false);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
