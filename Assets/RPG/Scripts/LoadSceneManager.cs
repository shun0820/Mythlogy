using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager loadSceneManager;
    //�V�[���ړ��Ɋւ���f�[�^�t�@�C��
    [SerializeField]
    private SceneMovementData sceneMovementData = null;
    //�t�F�[�h(�g�����W�V����)�v���n�u
    [SerializeField]
    private GameObject fadePrefab = null;
    //�t�F�[�h�C���X�^���X
    private GameObject fadeInstance;
    //�t�F�[�h�̉摜
    private Image fadeImage;
    //�t�F�[�h�̃X�s�[�h
    [SerializeField]
    private float fadeSpeed = 0f;
    //�V�[���J�ڒ����ǂ���
    private bool isTransition;

    private void Awake()
    {
        //LoadSceneManager�͏��1�����ɂ���
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

    //�t�F�[�h��������ɃV�[���ǂݍ���
    IEnumerator FadeAndLoadScene(SceneMovementData.SceneType scene)
    {
        //�t�F�[�h�̃C���X�^���X��
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        //�t�F�[�h�A�E�g����
        yield return StartCoroutine(Fade(1f));

        //�V�[���̓ǂݍ���
        if (scene == SceneMovementData.SceneType.FirstVillage){
            yield return StartCoroutine(LoadScene("Village"));
        }else if(scene == SceneMovementData.SceneType.FirstVillageToWorldMap){
            yield return StartCoroutine(LoadScene("WorldMap"));
        }

        //�t�F�[�hUI�̃C���X�^���X��
        fadeInstance = Instantiate<GameObject>(fadePrefab);
        fadeImage = fadeInstance.GetComponentInChildren<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        //�t�F�[�h�C������
        yield return StartCoroutine(Fade(0f));

        Destroy(fadeInstance);
    }

    //�t�F�[�h����
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

    //���ۂɃV�[����ǂݍ��ޏ���
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
