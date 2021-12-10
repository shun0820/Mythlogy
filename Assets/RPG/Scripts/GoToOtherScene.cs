using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoToOtherScene : MonoBehaviour
{
    private LoadSceneManager sceneManager;
    //�ǂ̃V�[���ɑJ�ڂ��邩
    [SerializeField]
    private SceneMovementData.SceneType scene = SceneMovementData.SceneType.FirstVillage;
    //�V�[���J�ڒ����ǂ���
    private bool isTransitionToOtherScene;

    private void Awake()
    {
        sceneManager = FindObjectOfType<LoadSceneManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //���̃V�[���ɑJ�ړr���łȂ��Ƃ�
        if (collider.tag == "Player" && !isTransitionToOtherScene)
        {
            isTransitionToOtherScene = true;
            sceneManager.GoToNextScene(scene);
        }
    }

    /*
     //�t�F�[�h��������A�V�[����ǂݍ���
    IEnumerator FadeAndLoadScene(SceneMovementData.SceneType scene)
    {
       
        isTransitionToOtherScene = false;
    }
    */
}
