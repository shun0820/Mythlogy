using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager loadSceneManager;
    //シーン移動に関するデータファイル
    [SerializeField]
    private SceneMovementData sceneMovementData = null;
    //フェード(トランジション)プレハブ
    [SerializeField]
    private GameObject fadePrefab = null;
    //フェードインスタンス
    private GameObject fadeInstance;
    //フェードの画像
    private Image fadeImage;
    //フェードのスピード
    [SerializeField]
    private float fadeSpeed = 0f;
    //シーン遷移中かどうか
    private bool isTransition;

    private void Awake()
    {
        //LoadSceneManagerは常に1つだけにする
        if (loadSceneManager == null)
        {
            loadSceneManager = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public void GoToNextScene(SceneMovementData.SceneType scene)
    {
        isTransition = true;
        sceneMovementData.SetSceneType(scene);
        StartCoroutine(FadeAndLoadScene(scene));
    }

    //フェードをした後にシーン読み込み
    IEnumerator FadeAndLoadScene(SceneMovementData.SceneType scene)
    {
        //フェードのインスタンス化
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        //フェードアウト処理
        yield return StartCoroutine(Fade(1f));

        //シーンの読み込み
        if (scene == SceneMovementData.SceneType.FirstVillage){
            yield return StartCoroutine(LoadScene("Village"));
        }else if(scene == SceneMovementData.SceneType.FirstVillageToWorldMap){
            yield return StartCoroutine(LoadScene("WorldMap"));
        }

        //フェードUIのインスタンス化
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        //フェードイン処理
        yield return StartCoroutine(Fade(0f));

        Destroy(fadeInstance);
    }

    //フェード処理
    IEnumerator Fade(float alpha)
    {
        var fadeImageAlpha = fadeImage.color.a;

        while (Mathf.Abs(fadeImageAlpha - alpha) > 0.01f)
        {
            fadeImageAlpha = Mathf.Lerp(fadeImageAlpha, alpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = new Color(0f, 0f, 0f, fadeImageAlpha);
            yield return null;
        }
    }

    //実際にシーンを読み込む処理
    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            yield return null;
        }
    }

    public bool IsTransition()
    {
        return isTransition;
    }
}
