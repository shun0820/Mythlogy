using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoToOtherScene : MonoBehaviour
{
    private LoadSceneManager sceneManager;
    //どのシーンに遷移するか
    [SerializeField]
    private SceneMovementData.SceneType scene = SceneMovementData.SceneType.FirstVillage;
    //シーン遷移中かどうか
    private bool isTransitionToOtherScene;

    private void Awake()
    {
        sceneManager = FindObjectOfType<LoadSceneManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        //次のシーンに遷移途中でないとき
        if (collider.tag == "Player" && !isTransitionToOtherScene)
        {
            isTransitionToOtherScene = true;
            sceneManager.GoToNextScene(scene);
        }
    }

    /*
     //フェードをした後、シーンを読み込む
    IEnumerator FadeAndLoadScene(SceneMovementData.SceneType scene)
    {
       
        isTransitionToOtherScene = false;
    }
    */
}
