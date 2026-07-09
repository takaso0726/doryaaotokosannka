using UnityEngine;
using UnityEngine.InputSystem;

//タイトルのカーソル

public class TitleCurosr : MonoBehaviour
{
    // スティックでの移動速度
    [SerializeField] private float speed = 500.0f;

    // 移動範囲の制限（画面外に行かないように調整するための値）
    [SerializeField] private Vector2 moveLimit = new Vector2(960.0f, 540.0f);

    // ターゲットとなるテキストのRectTransform
    [SerializeField] private RectTransform targetTextRect;
    [SerializeField] private float hitDistance = 100f; // 当たり判定の広さ

    // タイトルが重なった時
    private TitileText hoveredText;
    private RectTransform rectTransform;

    private Canvas parentCanvas; // Cunvasを取得

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Canvas内での位置制御のトランスフォームを取得
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }

        parentCanvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform == null) return;

        //カーソルを動かす関数
        MovementCurrosr();

        //重なりを確認する関数
        CheckCollision();

        //クリックを判定する関数
        HandleInput();
    }

    //マウスの移動処理
    private void MovementCurrosr()
    {
        Vector2 stickInput = Gamepad.current != null ? Gamepad.current.rightStick.ReadValue() : Vector2.zero;

        if (stickInput.magnitude > 0.1f)
        {
            // ゲームパッドでの移動処理
            Vector2 currentPos = rectTransform.anchoredPosition;
            currentPos.x += stickInput.x * speed * Time.deltaTime;
            currentPos.y += stickInput.y * speed * Time.deltaTime;
            currentPos.x = Mathf.Clamp(currentPos.x, -moveLimit.x, moveLimit.x);
            currentPos.y = Mathf.Clamp(currentPos.y, -moveLimit.y, moveLimit.y);
            rectTransform.anchoredPosition = currentPos;
        }
        else if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            if (mouseDelta.magnitude > 0.01f && parentCanvas != null)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                //画面のマウス位置を、UI（Canvas）の座標に変換する
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    mousePosition,
                    parentCanvas.worldCamera,
                    out Vector2 localPoint
                );
                rectTransform.anchoredPosition = localPoint;
            }
        }
    }

    //クリック処理
    private void CheckCollision()
    {
        if (targetTextRect != null)
        {
            float distance = Vector2.Distance(rectTransform.anchoredPosition, targetTextRect.anchoredPosition);

            //重なっているか確認
            if (distance < hitDistance)
            {
                if (hoveredText == null)
                {
                    hoveredText = targetTextRect.GetComponent<TitileText>();
                    if (hoveredText != null) Debug.Log("テキストに重なりました");
                }
            }
            else
            {
                if (hoveredText != null)
                {
                    Debug.Log("テキストから離れました");
                    hoveredText = null;
                }
            }
        }
    }

    //入力処理
    private void HandleInput()
    {
        if (hoveredText == null) return; //重なっていなければ何もしない

        bool isClicked = false;

        //ゲームパッドのAボタン
        if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
        {
            Debug.Log("Aボタンが押されました");
            isClicked = true;
        }

        //マウスの右クリック
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("右クリックが押されました");
            isClicked = true;
        }

        if (isClicked)
        {
            hoveredText.OnCursorClick(); //カウントを進める
        }
    }
}