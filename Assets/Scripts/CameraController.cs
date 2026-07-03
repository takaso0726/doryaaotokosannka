using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D格闘ゲーム用カメラコントローラー
/// 出場中の全キャラクターの中心点（重心）を自動算出し、
/// カメラがその座標へスムーズに移動する。
/// キャラクター同士の距離に応じてズーム（カメラ距離）も自動調整する。
/// </summary>
public class FightingCameraController : MonoBehaviour
{
    [Header("追従対象")]
    [Tooltip("現在出場中のキャラクターのTransformリスト。動的に増減してOK")]
    public List<Transform> targets = new List<Transform>();

    [Header("カメラ追従設定")]
    [Tooltip("中心点への移動速度（大きいほど速く追従）")]
    public float followSmoothTime = 0.25f;

    [Tooltip("中心点から見たカメラの基本オフセット（ローカル方向）")]
    public Vector3 baseOffset = new Vector3(0f, 3f, -8f);

    [Header("ズーム（距離）調整設定")]
    [Tooltip("キャラクター間の最大距離がこの値のときの最小カメラ距離倍率")]
    public float minZoomDistance = 6f;

    [Tooltip("キャラクター間の最大距離がこの値のときの最大カメラ距離倍率")]
    public float maxZoomDistance = 16f;

    [Tooltip("キャラクター間の距離とカメラ距離の対応を調整する係数")]
    public float spreadToDistanceMultiplier = 1.5f;

    [Tooltip("ズーム変化のスムーズさ")]
    public float zoomSmoothTime = 0.3f;

    [Header("注視点設定")]
    [Tooltip("キャラクターの足元ではなく少し上を見るためのYオフセット")]
    public float lookAtHeightOffset = 1.0f;

    [Tooltip("注視点の回転スムーズさ")]
    public float rotationSmoothTime = 0.2f;

    // 内部状態
    private Vector3 _velocityPos;   // SmoothDamp用
    private float _velocityZoom;    // SmoothDamp用（float）
    private float _currentDistance; // 現在のカメラ距離
    private Vector3 _currentLookAtVelocity;
    private Vector3 _smoothedLookAt;

    void Start()
    {
        _currentDistance = (maxZoomDistance + minZoomDistance) * 0.5f;
        _smoothedLookAt = CalculateCenterPoint();
    }

    void LateUpdate()
    {
        // 出場キャラクターがいない場合は何もしない
        CleanupNullTargets();
        if (targets.Count == 0) return;

        // 1. 中心点（重心）を算出
        Vector3 centerPoint = CalculateCenterPoint();

        // 2. キャラクター間の広がり（最大距離）を算出してズーム量を決定
        float spread = CalculateMaxSpread(centerPoint);
        float targetDistance = Mathf.Clamp(
            spread * spreadToDistanceMultiplier,
            minZoomDistance,
            maxZoomDistance
        );

        _currentDistance = Mathf.SmoothDamp(
            _currentDistance,
            targetDistance,
            ref _velocityZoom,
            zoomSmoothTime
        );

        // 3. 注視点をスムーズに更新
        Vector3 lookAtTarget = centerPoint + Vector3.up * lookAtHeightOffset;
        _smoothedLookAt = Vector3.SmoothDamp(
            _smoothedLookAt,
            lookAtTarget,
            ref _currentLookAtVelocity,
            rotationSmoothTime
        );

        // 4. オフセット方向を距離に応じてスケーリングしてカメラ目標位置を算出
        Vector3 offsetDirection = baseOffset.normalized;
        Vector3 desiredCameraPos = _smoothedLookAt + offsetDirection * _currentDistance;

        // 5. カメラ位置をスムーズに移動
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredCameraPos,
            ref _velocityPos,
            followSmoothTime
        );

        // 6. 常に中心点を見るように回転
        transform.LookAt(_smoothedLookAt);
    }

    /// <summary>
    /// 出場中の全キャラクターの中心点（重心）を算出する
    /// </summary>
    private Vector3 CalculateCenterPoint()
    {
        if (targets.Count == 0) return transform.position;

        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (var t in targets)
        {
            if (t == null) continue;
            sum += t.position;
            count++;
        }

        return count > 0 ? sum / count : transform.position;
    }

    /// <summary>
    /// 中心点から最も離れているキャラクターまでの距離（広がり具合）を算出する
    /// キャラクター同士が離れているほどカメラを引く（ズームアウト）ために使用
    /// </summary>
    private float CalculateMaxSpread(Vector3 center)
    {
        float maxDist = 0f;
        foreach (var t in targets)
        {
            if (t == null) continue;
            float dist = Vector3.Distance(center, t.position);
            if (dist > maxDist) maxDist = dist;
        }
        return maxDist;
    }

    /// <summary>
    /// リストからnull（撃破・非表示等で消えたキャラクター）を除去する
    /// </summary>
    private void CleanupNullTargets()
    {
        targets.RemoveAll(t => t == null);
    }

    /// <summary>
    /// キャラクター出現時に呼び出して追従対象に追加する
    /// </summary>
    public void RegisterTarget(Transform t)
    {
        if (t != null && !targets.Contains(t))
        {
            targets.Add(t);
        }
    }

    /// <summary>
    /// キャラクター退場（撃破・リタイア等）時に呼び出して追従対象から除外する
    /// </summary>
    public void UnregisterTarget(Transform t)
    {
        if (targets.Contains(t))
        {
            targets.Remove(t);
        }
    }

    // デバッグ用：シーンビューに中心点と広がり範囲を可視化
    private void OnDrawGizmosSelected()
    {
        if (targets == null || targets.Count == 0) return;

        Vector3 center = CalculateCenterPoint();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.3f);

        Gizmos.color = Color.yellow;
        foreach (var t in targets)
        {
            if (t == null) continue;
            Gizmos.DrawLine(center, t.position);
        }
    }
}
