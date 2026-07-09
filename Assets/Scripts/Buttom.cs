using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttom : MonoBehaviour, ICursorClickable
{
    //インスペクターからシーン名を自由に設定できるよう
    [SerializeField] private string targetScene;

    void Start()
    {
        // 必要に応じて初期化処理をここに書きます
    }

    void Update()
    {
        
    }

    //カーソルが重なった時
    public void OnCursorEnter()
    {
        Debug.Log("ボタンにカーソルが重なりました");
    }

    //カーソルが離れた時
    public void OnCursorExit()
    {
        Debug.Log("ボタンにカーソルが重なりました");
    }

    //カーソルが重なった状態で押された時
    public void OnCursorClick()
    {
        
        // 設定されたシーン名が空でなければ遷移する
        if (!string.IsNullOrEmpty(targetScene))
        {
            LoadNextScene(targetScene);
        }
        else
        {
            
        }
    }

    // シーン移動を実行する関数
    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
