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
    //プレイヤーキャラコマンドスクリプト
    private UnityChanCommandScript unityChanCommandScript;

    //最初に選択するButtonのTransform
    private GameObject firstSelectButton;

    //コマンドパネル
    private GameObject commandPanel;
    //ステータス表示パネル
    private GameObject statusPanel;
    //キャラクター選択パネル
    private GameObject selectCharacterPanel;

    //コマンドパネルのCanvasGroup
    private CanvasGroup commandPanelCanvasGroup;
    //キャラ選択パネルのCanvasGroup
    private CanvasGroup selectCharacterPanelCanvasGroup;

    //キャラ名
    private Text characterNameText;
    //ステータスタイトルテキスト
    private Text statusTitleText;
    //ステータスパラメータテキスト１
    private Text statusParamater1Text;
    //ステータスパラメータテキスト２
    private Text statusParamater2Text;
    //パーティステータス(後で)
    [SerializeField]
    private PartyStatus partyStatus = null;

    //キャラ選択ボタンのプレハブ
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //最後に選択していたゲームオブジェクトをスタック
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    //アイテム表示パネル
    private GameObject itemPanel;
    //アイテムパネルボタンを表示する場所
    private GameObject content;
    //アイテムを使う選択パネル
    private GameObject useItemPanel;
    //アイテムを使う相手をだれにするかを選ぶパネル
    private GameObject useItemSelectCharacterPanel;
    //アイテム情報を表示するパネル
    private GameObject itemInformationPanel;
    //アイテム使用後の情報(使用メッセージなど)を表示するパネル
    private GameObject useItemInformationPanel;

    //アイテムパネルのキャンバスグループ
    private CanvasGroup itemPanelCanvasGroup;
    //アイテムを使う選択パネルのキャンバスグループ
    private CanvasGroup useItemPanelCanvasGroup;
    //アイテムを使うキャラクターを選択するためのキャンバスグループ
    private CanvasGroup useItemSelectCharacterPanelCanvasGroup;

    //情報表示タイトルテキスト
    private Text informationTitleText;
    //情報表示テキスト
    private Text informationText;

    //キャラクターアイテムのボタンのプレハブ
    [SerializeField]
    private GameObject itemPanelButtonPrefab = null;
    //アイテム使用時のボタンのプレハブ
    [SerializeField]
    private GameObject useItemPanelButtonPrefab = null;

    //アイテムボタン一覧
    private List<GameObject> itemPanelButtonList = new List<GameObject>();


    void Awake()
    {
        //コマンド画面を開く処理をしているUnityChanCommandScriptを取得
        unityChanCommandScript = GameObject.FindWithTag("Player").GetComponent<UnityChanCommandScript>();
        //現在のコマンドを初期化
        currentCommand = CommandMode.CommandPanel;
        //階層を辿って取得
        firstSelectButton = transform.Find("CommandPanel/StatusButton").gameObject;
        //パネル系
        commandPanel = transform.Find("CommandPanel").gameObject;
        statusPanel = transform.Find("StatusPanel").gameObject;
        selectCharacterPanel = transform.Find("SelectCharacterPanel").gameObject;
        useItemSelectCharacterPanel = transform.Find("UseItemSelectCharacterPanel").gameObject;

        //CanvasGroup
        commandPanelCanvasGroup = commandPanel.GetComponent<CanvasGroup>();
        selectCharacterPanelCanvasGroup = selectCharacterPanel.GetComponent<CanvasGroup>();

        //ステータス用テキスト
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

        //情報表示用テキスト
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

        //アイテムパネルボタンがあればすべて削除
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        //アイテムを使うキャラクター選択ボタンがあればすべて削除
        for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }

        //アイテムを使う相手のキャラクター選択ボタンがあればすべて削除
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
                //リストをクリア
                itemPanelButtonList.Clear();
                //ItemPanelでCancelを押したらcontent以下のアイテムパネルボタンをすべて削除
                for(int i = content.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(content.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                selectCharacterPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.ItemPanelSelectCharacter;

                //アイテムを選択し、どう使うかを選択しているとき
            }else if (currentCommand == CommandMode.UseItemPanel)
            {
                useItemPanelCanvasGroup.interactable = false;
                useItemPanel.SetActive(false);
                // UseItemPanelでCancelボタンを押したらUseItemPanelの子要素のボタンをすべて削除
                for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(useItemPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                itemPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.ItemPanel;

                //アイテム使用時の使用相手を選択しているとき
            }else if (currentCommand == CommandMode.UseItemSelectCharacterPanel)
            {
                useItemSelectCharacterPanelCanvasGroup.interactable = false;
                useItemSelectCharacterPanel.SetActive(false);
                //UseItemSelectCharacterPanelでCancelボタンを推したら、アイテムを使用するキャラボタンを全て削除
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
                //アイテムを使用する相手のキャラ選択からアイテムをどうするかに移行するとき
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
                //アイテムを捨てるを選択した後の状態
            }else if (currentCommand == CommandMode.UseItemPanelToItemPanel)
            {
                if (Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.UseItemPanel;
                    useItemInformationPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;
                }
                //アイテムを使用、渡す、捨てるを選択した後にそのアイテムの数が0になった時
            }else if (currentCommand == CommandMode.NoItemPassed)
            {
                if (Input.anyKeyDown || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f))
                {
                    currentCommand = CommandMode.ItemPanel;
                    useItemInformationPanel.SetActive(false);
                    useItemPanel.SetActive(false);
                    itemPanel.transform.SetAsLastSibling();
                    itemPanelCanvasGroup.interactable = true;

                    //アイテムパネルボタンがあれば最初のアイテムパネルボタンを選択
                    if (content.transform.childCount != 0)
                    {
                        EventSystem.current.SetSelectedGameObject(content.transform.GetChild(0).gameObject);
                    } else
                    {
                        //アイテムパネルボタンがなければ(＝アイテムを持っていないのであれば)ItemSelectPanelに戻る
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

    //選択したコマンドで処理分け
    public void SelectCommand(string command)
    {
        if (command == "Status")
        {
            currentCommand = CommandMode.StatusPanelSelectCharacter;
            //UIのオン・オフや選択アイコンの設定
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            //パーティーメンバー分のボタンを作成
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

        //階層を一番最後に並べ替え
        selectCharacterPanel.transform.SetAsLastSibling();
        selectCharacterPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.transform.GetChild(0).gameObject);
    }

    //キャラのステータスを表示
    public void ShowStatus(AllyStatus allyStatus)
    {
        currentCommand = CommandMode.StatusPanel;
        statusPanel.SetActive(true);
        //キャラ名を表示
        characterNameText.text = allyStatus.GetCharacterName();

        //タイトルの表示
        var text = "レベル\n";
        text += "HP\n";
        text += "SP\n";
        text += "経験値\n";
        text += "状態異常\n";
        text += "STR\n";
        text += "AGI\n";
        text += "VIT\n";
        text += "DEX\n";
        text += "INT\n";
        text += "LUK\n";
        text += "装備武器\n";
        text += "装備鎧\n";
        text += "攻撃力\n";
        text += "防御力\n";
        statusTitleText.text = text;

        //HPとSPのDivision記号の表示
        text = "\n";
        text += allyStatus.GetHP() + "\n";
        text += allyStatus.GetSP() + "\n";
        statusParamater1Text.text = text;

        //ステータスパラメータの表示
        text = allyStatus.GetLevel() + "\n";
        text += allyStatus.GetMaxHP() + "\n";
        text += allyStatus.GetMaxSP() + "\n";
        text += allyStatus.get_earned_EXP() + "\n";
        if(!allyStatus.IsPoisonState() && !allyStatus.IsNumbnessState())
        {
            text += "正常\n";
        }
        else
        {
            if (allyStatus.IsPoisonState())
            {
                text += "毒";
                if (allyStatus.IsNumbnessState())
                {
                    text += "シビレ\n";
                }
            } else
            {
                if (allyStatus.IsNumbnessState())
                {
                    text += "シビレ\n";
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

    //キャラクターが持っているアイテムのボタン表示
    public void CreateItemPanelButton(AllyStatus allyStatus)
    {
        itemInformationPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = false;

        //アイテムパネルボタンを何個作製したか
        int itemPanelButtonNum = 0;
        GameObject itemButtonIns;
        //選択したキャラクターのアイテム数分、アイテムパネルボタンを作成
        //持っているアイテム分のボタンの作製と、クリック時の実行メソッドの設定
        foreach(var item in allyStatus.get_item_dictionary().Keys)
        {
            itemButtonIns = Instantiate<GameObject>(itemPanelButtonPrefab, content.transform);
            itemButtonIns.transform.Find("ItemName").GetComponent<Text>().text = item.GetItemName();
            itemButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItem(allyStatus, item));
            itemButtonIns.GetComponent<ItemPanelButtonScript>().SetParam(item);

            //アイテム数表示
            itemButtonIns.transform.Find("Num").GetComponent<Text>().text = allyStatus.get_item_number(item).ToString();

            //装備している武器や防具には、名前の横にEを表示し、そのTextを保持して書く
            if (allyStatus.get_equiped_weapon() == item)
            {
                itemButtonIns.transform.Find("Equip").GetComponent<Text>().text = "E";
            } else if (allyStatus.get_equiped_armor() == item)
            {
                itemButtonIns.transform.Find("Equip").GetComponent<Text>().text = "E";
            }

            //アイテムボタンリストに追加
            itemPanelButtonList.Add(itemButtonIns);
            //アイテムパネルボタン番号を更新(1個増やす)
            itemPanelButtonNum++;
        }

        if (content.transform.childCount != 0)
        {
            //SelectCharacterPanelで最後にどのゲームオブジェクトを選択していたか
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
            informationText.text = "アイテムを持っていません";
            selectCharacterPanelCanvasGroup.interactable = true;
        }
    }

    public void SelectItem(AllyStatus allyStatus,Item item)
    {
        //アイテムの種類に応じてできる項目を変更する
        if(item.GetItemType()==Item.Type.ArmorAll || item.GetItemType()==Item.Type.ArmorPlayer || item.GetItemType() == Item.Type.WeaponAll || item.GetItemType() == Item.Type.WeaponPlayer)
        {
            var itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            if (item == allyStatus.get_equiped_weapon() || item == allyStatus.get_equiped_armor())
            {
                itemMenuButtonIns.GetComponentInChildren<Text>().text = "装備を外す";
                itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => RemoveEquip(allyStatus, item));
            }
            else
            {
                itemMenuButtonIns.GetComponentInChildren<Text>().text = "装備する";
                itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => Equip(allyStatus, item));
            }

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "渡す";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItem(allyStatus, item));
            
            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "捨てる";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => ThrowAwayItem(allyStatus, item));

        }else if (item.GetItemType() == Item.Type.HPRecovery || item.GetItemType() == Item.Type.SPRecovery)
        {
            var itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "使う";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(allyStatus, item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "渡す";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItem(allyStatus, item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "捨てる";
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

            //ItemPanelで最後にどれを選択していたか？
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

        //UseItemPanelでどれを最後に選択していたか
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject characterButtonIns;

        //パーティーメンバー分のボタンを作成
        foreach (var member in partyStatus.GetAllyStatus())
        {
            characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
            characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
            characterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItemToCharacter(allyStatus, member, item));
        }

        //UseItemSelectCharacterPanelに移行する
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

        //HP回復アイテム
        if (item.GetItemType() == Item.Type.HPRecovery)
        {
            //HPが満タンの場合は使わない
            if (toChara.GetHP() == toChara.GetMaxHP())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "のHPは満タンだ";
            }
            else
            {
                //回復処理
                toChara.SetHP(toChara.GetHP() + item.GetAmount());

                //アイテムを使用したことを表示
                useItemInformationPanel.GetComponentInChildren<Text>().text =fromChara.GetCharacterName()+"は"+item.GetItemName()+"を"+toChara.GetCharacterName()+"に使用した\n"+ toChara.GetCharacterName() + "のHPが"+item.GetAmount()+"回復した";

                //アイテムを減らす
                fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);
            }
            //SP回復アイテム
        }else if (item.GetItemType() == Item.Type.SPRecovery)
        {
            //SPが満タンの場合は使わない
            if (toChara.GetSP() == toChara.GetMaxSP())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "のSPは満タンだ";
            }
            else
            {
                //回復処理
                toChara.SetSP(toChara.GetSP() + item.GetAmount());

                //アイテムを使用したことを表示
                useItemInformationPanel.GetComponentInChildren<Text>().text = fromChara.GetCharacterName() + "は" + item.GetItemName() + "を" + toChara.GetCharacterName() + "に使用した\n" + toChara.GetCharacterName() + "のSPが" + item.GetAmount() + "回復した";

                //アイテムを減らす
                fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);
            }
            //SP回復アイテム
        }

        //アイテムを使用したら、アイテムを使用する相手のUseItemSelectCharacterPanelの子要素のボタンを削除
        for(int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        //itemPanelButtonListから該当するアイテムを探し数を更新する
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
        itemButton.transform.Find("Num").GetComponent<Text>().text = fromChara.get_item_number(item).ToString();

        //アイテム数が0だったらボタンとキャラクターステータスからアイテムを削除
        if (fromChara.get_item_number(item) == 0)
        {
            //アイテム数が0になったら一気にItemPanelに戻すため、UseItemPanel内とUseItemSelectCharacterPanel内でのオブジェクト登録を削除
            selectedGameObjectStack.Pop();
            selectedGameObjectStack.Pop();
            //itemPanelButtonListからアイテムパネルボタンを削除
            itemPanelButtonList.Remove(itemButton);
            //アイテムパネルボタン自身の削除
            Destroy(itemButton);
            //アイテムを渡したキャラ自身ののItemDictionaryからそのアイテムを削除
            fromChara.get_item_dictionary().Remove(item);
            //ItemPanelに戻るため、UseItemPanel内に作ったボタンを削除
            for(int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

            //アイテム数が0になったので、CommandMode.NoItemPassedに変更
            currentCommand = CommandMode.NoItemPassed;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        } else
        {
            //アイテム数が残っている場合はUseItemPanelでアイテムをどうするかの選択になる
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
    }
    
    public void PassItem(AllyStatus allyStatus,Item item)
    {
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(true);

        //UseItemPanelでどれを最初に選択していたか
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject characterButtonIns;

        //パーティーメンバー分のボタンを、自分自身を除いて作成
        foreach(var member in partyStatus.GetAllyStatus())
        {
            if (member != allyStatus)
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => PassItemToOtherCharacter(allyStatus, member, item));
            }
        }

        //UseItemSelectCharacterPanelに移行
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

        //持っているアイテム数を減らす
        fromChara.set_item_number(item, fromChara.get_item_number(item) - 1);

        //渡されたキャラクターがアイテムを持っていなければそのアイテムを登録
        if (!toChara.get_item_dictionary().ContainsKey(item))
        {
            toChara.set_item_dictionary(item, 0);
        }

        //渡されたキャラのアイテム数を増やす
        toChara.set_item_number(item, toChara.get_item_number(item) + 1);

        //アイテムを渡し終わったらアイテムを渡す相手のUseItemSelectCharacterPanelの子要素のボタンを全削除
        for(int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        //itemPanelButtonLIstから該当アイテムを探し、数を更新
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemName").GetComponent<Text>().text == item.GetItemName());
        itemButton.transform.Find("Num").GetComponent<Text>().text = fromChara.get_item_number(item).ToString();
        //アイテムを渡したことを表示
        useItemInformationPanel.GetComponentInChildren<Text>().text = fromChara.GetCharacterName() + "は" + toChara.GetCharacterName() + "に" + item.GetItemName() + "を渡した";

        //アイテム数が0だったらボタンとキャラステータスからアイテムを削除
        if (fromChara.get_item_number(item) == 0)
        {
            //装備している武器や鎧だったら外す
            if (fromChara.get_equiped_armor() == item)
            {
                fromChara.set_equiped_armor(null);
            }
            if (fromChara.get_equiped_weapon() == item)
            {
                fromChara.set_equiped_weapon(null);
            }

            //アイテムが0になったら一気にItemPanelに戻すために、UseItemPanel内とUseItemSelectCharacterPanel内でのオブジェクト登録を削除
            selectedGameObjectStack.Pop();
            selectedGameObjectStack.Pop();

            //itemPanelButtonListからitemButtonを削除
            itemPanelButtonList.Remove(itemButton);

            //itemButton自身の削除
            Destroy(itemButton);

            //アイテムを渡したキャラ自身のItemDictionaryからそのアイテムを削除
            fromChara.get_item_dictionary().Remove(item);

            //ItemPanelに戻るため、UseItemPanel内に作ったボタンを削除
            for(int i=useItemPanel.transform.childCount-1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

            //アイテム数が0になったので、CommanMode.NoItemPassedに変更
            currentCommand = CommandMode.NoItemPassed;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
        else
        {
            //アイテム数が残っている場合は、UseItemPanelでアイテムをどうするかの選択に戻る
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
    }

    public void ThrowAwayItem(AllyStatus allyStatus, Item item)
    {
        //アイテム数を減らす
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
        useItemInformationPanel.transform.GetComponentInChildren<Text>().text = item.GetItemName() + "を捨てた";

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
        if (allyStatus.GetCharacterName() == "ユニティちゃん")
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
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備した";
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
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備した";
            }
            else
            {
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備できない！";
            }

        }
        else if (allyStatus.GetCharacterName() == "ちびユニティちゃん")
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
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備した";
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
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備した";
            }
            else
            {
                useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を装備できない！";
            }

        }

        //装備を切り替えたらItemPanelに戻る
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

    //装備を外す
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

        //装備を外したことを表示
        useItemInformationPanel.transform.GetComponentInChildren<Text>().text = allyStatus.GetCharacterName() + "は" + item.GetItemName() + "を外した";

        //装備を外したらItemPanelに戻る処理
        useItemPanelCanvasGroup.interactable = false;
        useItemPanel.SetActive(false);
        itemPanelCanvasGroup.interactable = true;

        //ItemPanelにもどるので、UseItemPanelの子要素のボタンを全削除
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
