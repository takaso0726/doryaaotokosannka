using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// 3D格闘ゲーム用カメラコントローラー
// 出場中の全キャラクターの中心点（重心）を自動算出し、
// カメラがその座標へスムーズに移動する。
// キャラクター同士の距離に応じてズーム（カメラ距離）も自動調整する。

public class CameraController : MonoBehaviour
{
    [Header("追従対象")]
    [Tooltip("現在出場中のキャラクターのTransformリスト。動的に増減してOK")]
    public List<Transform> target = new List<Transform>();

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_currentDistance = (maxZoomDistance + minZoomDistance) * 0.5f;
        //_smoothedLookAt = CalculateCenterPoint();
    }
    /*
    // Update is called once per frame
    void LateUpdate()
    {
        // 出場キャラクターがいない場合は何もしない
        /*CleanupNullTargets();
        if (players.Length == 0)
            return;

        // 全員の中心座標を計算
        Vector3 center = GetCenterPoint();

        // 中心から一番遠いプレイヤーまでの距離
        float maxDistance = GetMaxDistance(center);

        // カメラ位置
        Vector3 targetPos =
            center
            + Vector3.up * height
            - transform.forward * (distance + maxDistance * zoomMultiplier);

        // なめらかに移動
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime);

        // 常に中心を見る
        transform.LookAt(center);
    }

    Vector3 GetCenterPoint()
    {
        Vector3 sum = Vector3.zero;

        foreach(Transform p in players)
        {
            sum += p.position;
        }

        return sum / players.Length;
    }

    float GetMaxDistance(Vector3 center)
    {
        float max = 0f;

        foreach(Transform p in players)
        {
            float d = Vector3.Distance(center, p.position);

            if(d > max)
                max = d;
        }

        return max;
    }

    // 出場中の全キャラクターの中心点（重心）を算出する
    private Vector3 CalculateCenterPoint()
    {

    }

    // 中心点から最も離れているキャラクターまでの距離（広がり具合）を算出する
    // キャラクター同士が離れているほどカメラを引く（ズームアウト）ために使用
    private float CalculateMaxSpread(Vector3 center)
    {

    }

    // リストからnull（撃破・非表示等で消えたキャラクター）を除去する
    private void CleanupNullTargets()
    {
        targets.RemoveAll(t => t == null);
    }
        */
}
