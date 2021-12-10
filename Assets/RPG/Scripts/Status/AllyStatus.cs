using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName ="AllyStatus",menuName ="CreateNewAllyStatus")]
public class AllyStatus : CharacterStatus
{
    [SerializeField]
    private int earned_EXP = 0;
    [SerializeField]
    private Item equiped_weapon = null;
    [SerializeField]
    private Item equiped_armor = null;
    //ÉAÉCÉeÉÄÇ∆å¬êîÇÃDictionary
    [SerializeField]
    private ItemDictionary item_dictionary = null;

    public void set_earned_EXP(int earned_EXP)
    {
        this.earned_EXP = earned_EXP;
    }
    public int get_earned_EXP()
    {
        return earned_EXP;
    }

    public void set_equiped_weapon(Item equiped_weapon)
    {
        this.equiped_weapon = equiped_weapon;
    }
    public Item get_equiped_weapon()
    {
        return equiped_weapon;
    }

    public void set_equiped_armor(Item equiped_armor)
    {
        this.equiped_armor = equiped_armor;
    }
    public Item get_equiped_armor()
    {
        return equiped_armor;
    }

    public void create_item_dictionary(ItemDictionary item_dictionary)
    {
        this.item_dictionary = item_dictionary;
    }
    public void set_item_dictionary(Item item, int num = 0)
    {
        item_dictionary.Add(item, num);
    }

    public ItemDictionary get_item_dictionary()
    {
        return item_dictionary;
    }
    public IOrderedEnumerable<KeyValuePair<Item,int>> get_sorted_item_dictionary()
    {
        return item_dictionary.OrderBy(item => item.Key.GetItemKanaName());
    }
    public int set_item_number(Item temp_item,int num)
    {
        return item_dictionary[temp_item] = num;
    }
    public int get_item_number(Item item)
    {
        return item_dictionary[item];
    }


}
