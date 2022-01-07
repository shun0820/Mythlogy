using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandScript : MonoBehaviour
{
    public enum CommandMode
    {
        CommandPanel,
        StatusPanelSelectCharacter,
        StatusPanel,
        ItemPanelSelectCharacter,
        ItemPanel,
        UseItemPanel,
        UseItemSelectCharacterPanel,
        UseItemPanelToItemPanel,
        UseItemPanelToUseItemPanel,
        UseItemSelectCharacterPanelToUseItemPanel,
        NoItemPassed
    }

    private CommandMode currentCommand;
    //�v���C���[�L�����R�}���h�X�N���v�g
    private UnityChanCommandScript unityChanCommandScript;

    //�ŏ��ɑI������Button��Transform
    private GameObject firstSelectButton;

    //�R�}���h�p�l��
    private GameObject commandPanel;
    //�X�e�[�^�X�\���p�l��
    private GameObject statusPanel;
    //�L�����N�^�[�I���p�l��
    private GameObject selectCharacterPanel;

    //�R�}���h�p�l����CanvasGroup
    private CanvasGroup commandPanelCanvasGroup;
    //�L�����I���p�l����CanvasGroup
    private CanvasGroup selectCharacterPanelCanvasGroup;

    //�L������
    private Text characterNameText;
    //�X�e�[�^�X�^�C�g���e�L�X�g
    private Text statusTitleText;
    //�X�e�[�^�X�p�����[�^�e�L�X�g�P
    private Text statusParamater1Text;
    //�X�e�[�^�X�p�����[�^�e�L�X�g�Q
    private Text statusParamater2Text;
    //�p�[�e�B�X�e�[�^�X(���)
    [SerializeField]
    private PartyStatus partyStatus = null;

    //�L�����I���{�^���̃v���n�u
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //�Ō�ɑI�����Ă����Q�[���I�u�W�F�N�g���X�^�b�N
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    //�A�C�e���\���p�l��
    private GameObject itemPanel;
    //�A�C�e���p�l���{�^����\������ꏊ
    private GameObject content;
    //�A�C�e�����g���I���p�l��
    private GameObject useItemPanel;
    //�A�C�e�����g�����������ɂ��邩��I�ԃp�l��
    private GameObject useItemSelectCharacterPanel;
    //�A�C�e������\������p�l��
    private GameObject itemInformationPanel;
    //�A�C�e���g�p��̏��(�g�p���b�Z�[�W�Ȃ�)��\������p�l��
    private GameObject useItemInformationPanel;

    //�A�C�e���p�l���̃L�����o�X�O���[�v
    private CanvasGroup itemPanelCanvasGroup;
    //�A�C�e�����g���I���p�l���̃L�����o�X�O���[�v
    private CanvasGroup useItemPanelCanvasGroup;
    //�A�C�e�����g���L�����N�^�[��I�����邽�߂̃L�����o�X�O���[�v
    private CanvasGroup useItemSelectCharacterPanelCanvasGroup;

    //���\���^�C�g���e�L�X�g
    private Text informationTitleText;
    //���\���e�L�X�g
    private Text informationText;

    //�L�����N�^�[�A�C�e���̃{�^���̃v���n�u
    [SerializeField]
    private GameObject itemPanelButtonPrefab = null;
    //�A�C�e���g�p���̃{�^���̃v���n�u
    [SerializeField]
    private GameObject useItemPanelButtonPrefab = null;

    //�A�C�e���{�^���ꗗ
    private List<GameObject> itemPanelButtonList = new List<GameObject>();


    void Awake()
    {
        //�R�}���h��ʂ��J�����������Ă���UnityChanCommandScript���擾
        unityChanCommandScript = GameObject.FindWithTag("Player").GetComponent<UnityChanCommandScript>();
        //���݂̃R�}���h��������
        currentCommand = CommandMode.CommandPanel;
        //�K�w��H���Ď擾
        firstSelectButton = transform.Find("CommandPanel/StatusButton").gameObject;
        //�p�l���n
        commandPanel = transform.Find("CommandPanel").gameObject;
        statusPanel = transform.Find("StatusPanel").gameObject;
        selectCharacterPanel = transform.Find("SelectCharacterPanel").gameObject;
        useItemSelectCharacterPanel = transform.Find("UseItemSelectCharacterPanel").gameObject;

        //CanvasGroup
        commandPanelCanvasGroup = commandPanel.GetComponent<CanvasGroup>();
        selectCharacterPanelCanvasGroup = selectCharacterPanel.GetComponent<CanvasGroup>();

        //�X�e�[�^�X�p�e�L�X�g
        characterNameText = statusPanel.transform.Find("CharacterNamePanel/Text").GetComponent<Text>();
        statusTitleText = statusPanel.transform.Find("StatusParamaterPanel/Title").GetComponent<Text>();
        statusParamater1Text = statusPanel.transform.Find("StatusParamaterPanel/Paramater1").GetComponent<Text>();
        statusParamater2Text = statusPanel.transform.Find("StatusParamaterPanel/Paramater2").GetComponent<Text>();

        itemPanel = transform.Find("ItemPanel").gameObject;
        content = itemPanel.transform.Find("Mask/Content").gameObject;
        useItemPanel = transform.Find("UseItemPanel").gameObject;
        itemInformationPanel = transform.Find("ItemInformationPanel").gameObject;
        useItemInformationPanel = transform.Find("UseItemInformationPanel").gameObject;

        itemPanelCanvasGroup = itemPanel.GetComponent<CanvasGroup>();
        useItemPanelCanvasGroup = useItemPanel.GetComponent<CanvasGroup>();
        useItemSelectCharacterPanelCanvasGroup = useItemSelectCharacterPanel.GetComponent<CanvasGroup>();

        //���\���p�e�L�X�g
        informationTitleText = itemInformationPanel.transform.Find("Title").GetComponent<Text>();
        informationText = itemInformationPanel.transform.Find("Information").GetComponent<Text>();
    }

    private void OnEnable()
    {
        currentCommand = CommandMode.CommandPanel;

        statusPanel.SetActive(false);
        selectCharacterPanel.SetActive(false);

        for(int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }

        selectedGameObjectStack.Clear();

        commandPanelCanvasGroup.interactable = true;
        selectCharacterPanelCanvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(firstSelectButton);

        itemPanel.SetActive(false);
        useItemPanel.SetActive(false);
        useItemSelectCharacterPanel.SetActive(false);
        itemInformationPanel.SetActive(false);
        useItemInformationPanel.SetActive(false);

        //�A�C�e���p�l���{�^��������΂��ׂč폜
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        //�A�C�e�����g���L�����N�^�[�I���{�^��������΂��ׂč폜
        for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }

        //�A�C�e�����g������̃L�����N�^�[�I���{�^��������΂��ׂč폜
        for(int i= useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        itemPanelButtonList.Clear();

        itemPanelCanvasGroup.interactable = false;
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (currentCommand == CommandMode.CommandPanel)
            {
                unityChanCommandScript.ExitCommand();
                gameObject.SetActive(false);
            }else if (currentCommand == CommandMode.StatusPanelSelectCharacter || currentCommand == CommandMode.StatusPanel)
            {
                selectCharacterPanelCanvasGroup.interactable = false;
                selectCharacterPanel.SetActive(false);
                statusPanel.SetActive(false);

                for(int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                commandPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.CommandPanel;
            }else if (currentCommand == CommandMode.ItemPanelSelectCharacter)
            {
                selectCharacterPanelCanvasGroup.interactable = false;
                selectCharacterPanel.SetActive(false);
                itemInformationPanel.SetActive(false);

                for(int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                commandPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.CommandPanel;

            }else if (currentCommand == CommandMode.ItemPanel)
            {
                itemPanelCanvasGroup.interactable = false;
                itemPanel.SetActive(false);
                itemInformationPanel.SetActive(false);
                //���X�g���N���A
                itemPanelButtonList.Clear();
                //ItemPanel��Cancel����������content�ȉ��̃A�C�e���p�l���{�^�������ׂč폜
                for(int i = content.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(content.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                selectCharacterPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.ItemPanelSelectCharacter;

                //�A�C�e����I�����A�ǂ��g������I�����Ă���Ƃ�
            }else if (currentCommand == CommandMode.UseItemPanel)
            {
                useItemPanelCanvasGroup.interactable = false;
                useItemPanel.SetActive(false);
                // UseItemPanel��Cancel�{�^������������UseItemPanel�̎q�v�f�̃{�^�������ׂč폜
                for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(useItemPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                itemPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.ItemPanel;

                //�A�C�e���g�p���̎g�p�����I�����Ă���Ƃ�
            }else if (currentCommand == CommandMode.UseItemSelectCharacterPanel)
            {
                useItemSelectCharacterPanelCanvasGroup.interactable = false;
                useItemSelectCharacterPanel.SetActive(false);
                //UseItemSelectCharacterPanel��Cancel�{�^���𐄂�����A�A�C�e�����g�p����L�����{�^����S�č폜
                for(int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                currentCommand = CommandMode.UseItemPanel;
            }

            if (currentCommand == CommandMode.UseItemPanelToItemPanel)
            {
                if (Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.ItemPanel;
                    useItemInformationPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;

                    EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                }
                //�A�C�e�����g�p���鑊��̃L�����I������A�C�e�����ǂ����邩�Ɉڍs����Ƃ�
            }else if (currentCommand == CommandMode.UseItemSelectCharacterPanelToUseItemPanel)
            {
                if(Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.UseItemPanel;
                    useItemInformationPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;

                    EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                }
                //�A�C�e�����̂Ă��I��������̏��
            }else if (currentCommand == CommandMode.UseItemPanelToItemPanel)
            {
                if (Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.UseItemPanel;
                    useItemInformationPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;
                }
                //�A�C�e�����g�p�A�n���A�̂Ă��I��������ɂ��̃A�C�e���̐���0�ɂȂ�����
            }else if (currentCommand == CommandMode.NoItemPassed)
            {
                if (Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.ItemPanel;
                    useItemInformationPanel.SetActive(false);
                    useItemPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;

                    //�A�C�e���p�l���{�^��������΍ŏ��̃A�C�e���p�l���{�^����I��
                    if (content.transform.childCount != 0)
                    {
                        EventSystem.current.SetSelectedGameObject(content.transform.GetChild(0).gameObject);
                    } else
                    {
                        //�A�C�e���p�l���{�^�����Ȃ����(���A�C�e���������Ă��Ȃ��̂ł����)ItemSelectPanel�ɖ߂�
                        currentCommand = CommandMode.ItemPanelSelectCharacter;
                        itemPanelCanvasGroup.interactable = false;
                        itemPanel.SetActive(false);
                        selectCharacterPanelCanvasGroup.interactable = true;
                        selectCharacterPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                    }
                }
            }
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (currentCommand == CommandMode.CommandPanel)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.ItemPanel)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }else if (currentCommand == CommandMode.ItemPanelSelectCharacter)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }
            else if (currentCommand == CommandMode.ItemPanelSelectCharacter)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }else if (currentCommand == CommandMode.UseItemPanel)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }else if (currentCommand == CommandMode.StatusPanel)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }else if (currentCommand == CommandMode.StatusPanelSelectCharacter)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }else if (currentCommand == CommandMode.UseItemSelectCharacterPanel)
            {
                EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(0).gameObject);
            }
        }

    }

    //�I�������R�}���h�ŏ�������
    public void SelectCommand(string command)
    {
        if (command == "Status")
        {
            currentCommand = CommandMode.StatusPanelSelectCharacter;
            //UI�̃I���E�I�t��I���A�C�R���̐ݒ�
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            //�p�[�e�B�[�����o�[���̃{�^�����쐬
            foreach(var member in partyStatus.GetAllyStatus())
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, selectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => ShowStatus(member));
            }
        } else if (command == "Item")
        {
            currentCommand = CommandMode.ItemPanelSelectCharacter;
            statusPanel.SetActive(false);
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            foreach(var member in partyStatus.GetAllyStatus())
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, selectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponentInChildren<Button>().onClick.AddListener(() => CreateItemPanelButton(member));
            }
        }

        //�K�w����ԍŌ�ɕ��בւ�
        selectCharacterPanel.transform.SetAsLastSibling();
        selectCharacterPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.transform.GetChild(0).gameObject);
    }

    //�L�����̃X�e�[�^�X��\��
    public void ShowStatus(AllyStatus allyStatus)
    {
        currentCommand = CommandMode.StatusPanel;
        statusPanel.SetActive(true);
        //�L��������\��
        characterNameText.text = allyStatus.GetCharacterName();

        //�^�C�g���̕\��
        var text = "���x��\n";
        text += "HP\n";
        text += "SP\n";
        text += "�o���l\n";
        text += "��Ԉُ�\n";
        text += "STR\n";
        text += "AGI\n";
        text += "VIT\n";
        text += "DEX\n";
        text += "INT\n";
        text += "LUK\n";
        text += "��������\n";
        text += "�����Z\n";
        text += "�U����\n";
        text += "�h���\n";
        statusTitleText.text = text;

        //HP��SP��Division�L���̕\��
        text = "\n";
        text += allyStatus.GetHP() + "\n";
        text += allyStatus.GetSP() + "\n";
        statusParamater1Text.text = text;

        //�X�e�[�^�X�p�����[�^�̕\��
        text = allyStatus.GetLevel() + "\n";
        text += allyStatus.GetMaxHP() + "\n";
        text += allyStatus.GetMaxSP() + "\n";
        text += allyStatus.get_earned_EXP() + "\n";
        if(!allyStatus.IsPoisonState() && !allyStatus.IsNumbnessState())
        {
            text += "����\n";
        }
        else
        {
            if (allyStatus.IsPoisonState())
            {
                text += "��";
                if (allyStatus.IsNumbnessState())
                {
                    text += "�V�r��\n";
                }
            } else
            {
                if (allyStatus.IsNumbnessState())
                {
                    text += "�V�r��\n";
                }
            }
        }
        text += allyStatus.GetSTR() + "\n";
        text += allyStatus.GetAGI() + "\n";
        text += allyStatus.GetVIT() + "\n";
        text += allyStatus.GetDEX() + "\n";
        text += allyStatus.GetINT() + "\n";
        text += allyStatus.GetLUK() + "\n";
        text += allyStatus?.get_equiped_weapon()?.GetItemName() ?? "";
        text += "\n";
        text += allyStatus.get_equiped_armor()?.GetItemName() ?? "";
        text += "\n";
        text += allyStatus.GetSTR() + (allyStatus.get_equiped_weapon()?.GetAmount() ?? 0) + "\n";
        text += allyStatus.GetVIT() + (allyStatus.get_equiped_armor()?.GetAmount() ?? 0) + "\n";
        statusParamater2Text.text = text;

        statusPanel.transform.SetAsLastSibling();
    }

    //�L�����N�^�[�������Ă���A�C�e���̃{�^���\��
    public void CreateItemPanelButton(AllyStatus allyStatus)
    {
        itemInformationPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = false;

        //�A�C�e���p�l���{�^�������쐻������
        int itemPanelButtonNum = 0;
        GameObject itemButtonIns;
        //�I�������L�����N�^�[�̃A�C�e�������A�A�C�e���p�l���{�^�����쐬
        //�����Ă���A�C�e�����̃{�^���̍쐻�ƁA�N���b�N���̎��s���\�b�h�̐ݒ�
        foreach(var item in allyStatus.get_item_dictionary().Keys)
        {
            itemButtonIns = Instantiate<GameObject>(itemPanelButtonPrefab, content.transform);
            itemButtonIns.transform.Find("ItemName").GetComponent<Text>().text = item.GetItemName();
            itemButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItem(allyStatus, item));
            itemButtonIns.GetComponent<ItemPanelButtonScript>().SetParam(item);

            //�A�C�e�����\��
            itemButtonIns.transform.Find("Num").GetComponent<Text>().text = allyStatus.get_item_number(item).ToString();

            //�������Ă��镐���h��ɂ́A���O�̉���E��\�����A����Text��ێ����ď���
            if (allyStatus.get_equiped_weapon() == item)
            {
                itemButtonIns.transform.Find("Equip").GetComponent<Text>().text = "E";
            } else if (allyStatus.get_equiped_armor() == item)
            {
                itemButtonIns.transform.Find("Equip").GetComponent<Text>().text = "E";
            }

            //�A�C�e���{�^�����X�g�ɒǉ�
            itemPanelButtonList.Add(itemButtonIns);
            //�A�C�e���p�l���{�^���ԍ����X�V(1���₷)
            itemPanelButtonNum++;
        }

        if (content.transform.childCount != 0)
        {
            //SelectCharacterPanel�ōŌ�ɂǂ̃Q�[���I�u�W�F�N�g��I�����Ă�����
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);
            currentCommand = CommandMode.ItemPanel;
            itemPanel.SetActive(true);
            itemPanel.transform.SetAsLastSibling();
            itemPanelCanvasGroup.interactable = true;
            EventSystem.current.SetSelectedGameObject(content.transform.GetChild(0).gameObject);
        }
        else
        {
            informationTitleText.text = "";
            informationText.text = "�A�C�e���������Ă��܂���";
            selectCharacterPanelCanvasGroup.interactable = true;
        }
    }

    public void SelectItem(AllyStatus allyStatus,Item item)
    {
        //�A�C�e���̎�ނɉ����Ăł��鍀�ڂ�ύX����
        if(item.GetItemType()==Item.Type.ArmorAll || item.GetItemType()==Item.Type.ArmorPlayer || item.GetItemType() == Item.Type.WeaponAll || item.GetItemType() == Item.Type.WeaponPlayer)
        {
            var itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            if (item == allyStatus.get_equiped_weapon() || item == allyStatus.get_equiped_armor())
            {
                itemMenuButtonIns.GetComponentInChildren<Text>().text = "�������O��";
                itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => RemoveEquip(allyStatus, item));
            }
            else
            {
                itemMenuButtonIns.GetComponentInChildren<Text>().text = "��������";
                itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => Equip(allyStatus, item));
            }

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "�n��";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItem(allyStatus, item));
            
            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "�̂Ă�";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => ThrowAwayItem(allyStatus, item));

        }else if (item.GetItemType() == Item.Type.HPRecovery || item.GetItemType() == Item.Type.SPRecovery)
        {
            var itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "�g��";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(allyStatus, item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "�n��";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItem(allyStatus, item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "�̂Ă�";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => ThrowAwayItem(allyStatus, item));

        } else if (item.GetItemType() == Item.Type.Valuables)
        {
            informationTitleText.text = item.GetItemName();
            informationText.text = item.GetItemInformation();
        }

        if (item.GetItemType() != Item.Type.Valuables)
        {
            useItemPanel.SetActive(true);
            itemPanelCanvasGroup.interactable = false;
            currentCommand = CommandMode.UseItemPanel;

            //ItemPanel�ōŌ�ɂǂ��I�����Ă������H
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            useItemPanel.transform.SetAsLastSibling();
            EventSystem.current.SetSelectedGameObject(useItemPanel.transform.GetChild(0).gameObject);
            useItemPanelCanvasGroup.interactable = true;
            Input.ResetInputAxes();
        }
    }

    public void UseItem(AllyStatus allyStatus, Item item)
    {
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(true);

        //UseItemPanel�łǂ���Ō�ɑI�����Ă�����
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject characterButtonIns;

        //�p�[�e�B�[�����o�[���̃{�^�����쐬
        foreach (var member in partyStatus.GetAllyStatus())
        {
            characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
            characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
            characterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItemToCharacter(allyStatus, member, item));
        }

        //UseItemSelectCharacterPanel�Ɉڍs����
        currentCommand = CommandMode.UseItemSelectCharacterPanel;
        useItemSelectCharacterPanel.transform.SetAsLastSibling();
        EventSystem.current.SetSelectedGameObject(useItemSelectCharacterPanel.transform.GetChild(0).gameObject);
        useItemSelectCharacterPanelCanvasGroup.interactable = true;
        Input.ResetInputAxes();
    }

    public void UseItemToCharacter(AllyStatus fromChara, AllyStatus toChara, Item item)
    {
        useItemInformationPanel.SetActive(true);
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(false);

        //HP�񕜃A�C�e��
        if (item.GetItemType() == Item.Type.HPRecovery)
        {
            //HP�����^���̏ꍇ�͎g��Ȃ�
            if (toChara.GetHP() == toChara.GetMaxHP())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "��HP�͖��^����";
            }
            else
            {
                //�񕜏���
                toChara.SetHP(toChara.GetHP() + item.GetAmount());

                //�A�C�e�����g�p�������Ƃ�\��
                useItemInformationPanel.GetComponentInChildren<Text>().text =fromChara.GetCharacterName()+"��"+item.GetItemName()+"��"+toChara.GetCharacterName()+"�Ɏg�p����\n"+ toChara.GetCharacterName() + "��HP��"+item.GetAmount()+"�񕜂���";

                //�A�C�e�������炷
                fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);
            }
            //SP�񕜃A�C�e��
        }else if (item.GetItemType() == Item.Type.SPRecovery)
        {
            //SP�����^���̏ꍇ�͎g��Ȃ�
            if (toChara.GetSP() == toChara.GetMaxSP())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "��SP�͖��^����";
            }
            else
            {
                //�񕜏���
                toChara.SetSP(toChara.GetSP() + item.GetAmount());

                //�A�C�e�����g�p�������Ƃ�\��
                useItemInformationPanel.GetComponentInChildren<Text>().text = fromChara.GetCharacterName() + "��" + item.GetItemName() + "��" + toChara.GetCharacterName() + "�Ɏg�p����\n" + toChara.GetCharacterName() + "��SP��" + item.GetAmount() + "�񕜂���";

                //�A�C�e�������炷
                fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);
            }
            //SP�񕜃A�C�e��
        }

        //�A�C�e�����g�p������A�A�C�e�����g�p���鑊���UseItemSelectCharacterPanel�̎q�v�f�̃{�^�����폜
        for(int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        //itemPanelButtonList����Y������A�C�e����T�������X�V����
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
        itemButton.transform.Find("Num").GetComponent<Text>().text = fromChara.get_item_number(item).ToString();

        //�A�C�e������0��������{�^���ƃL�����N�^�[�X�e�[�^�X����A�C�e�����폜
        if (fromChara.get_item_number(item) == 0)
        {
            //�A�C�e������0�ɂȂ������C��ItemPanel�ɖ߂����߁AUseItemPanel����UseItemSelectCharacterPanel���ł̃I�u�W�F�N�g�o�^���폜
            selectedGameObjectStack.Pop();
            selectedGameObjectStack.Pop();
            //itemPanelButtonList����A�C�e���p�l���{�^�����폜
            itemPanelButtonList.Remove(itemButton);
            //�A�C�e���p�l���{�^�����g�̍폜
            Destroy(itemButton);
            //�A�C�e����n�����L�������g�̂�ItemDictionary���炻�̃A�C�e�����폜
            fromChara.get_item_dictionary().Remove(item);
            //ItemPanel�ɖ߂邽�߁AUseItemPanel���ɍ�����{�^�����폜
            for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

            //�A�C�e������0�ɂȂ����̂ŁACommandMode.NoItemPassed�ɕύX
            currentCommand = CommandMode.NoItemPassed;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        } else
        {
            //�A�C�e�������c���Ă���ꍇ��UseItemPanel�ŃA�C�e�����ǂ����邩�̑I���ɂȂ�
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
    }
    
    public void PassItem(AllyStatus allyStatus,Item item)
    {
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(true);

        //UseItemPanel�łǂ���ŏ��ɑI�����Ă�����
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject characterButtonIns;

        //�p�[�e�B�[�����o�[���̃{�^�����A�������g�������č쐬
        foreach(var member in partyStatus.GetAllyStatus())
        {
            if (member != allyStatus)
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItemToOtherCharacter(allyStatus, member, item));
            }
        }

        //UseItemSelectCharacterPanel�Ɉڍs
        currentCommand = CommandMode.UseItemSelectCharacterPanel;
        useItemSelectCharacterPanel.transform.SetAsLastSibling();
        EventSystem.current.SetSelectedGameObject(useItemSelectCharacterPanel.transform.GetChild(0).gameObject);
        useItemSelectCharacterPanelCanvasGroup.interactable = true;
        Input.ResetInputAxes();
    }
    
    public void PassItemToOtherCharacter(AllyStatus fromChara, AllyStatus toChara, Item item)
    {
        useItemInformationPanel.SetActive(true);
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(false);

        //�����Ă���A�C�e���������炷
        fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);

        //�n���ꂽ�L�����N�^�[���A�C�e���������Ă��Ȃ���΂��̃A�C�e����o�^
        if (!toChara.get_item_dictionary().ContainsKey(item))
        {
            toChara.set_item_dictionary(item, 0);
        }

        //�n���ꂽ�L�����̃A�C�e�����𑝂₷
        toChara.set_item_number(item, toChara.get_item_number(item) + 1);

        //�A�C�e����n���I�������A�C�e����n�������UseItemSelectCharacterPanel�̎q�v�f�̃{�^����S�폜
        for(int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        //itemPanelButtonLIst����Y���A�C�e����T���A�����X�V
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
        itemButton.transform.Find("Num").GetComponent<Text>().text = fromChara.get_item_number(item).ToString();
        //�A�C�e����n�������Ƃ�\��
        useItemInformationPanel.GetComponentInChildren<Text>().text = fromChara.GetCharacterName() + "��" + toChara.GetCharacterName() + "��" + item.GetItemName() + "��n����";

        //�A�C�e������0��������{�^���ƃL�����X�e�[�^�X����A�C�e�����폜
        if (fromChara.get_item_number(item) == 0)
        {
            //�������Ă��镐���Z��������O��
            if (fromChara.get_equiped_armor() == item)
            {
                fromChara.set_equiped_armor(null);
            }
            if (fromChara.get_equiped_weapon() == item)
            {
                fromChara.set_equiped_weapon(null);
            }

            //�A�C�e����0�ɂȂ������C��ItemPanel�ɖ߂����߂ɁAUseItemPanel����UseItemSelectCharacterPanel���ł̃I�u�W�F�N�g�o�^���폜
            selectedGameObjectStack.Pop();
            selectedGameObjectStack.Pop();

            //itemPanelButtonList����itemButton���폜
            itemPanelButtonList.Remove(itemButton);

            //itemButton���g�̍폜
            Destroy(itemButton);

            //�A�C�e����n�����L�������g��ItemDictionary���炻�̃A�C�e�����폜
            fromChara.get_item_dictionary().Remove(item);

            //ItemPanel�ɖ߂邽�߁AUseItemPanel���ɍ�����{�^�����폜
            for(int i=useItemPanel.transform.childCount-1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

            //�A�C�e������0�ɂȂ����̂ŁACommanMode.NoItemPassed�ɕύX
            currentCommand = CommandMode.NoItemPassed;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
        else
        {
            //�A�C�e�������c���Ă���ꍇ�́AUseItemPanel�ŃA�C�e�����ǂ����邩�̑I���ɖ߂�
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
    }

    public void ThrowAwayItem(AllyStatus allyStatus, Item item)
    {
        //�A�C�e���������炷
        allyStatus.set_item_number(item, allyStatus.get_item_number(item) - 1);

        //
        if (allyStatus.get_item_number(item) == 0)
        {
            //
            if (item == allyStatus.get_equiped_armor())
            {
                var equipedArmorOrWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipedArmorOrWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "";
                equipedArmorOrWeaponButton = null;
                allyStatus.set_equiped_armor(null);
            }
            else if (item == allyStatus.get_equiped_weapon())   
            {
                var equipedArmorOrWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipedArmorOrWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "";
                allyStatus.set_equiped_weapon(null);
            }
        }

        //
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
        itemButton.transform.Find("Num").GetComponent<Text>().text = allyStatus.get_item_number(item).ToString();
        useItemInformationPanel.transform.GetComponentInChildren<Text>().text = item.GetItemName() + "���̂Ă�";

        //
        if (allyStatus.get_item_number(item) == 0)
        {
            selectedGameObjectStack.Pop();
            itemPanelButtonList.Remove(itemButton);
            Destroy(itemButton);
            allyStatus.get_item_dictionary().Remove(item);

            currentCommand = CommandMode.NoItemPassed;
            useItemPanelCanvasGroup.interactable = false;
            useItemPanel.SetActive(false);
            useItemInformationPanel.transform.SetAsLastSibling();
            useItemInformationPanel.SetActive(true);

            //
            for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

        }
        else
        {
            useItemPanelCanvasGroup.interactable = false;
            useItemInformationPanel.transform.SetAsLastSibling();
            useItemInformationPanel.SetActive(true);
            currentCommand = CommandMode.UseItemPanelToUseItemPanel;
        }

        Input.ResetInputAxes();
    }

    public void Equip(AllyStatus allyStatus, Item item)
    {
        //
        if (allyStatus.GetCharacterName() == "���j�e�B�����")
        {
            if (item.GetItemType() == Item.Type.ArmorAll || item.GetItemType() == Item.Type.ArmorPlayer)
            {
                var equipArmorButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipArmorButton.transform.Find("Equip").GetComponent<Text>().text = "E";

                //
                if (allyStatus.get_equiped_armor() != null)
                {
                    equipArmorButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == allyStatus.get_equiped_armor().GetItemName());
                    equipArmorButton.transform.Find("Equip").GetComponent<Text>().text = "";
                }
                allyStatus.set_equiped_armor(item);
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔�����";
            }
            else if (item.GetItemType() == Item.Type.WeaponAll || item.GetItemType() == Item.Type.WeaponPlayer)
            {
                var equipWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "E";

                //
                if (allyStatus.get_equiped_weapon() != null)
                {
                    equipWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == allyStatus.get_equiped_weapon().GetItemName());
                    equipWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "";
                }
                allyStatus.set_equiped_weapon(item);
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔�����";
            }
            else
            {
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔��ł��Ȃ��I";
            }

        }
        else if (allyStatus.GetCharacterName() == "���у��j�e�B�����")
        {
            if (item.GetItemType() == Item.Type.ArmorAll)
            {
                var equipArmorButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipArmorButton.transform.Find("Equip").GetComponent<Text>().text = "E";

                //
                if (allyStatus.get_equiped_armor() != null)
                {
                    equipArmorButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == allyStatus.get_equiped_armor().GetItemName());
                    equipArmorButton.transform.Find("Equip").GetComponent<Text>().text = "";
                }
                allyStatus.set_equiped_armor(item);
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔�����";
            }
            else if (item.GetItemType() == Item.Type.WeaponAll)
            {
                var equipWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
                equipWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "E";

                //
                if (allyStatus.get_equiped_weapon() != null)
                {
                    equipWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == allyStatus.get_equiped_weapon().GetItemName());
                    equipWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "";
                }
                allyStatus.set_equiped_weapon(item);
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔�����";
            }
            else
            {
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "�𑕔��ł��Ȃ��I";
            }

        }

        //������؂�ւ�����ItemPanel�ɖ߂�
        useItemPanelCanvasGroup.interactable = false;
        useItemPanel.SetActive(false);
        itemPanelCanvasGroup.interactable = true;

        //
        for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }

        useItemInformationPanel.transform.SetAsLastSibling();
        useItemInformationPanel.SetActive(true);

        currentCommand = CommandMode.UseItemPanelToItemPanel;

        Input.ResetInputAxes();

    }

    //�������O��
    public void RemoveEquip(AllyStatus allyStatus, Item item)
    {
        //
        if (item.GetItemType() == Item.Type.ArmorAll || item.GetItemType() == Item.Type.ArmorPlayer)
        {
            var equipPanelButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
            equipPanelButton.transform.Find("Equip").GetComponent<Text>().text = "";
            allyStatus.set_equiped_armor(null);
        }else if(item.GetItemType()==Item.Type.WeaponAll || item.GetItemType() == Item.Type.WeaponPlayer)
        {
            var equipPanelButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
            equipPanelButton.transform.Find("Equip").GetComponent<Text>().text = "";
            allyStatus.set_equiped_weapon(null);
        }

        //�������O�������Ƃ�\��
        useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "��" + item.GetItemName() + "���O����";

        //�������O������ItemPanel�ɖ߂鏈��
        useItemPanelCanvasGroup.interactable = false;
        useItemPanel.SetActive(false);
        itemPanelCanvasGroup.interactable = true;

        //ItemPanel�ɂ��ǂ�̂ŁAUseItemPanel�̎q�v�f�̃{�^����S�폜
        for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }

        useItemInformationPanel.transform.SetAsLastSibling();
        useItemInformationPanel.SetActive(true);

        currentCommand = CommandMode.UseItemPanelToItemPanel;
        Input.ResetInputAxes();
    }
}
