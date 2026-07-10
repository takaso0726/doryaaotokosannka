using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 3D格闘ゲーム用 試合タイマー
/// 配置想定：画面中央最上部（左右HPバーに挟まれた位置）
///
/// 【Canvas側の配置手順】
/// 1. Canvas (Screen Space - Overlay) 直下に空のRectTransformを作成 → 名前例: "TimerRoot"
/// 2. TimerRoot の Anchor を Top-Center (0.5, 1) に設定
///    - Anchor Min/Max = (0.5, 1)
///    - Pivot = (0.5, 1)
///    - Pos Y = -20 程度（画面上端から少し下げる）
/// 3. TimerRoot の子に TextMeshPro - Text (UI) を配置し、本スクリプトをアタッチ
/// 4. 左HPバーは Anchor (0, 1) 付近、右HPバーは Anchor (1, 1) 付近に置くと
///    自然に「左右のHPバーに挟まれた中央上部」レイアウトになる
/// </summary>
public class MatchTimer : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("時間を表示するTextMeshProUGUI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("試合設定")]
    [Tooltip("試合時間（秒）。格闘ゲームでは60〜99秒が一般的")]
    [SerializeField] private int matchDuration = 60;

    [Tooltip("trueならカウントダウン、falseならカウントアップ")]
    [SerializeField] private bool countDown = true;

    [Header("警告演出")]
    [Tooltip("残りこの秒数以下で警告色＋点滅を開始")]
    [SerializeField] private int warningThreshold = 10;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float blinkInterval = 0.4f;

    [Header("イベント")]
    public UnityEvent OnTimeUp;          // 時間切れ時（判定処理などにフック）
    public UnityEvent<int> OnSecondTick; // 1秒ごとに呼ばれる（SE再生などにフック）

    private float currentTime;
    private bool isRunning = false;
    private bool isPaused = false;
    private int lastDisplayedSecond = -1;
    private float blinkTimer = 0f;
    private bool blinkState = false;

    public float CurrentTime => currentTime;
    public bool IsRunning => isRunning;

    private void Awake()
    {
        currentTime = countDown ? matchDuration : 0f;
        UpdateDisplay(Mathf.CeilToInt(currentTime));
    }

    /// <summary>試合開始時に呼ぶ（ラウンド開始のタイミングなど）</summary>
    public void StartTimer()
    {
        currentTime = countDown ? matchDuration : 0f;
        isRunning = true;
        isPaused = false;
        lastDisplayedSecond = -1;
        if (timerText != null) timerText.color = normalColor;
    }

    /// <summary>一時停止（ポーズメニューやKO演出中に使用）</summary>
    public void PauseTimer() => isPaused = true;

    /// <summary>再開</summary>
    public void ResumeTimer() => isPaused = false;

    /// <summary>強制停止</summary>
    public void StopTimer() => isRunning = false;

    private void Start()
    {
        StartTimer(); // テスト用。本番はラウンド管理側から呼ぶこと
    }

    private void Update()
    {
        if (!isRunning || isPaused) return;

        currentTime += countDown ? -Time.deltaTime : Time.deltaTime;

        if (countDown && currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateDisplay(0);
            OnTimeUp?.Invoke();
            return;
        }

        int displaySecond = Mathf.CeilToInt(currentTime);
        if (displaySecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = displaySecond;
            UpdateDisplay(displaySecond);
            OnSecondTick?.Invoke(displaySecond);
        }

        HandleWarningBlink(displaySecond);
    }

    private void UpdateDisplay(int seconds)
    {
        if (timerText == null) return;
        // 格闘ゲームらしく2桁表示（例: "07", "60"）
        timerText.text = Mathf.Clamp(seconds, 0, 99).ToString("00");
    }

    private void HandleWarningBlink(int secondsLeft)
    {
        if (timerText == null) return;

        bool inWarning = countDown && secondsLeft <= warningThreshold && secondsLeft > 0;

        if (!inWarning)
        {
            if (timerText.color != normalColor)
                timerText.color = normalColor;
            blinkTimer = 0f;
            return;
        }

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            blinkState = !blinkState;
            timerText.color = blinkState ? warningColor : normalColor;
        }
    }
}
