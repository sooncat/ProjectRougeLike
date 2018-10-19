using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class FightView : BaseView {

    UINode _rootNode;
    UINode _enemyNode;
    UINode _modelNode;
    Transform _heroNodeRoot;
    Transform _itemNodeRoot;

    /// <summary>
    /// 在界面上显示的英雄
    /// {Key:heroId, Value:nodeTrans}
    /// </summary>
    Dictionary<int, Transform> _heroNodes;
    
    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        rootNode.GetRef("AttackBtn").GetComponent<Button>().onClick.AddListener(OnAttackBtnClicked);
        rootNode.GetRef("SkillBtn").GetComponent<Button>().onClick.AddListener(OnSkillBtnClicked);
        rootNode.GetRef("PetBtn").GetComponent<Button>().onClick.AddListener(OnPetBtnClicked);

        _rootNode = rootNode;
        _rootNode.gameObject.SetActive(false);

        _enemyNode = rootNode.GetNode("Enemy");
        _modelNode = rootNode.GetNode("Models");

        _heroNodeRoot = rootNode.GetRef("HeroRoot");
        _itemNodeRoot = rootNode.GetRef("ItemsContent");
        
        EventSys.Instance.AddHander(ViewEvent.CreateFightView, OnCreateFightView);
        EventSys.Instance.AddHander(ViewEvent.SetSelectedHero, OnChangeHero);
    }

    void OnCreateFightView(int id, object p1, object p2)
    {
        //create enemy
        Dictionary<int, Enemy> enemies = (Dictionary<int, Enemy>)p2;
        foreach (KeyValuePair<int, Enemy> pair in enemies)
        {
            Enemy e = pair.Value;
            Image cg = _enemyNode.GetRef("Cg").GetComponent<Image>();
            cg.sprite = ResourceSys.Instance.GetSprite(e.CreatureData.Cg);
            Slider s = _enemyNode.GetRef("HpSlider").GetComponent<Slider>();
            s.value = e.CreatureData.HpPercent;
        }
        
        //create heros
        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p2;
        _heroNodes = new Dictionary<int, Transform>();
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            FightHero fh = pair.Value;

            GameObject go = Instantiate(_modelNode.GetRef("Hero").gameObject);
            go.transform.SetParent(_heroNodeRoot);

            UINode node = go.GetComponent<UINode>();
            Image ico = node.GetRef("Icon").GetComponent<Image>();
            ico.sprite = ResourceSys.Instance.GetSprite(fh.CreatureData.Icon);
            Slider hs = node.GetRef("HpSlider").GetComponent<Slider>();
            hs.value = fh.CreatureData.HpPercent;
            Slider ms = node.GetRef("MpSlider").GetComponent<Slider>();
            ms.value = fh.CreatureData.MpPercent;
            node.GetRef("Selected").gameObject.SetActive(false);

            Dropable dropable = go.AddComponent<Dropable>();
            dropable.ActionId = fh.Id;

            _heroNodes.Add(fh.Id, go.transform);
        }

        _rootNode.gameObject.SetActive(true);
    }

    void OnChangeHero(int id, object p1, object p2)
    {
        //reset hero selected label
        FightHero fh = (FightHero)p1;
        Transform t = _heroNodes[fh.Id];
        UINode node = t.GetComponent<UINode>();
        node.GetRef("Selected").gameObject.SetActive(true);
        
        //reset items

    }

    void OnAttackBtnClicked()
    {
        EventSys.Instance.AddEvent(InputEvent.FightAttack);
    }
    void OnSkillBtnClicked()
    {
        //Not Support Yet.
    }
    void OnPetBtnClicked()
    {
        //Not Support Yet.
    }

    
}
