using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    Transform _attBtn;
    Transform _skillBtn;
    Transform _petBtn;

    /// <summary>
    /// 在界面上显示的英雄
    /// {Key:heroId, Value:nodeTrans}
    /// </summary>
    Dictionary<int, Transform> _heroNodes;
    
    public override void InitUI(UINode rootNode)
    {
        base.InitUI(rootNode);

        _attBtn = rootNode.GetRef("AttackBtn");
        _attBtn.GetComponent<Button>().onClick.AddListener(OnAttackBtnClicked);
        _skillBtn = rootNode.GetRef("SkillBtn");
        _skillBtn.GetComponent<Button>().onClick.AddListener(OnSkillBtnClicked);
        _petBtn = rootNode.GetRef("PetBtn");
        _petBtn.GetComponent<Button>().onClick.AddListener(OnPetBtnClicked);

        _rootNode = rootNode;
        _rootNode.gameObject.SetActive(false);

        _enemyNode = rootNode.GetNode("Enemy");
        GameObject enemyHurt = _enemyNode.GetRef("Hurt").gameObject;
        
        enemyHurt.SetActive(false);

        _modelNode = rootNode.GetNode("Models");

        _heroNodeRoot = rootNode.GetRef("HeroRoot");
        _itemNodeRoot = rootNode.GetRef("ItemsContent");

        
        
        EventSys.Instance.AddHander(ViewEvent.CreateFightView, OnCreateFightView);
        EventSys.Instance.AddHander(ViewEvent.SetSelectedHero, OnChangeHero);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateRound, OnUpdateRound);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateEnemyState, OnUpdateEnemy);
        EventSys.Instance.AddHander(ViewEvent.FightShowWin, OnShowWin);
        EventSys.Instance.AddHander(ViewEvent.FightHeroAttack, OnHeroAttack);
        EventSys.Instance.AddHander(ViewEvent.FightShowLose, OnShowLose);
        EventSys.Instance.AddHander(ViewEvent.FightWinReturnToStage, OnFinish);
        EventSys.Instance.AddHander(ViewEvent.FightChangeTurn, OnChangeTurn);
        EventSys.Instance.AddHander(ViewEvent.FightEnemyAttack, OnEnemyAttack);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateHeroState, OnUpdateHero);
        EventSys.Instance.AddHander(ViewEvent.FightLoseReturnToStage, OnFinish);
        
    }

    void OnEnemyAttack(object p1, object p2)
    {
        FightHero hero = (FightHero)p1;
        int damage = (int)p2;

        UINode node = _heroNodes[hero.Id].GetComponent<UINode>();
        SetHeroData(hero, node, false);
        ShowHurt(node, damage);
    }

    void OnUpdateHero(object p1, object p2)
    {
        Debug.LogError("Not Support Yet");
    }

    void OnFinish(object p1, object p2)
    {
        _rootNode.gameObject.SetActive(false);
        //clear
        _heroNodeRoot.DestroyChildren();
        _itemNodeRoot.DestroyChildren();
    }

    void OnUpdateEnemy(object p1, object p2)
    {
        Enemy e = (Enemy)p1;
        Slider s = _enemyNode.GetRef("HpSlider").GetComponent<Slider>();
        s.value = e.CreatureData.HpPercent;
    }

    void OnHeroAttack(object p1, object p2)
    {
        int damage = (int)p1;
        ShowHurt(_enemyNode, damage);
    }

    void ShowHurt(UINode node, int damage)
    {
        GameObject hurt = node.GetRef("Hurt").gameObject;
        if (damage < 0)
        {
            hurt.SetActive(false);
            return;
        }
        hurt.GetComponent<Text>().text = "-" + damage;
        DelayAction da = hurt.GetComponent<DelayAction>();
        if(da == null)
        {
            da = hurt.AddComponent<DelayAction>();
        }
        da.DelaySecond = 1;
        da.DAction = () => { hurt.gameObject.SetActive(false); };
        da.StartDelay();
    }

    void OnShowWin(object p1, object p2)
    {
        _rootNode.GetRef("Win").gameObject.SetActive(true);
    }

    void OnShowLose(object p1, object p2)
    {
        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p1;
        StringBuilder sb = new StringBuilder();
        foreach (var pair in heros)
        {
            sb.Append(pair.Value.CreatureData.Name).Append(",");
        }
        sb.Append("被打败了");

        UINode node = _rootNode.GetNode("Lose");
        node.gameObject.SetActive(true);
        Text text = node.GetComponent<UINode>().GetRef("Text").GetComponent<Text>();
        text.text = sb.ToString();

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
            SetHeroData(fh, node, false);
            ShowHurt(node, -1);

            Dropable dropable = go.AddComponent<Dropable>();
            dropable.ActionId = fh.Id;

            _heroNodes.Add(fh.Id, go.transform);
        }

        _rootNode.gameObject.SetActive(true);

        _rootNode.GetRef("Win").gameObject.SetActive(false);
        _rootNode.GetNode("Lose").gameObject.SetActive(false);
    }

    void SetHeroData(FightHero fightHero, UINode node, bool isSelected)
    {
        Image ico = node.GetRef("Icon").GetComponent<Image>();
        ico.sprite = ResourceSys.Instance.GetSprite(fightHero.CreatureData.Icon);
        Slider hs = node.GetRef("HpSlider").GetComponent<Slider>();
        hs.value = fightHero.CreatureData.HpPercent;
        Slider ms = node.GetRef("MpSlider").GetComponent<Slider>();
        ms.value = fightHero.CreatureData.MpPercent;
        node.GetRef("Selected").gameObject.SetActive(isSelected);
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

    void OnChangeTurn(object p1, object p2)
    {
        bool isHero = (bool)p1;
        
        _attBtn.gameObject.SetActive(isHero);
        _skillBtn.gameObject.SetActive(isHero);
        _petBtn.gameObject.SetActive(isHero);
        _itemNodeRoot.gameObject.SetActive(isHero);

        if(!isHero)
        {
            foreach (KeyValuePair<int, Transform> pair in _heroNodes)
            {
                UINode node = pair.Value.GetComponent<UINode>();
                node.GetRef("Selected").gameObject.SetActive(false);
            }
        }
    }

}
