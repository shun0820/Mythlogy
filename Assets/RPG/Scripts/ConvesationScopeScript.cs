using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvesationScopeScript : MonoBehaviour
{
    // OnTriggerStay�̓g���K�[�I�u�W�F�N�g(����̏ꍇ��SearchArea�I�u�W�F�N�g)�ɐN�����Ă���ԌĂяo�����
    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk)
        {
            //�v���C���[�L����(���j�e�B�����)���߂Â�����A��b����Ƃ��Ď����̃Q�[���I�u�W�F�N�g��Ԃ�(�n��)
            col.GetComponent<UnityChanTalkScript>().SetConversationPartner(transform.parent.gameObject);
        }
    }

    //OnTriggerExit�̓g���K�[�I�u�W�F�N�g���甲���o�����Ƃ��ɌĂяo�����
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk)
        {
            //�v���C���[�L������������������(SearchArea���甲������)��b���肩��O��
            col.GetComponent<UnityChanTalkScript>().ResetConversationPartner(transform.parent.gameObject);
        }
    }
}
