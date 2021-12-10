using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingPosition : MonoBehaviour
{
    [SerializeField]
    private SceneMovementData sceneMovementData = null;

    // Start is called before the first frame update
    void Start()
    {
        //�V�[���J�ڂ̎�ނɉ����ď����ʒu�̃Q�[���I�u�W�F�N�g�̈ʒu�Ɗp�x�ɐݒ�
        if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.StartGame){
            var initialPosition = GameObject.Find("InitialPosition").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        }else if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.FirstVillage){
            var initialPosition = GameObject.Find("InitialPositionOfFirstVillage").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        }else if (sceneMovementData.GetSceneType() == SceneMovementData.SceneType.FirstVillageToWorldMap){
            var initialPosition = GameObject.Find("InitialPositionFirstVillageToWorldMap").transform;
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        }
    }
}
