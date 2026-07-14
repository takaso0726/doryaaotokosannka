using UnityEngine;
using UnityEngine.InputSystem;

public class Curosr : MonoBehaviour
{
    [SerializeField] private float speed = 500.0f;
    [SerializeField] private Vector2 moveLimit = new Vector2(960.0f, 540.0f);
    [SerializeField] private float hitDistance = 100f; // 当たり判定の広さ

    //現在重立っているボタンのインターフェース
    private ICursorClickable hoveredButton;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    [SerializeField] private RectTransform targetButtonRect;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (rectTransform == null) return;

        MovementCursor();
        //CheckCollision();
        HandleInput();
    }

    //移動処理
    private void MovementCursor()
    {
        Vector2 stickInput = Gamepad.current != null ? Gamepad.current.leftStick.ReadValue() : Vector2.zero;

        if (stickInput.magnitude > 0.1f)
        {
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

    //周辺のボタンを探す
    private void CheckCollision()
    {
        /*
        // 画面内にある「ICursorClickable」を持っているスクリプトをすべて検索
        var clickables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);

        ICursorClickable closestButton = null;
        float closestDistance = hitDistance;

        foreach (var mono in clickables)
        {
            // インターフェース（目印）を持っているかチェック
            if (mono is ICursorClickable clickable)
            {
                RectTransform targetRect = mono.GetComponent<RectTransform>();
                if (targetRect == null) continue;

                // 距離を計算
                float distance = Vector2.Distance(rectTransform.anchoredPosition, targetRect.anchoredPosition);

                // 設定したhitDistanceより近く、かつ一番近いものを残す
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestButton = clickable;
                }
            }
        }

        // 重なる対象が変わった時の入れ替え処理
        if (closestButton != hoveredButton)
        {
            if (hoveredButton != null) hoveredButton.OnCursorExit();
            hoveredButton = closestButton;
            if (hoveredButton != null) hoveredButton.OnCursorEnter();
        }
        */
    }

    //入力判定
    private void HandleInput()
    {
        if (hoveredButton == null) return;

        bool isClicked = false;

        if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame) isClicked = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) isClicked = true;

        if (isClicked)
        {
            hoveredButton.OnCursorClick();
        }
    }
}

//カーソルで押せるボタンが共通で持つインターフェース
public interface ICursorClickable
{
    void OnCursorEnter(); //重なった時
    void OnCursorExit();  //離れた時
    void OnCursorClick(); //押された時
}
