using UnityEngine;
using TMPro;
using System.Collections;

// メインのテキストのアニメーション
public class MeinTextAnimation : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    [SerializeField] private float TargetFontSize = 150f; // 最終的なサイズ
    [SerializeField] private float MiniTime = 2.0f;       // 何秒かけて小さくするか

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            // 徐々にサイズを変更するコルーチンを開始
            StartCoroutine(ShrinkTextCoroutine());
        }
        else
        {
            Debug.LogError("コンポーネントが見つかりません");
        }
    }

    // 徐々にサイズを小さくする処理（コルーチン）
    IEnumerator ShrinkTextCoroutine()
    {
        float startFontSize = textMeshPro.fontSize; // 開始時のサイズ
        float elapsed = 0f;                         // 経過時間

        // 指定した時間（duration）が経過するまでループ
        while (elapsed < MiniTime)
        {
            elapsed += Time.deltaTime; //経過時間を足す
            float ratio = elapsed / MiniTime; //進行度（0.0 ～ 1.0）

            // 線形補間を使い、現在のサイズから目標サイズへ小さくさせる
            textMeshPro.fontSize = Mathf.Lerp(startFontSize, TargetFontSize, ratio);

            // 1フレーム待つ
            yield return null;
        }

        // 最後に目標サイズに設定
        textMeshPro.fontSize = TargetFontSize;
    }
}