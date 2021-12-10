using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName ="Item",menuName ="CreateNewItem")]

public class Item : ScriptableObject
{
    public enum Type
    {
        HPRecovery,
        SPRecovery,
        WeaponAll,
        WeaponPlayer,
        ArmorAll,
        ArmorPlayer,
        Valuables
    }

    [SerializeField]
    public Type item_type = Type.HPRecovery;
    [SerializeField]
    private string item_name = "";
    [SerializeField]
    private string hiragana_item_name = "";
    [SerializeField]
    private string item_information = "";
    [SerializeField]
    private int amount = 0;

    public Type GetItemType()
    {
        return item_type;
    }

    public string GetItemName()
    {
        return item_name;
    }

    public string GetItemKanaName()
    {
        return hiragana_item_name;
    }

    public string GetItemInformation()
    {
        return item_information;
    }

    //アイテムの強さ(？)を返す
    public int GetAmount()
    {
        return amount;
    }
}
