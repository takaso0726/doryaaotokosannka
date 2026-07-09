using UnityEngine;
using UnityEngine.SceneManagement;

// ステージ遷移用

public class Scenetransition : MonoBehaviour
{
    // シーンの遷移先
    [SerializeField] private string nextSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // シーンの遷移
    public void ChangeScene()
    {
        // インスペクターで入力されたシーン名が空でなければ遷移する
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("遷移されません");
        }
    }
}
