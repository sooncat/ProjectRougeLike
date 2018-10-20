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
        GameObject enemyHurt = _enemyNode.GetRef("Hurt").gameObject;
        DelayAction da = enemyHurt.AddComponent<DelayAction>();
        da.DelaySecond = 1;
        da.DAction = () => { enemyHurt.SetActive(false); };

        enemyHurt.SetActive(false);

        _modelNode = rootNode.GetNode("Models");

        _heroNodeRoot = rootNode.GetRef("HeroRoot");
        _itemNodeRoot = rootNode.GetRef("ItemsContent");

        _rootNode.GetRef("Win").gameObject.SetActive(false);
        _rootNode.GetRef("Lose").gameObject.SetActive(false);
        
        EventSys.Instance.AddHander(ViewEvent.CreateFightView, OnCreateFightView);
        EventSys.Instance.AddHander(ViewEvent.SetSelectedHero, OnChangeHero);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateRound, OnUpdateRound);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateEnemyState, OnUpdateEnemy);
        EventSys.Instance.AddHander(ViewEvent.FightShowWin, OnShowWin);
        EventSys.Instance.AddHander(ViewEvent.FightEnemyHurt, OnEnemyHurt);
        EventSys.Instance.AddHander(ViewEvent.FightShowLose, OnShowLose);
        EventSys.Instance.AddHander(ViewEvent.FightReturnToStage, OnFinish);
        
    }

    void OnFinish(object p1, object p2)
    {
        _rootNode.gameObject.SetActive(false);
    }

    void OnUpdateEnemy(object p1, object p2)
    {
        Enemy e = (Enemy)p1;
        Slider s = _enemyNode.GetRef("HpSlider").GetComponent<Slider>();
        s.value = e.CreatureData.HpPercent;
    }

    void OnEnemyHurt(object p1, object p2)
    {
        int damage = (int)p1;
        GameObject hurt = _enemyNode.GetRef("Hurt").gameObject;
        hurt.GetComponent<Text>().text = "-" + damage;
        DelayAction da = hurt.GetComponent<DelayAction>();
        da.StartDelay();
    }

    void OnShowWin(object p1, object p2)
    {
        _rootNode.GetRef("Win").gameObject.SetActive(true);
    }

    void OnShowLose(object p1, object p2)
    {
        _rootNode.GetRef("Lose").gameObject.SetActive(true);
    }

    void OnCreateFightView(object p1, object p2)
    {
        //create enemy
        List<Enemy> enemies = (List<Enemy>)p2;
        foreach (Enemy e in enemies)
        {
            Image cg = _enemyNode.GetRef("Cg").GetComponent<Image>();
            cg.sprite = ResourceSys.Instance.GetSprite(e.CreatureData.Cg);
            Slider s = _enemyNode.GetRef("HpSlider").GetComponent<Slider>();
            s.value = e.CreatureData.HpPercent;
        }
        
        //create heros
        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p1;
        _heroNodes = new Dictionary<int, Transform>();
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            FightHero fh = pair.Value;

            GameObject go = Instantiate(_modelNode.GetNode("Hero").gameObject);
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

    void OnChangeHero(object p1, object p2)
    {
        //reset hero selected label
        FightHero fh = (FightHero)p1;
        Transform t = _heroNodes[fh.Id];
        UINode node = t.GetComponent<UINode>();
        node.GetRef("Selected").gameObject.SetActive(true);

        _itemNodeRoot.DestroyChildren();

        //reset items
        foreach (Item item in fh.Items)
        {
            GameObject go = Instantiate(_modelNode.GetNode("Item").gameObject);
            go.transform.SetParent(_itemNodeRoot);

            UINode itemNode = go.GetComponent<UINode>();
            Image image = itemNode.GetRef("Image").GetComponent<Image>();
            image.sprite = ResourceSys.Instance.GetSprite(item.Icon);
            Text num = itemNode.GetRef("Text").GetComponent<Text>();
            num.text = item.Count.ToString();
        }
        
    }

    void OnUpdateRound(object p1, object p2)
    {
        int round = (int)p1;//暂时没地方显示Round
        FightHero fightHero = (FightHero)p2;
        OnChangeHero( fightHero, null);
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
