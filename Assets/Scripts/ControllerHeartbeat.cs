using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// コントローラーを「ドクン…ドクン…」と心拍のように振動させる。
/// MatchTimer の OnSecondTick(int) に OnTimerTick を紐付けることで、
/// 残り時間が少ないときに毎秒ハートビート振動が鳴るようにする。
///
/// 【必須】Edit > Project Settings > Input System Package (New) を利用可能にすること
///  ・Player Settings > Active Input Handling を
///    "Input System Package (New)" または "Both" に設定
///  ・Package Manager で "Input System" パッケージがインストール済みであること
/// </summary>
public class ControllerHeartbeat : MonoBehaviour
{
    [Header("発動条件")]
    [Tooltip("この秒数以下になったら毎秒ハートビート振動を鳴らす（MatchTimerのWarningThresholdと合わせる）")]
    [SerializeField] private int triggerBelowSeconds = 10;

    [Header("ドクン（1拍目）")]
    [SerializeField, Range(0f, 1f)] private float thump1LowFreq = 0.7f;   // 重み・鈍さ
    [SerializeField, Range(0f, 1f)] private float thump1HighFreq = 0.15f; // 鋭さ
    [SerializeField] private float thump1Duration = 0.09f;

    [Header("間の無音")]
    [SerializeField] private float gapDuration = 0.08f;

    [Header("ン（2拍目・弱め）")]
    [SerializeField, Range(0f, 1f)] private float thump2LowFreq = 0.35f;
    [SerializeField, Range(0f, 1f)] private float thump2HighFreq = 0.05f;
    [SerializeField] private float thump2Duration = 0.07f;

    private Coroutine heartbeatRoutine;

    /// <summary>
    /// MatchTimer の OnSecondTick(int) イベントにこの関数を登録する。
    /// 残り秒数が閾値以下のときだけ振動が鳴る。
    /// </summary>
    public void OnTimerTick(int secondsLeft)
    {
        if (secondsLeft <= triggerBelowSeconds && secondsLeft > 0)
        {
            TriggerHeartbeat();
        }
    }

    /// <summary>単発の「ドクン」を鳴らす。ボタンやイベントから直接呼んでもOK。</summary>
    public void TriggerHeartbeat()
    {
        if (heartbeatRoutine != null) StopCoroutine(heartbeatRoutine);
        heartbeatRoutine = StartCoroutine(HeartbeatPattern());
    }

    /// <summary>強制停止（試合終了時などに呼ぶ）</summary>
    public void StopHeartbeat()
    {
        if (heartbeatRoutine != null) StopCoroutine(heartbeatRoutine);
        SetRumble(0f, 0f);
    }

    private IEnumerator HeartbeatPattern()
    {
        // ドッ
        SetRumble(thump1LowFreq, thump1HighFreq);
        yield return new WaitForSecondsRealtime(thump1Duration);

        SetRumble(0f, 0f);
        yield return new WaitForSecondsRealtime(gapDuration);

        // クン
        SetRumble(thump2LowFreq, thump2HighFreq);
        yield return new WaitForSecondsRealtime(thump2Duration);

        SetRumble(0f, 0f);
    }

    private void SetRumble(float lowFreq, float highFreq)
    {
#if ENABLE_INPUT_SYSTEM
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);
        }
#else
        // Input System パッケージが無効な場合はここで何もしない。
        // Project Settings > Player > Active Input Handling を確認してください。
#endif
    }

    private void OnDisable()
    {
        SetRumble(0f, 0f);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SetRumble(0f, 0f);
    }

    // ==== デバッグ用：Inspectorの歯車メニューから実行できる ====
    [ContextMenu("Debug: Test Heartbeat Once")]
    private void DebugTestHeartbeat()
    {
#if ENABLE_INPUT_SYSTEM
        Debug.Log($"[ControllerHeartbeat] Gamepad.current = {(Gamepad.current != null ? Gamepad.current.displayName : "null")}");
#else
        Debug.LogWarning("[ControllerHeartbeat] ENABLE_INPUT_SYSTEM が無効です。Active Input Handling を確認してください。");
#endif
        TriggerHeartbeat();
    }

    [ContextMenu("Debug: Test Strong Continuous Rumble (1s)")]
    private void DebugTestStrongRumble()
    {
        StartCoroutine(DebugStrongRumbleRoutine());
    }

    private IEnumerator DebugStrongRumbleRoutine()
    {
        SetRumble(1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        SetRumble(0f, 0f);
    }
}
