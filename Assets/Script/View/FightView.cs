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
    UINode _winNode;
    UINode _loseNode;
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

    int _dragItemId;
    
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
        GameObject enemySupply = _enemyNode.GetRef("Supply").gameObject;
        enemySupply.SetActive(false);

        _modelNode = rootNode.GetNode("Models");

        _heroNodeRoot = rootNode.GetRef("HeroRoot");
        _itemNodeRoot = rootNode.GetRef("ItemsContent");

        _winNode = rootNode.GetNode("Win");
        _winNode.GetRef("Button").GetComponent<Button>().onClick.AddListener(OnWinConfirmed);
        _loseNode = rootNode.GetNode("Lose");
        _loseNode.GetRef("Button").GetComponent<Button>().onClick.AddListener(OnLoseConfirmed);

        ShowItemDes(string.Empty, null);

        EventSys.Instance.AddHander(ViewEvent.CreateFightView, OnCreateFightView);
        EventSys.Instance.AddHander(ViewEvent.SetSelectedHero, OnChangeHero);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateRound, OnUpdateRound);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateEnemyState, OnUpdateEnemy);
        EventSys.Instance.AddHander(ViewEvent.FightShowWin, OnShowWin);
        EventSys.Instance.AddHander(ViewEvent.FightHeroAttack, OnHeroAttack);
        EventSys.Instance.AddHander(ViewEvent.FightHeroHpSupply, OnHeroHpSupply);
        EventSys.Instance.AddHander(ViewEvent.FightHeroMpSupply, OnHeroMpSupply);
        EventSys.Instance.AddHander(ViewEvent.FightShowLose, OnShowLose);
        EventSys.Instance.AddHander(ViewEvent.FightWinReturnToStage, OnFinish);
        EventSys.Instance.AddHander(ViewEvent.FightChangeTurn, OnChangeTurn);
        EventSys.Instance.AddHander(ViewEvent.FightEnemyAttack, OnEnemyAttack);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateHeroState, OnUpdateHero);
        EventSys.Instance.AddHander(ViewEvent.FightUpdateAllHeroState, OnUpdateAllHero);
        EventSys.Instance.AddHander(ViewEvent.FightLoseReturnToStage, OnFinish);
        EventSys.Instance.AddHander(ViewEvent.FightShowTipNotSupportYet, ShowTipNotSupportYet);
        EventSys.Instance.AddHander(ViewEvent.FightShowItemDes, ShowItemDes);
    }

    void OnEnemyAttack(object p1, object p2)
    {
        FightHero hero = (FightHero)p1;
        int damage = (int)p2;

        UINode node = _heroNodes[hero.Id].GetComponent<UINode>();
        SetHeroData(hero, node);
        node.GetRef("Selected").gameObject.SetActive(false);
        ShowHurt(node, damage);

        if(hero.CreatureData.Hp.Value <= 0)
        {
            ImageGray imageGray = node.GetRef("Icon").gameObject.GetComponent<ImageGray>();
            if(imageGray == null)
            {
                imageGray = node.GetRef("Icon").gameObject.AddComponent<ImageGray>();
            }
            imageGray.Gray = true;
        }

        ShowItemDes(string.Empty, null);
    }

    void OnUpdateHero(object p1, object p2)
    {
        FightHero fh = (FightHero)p1;
        UINode node = _heroNodes[fh.Id].GetComponent<UINode>();
        SetHeroData(fh, node);

        if(node.GetRef("Selected").gameObject.activeSelf)
        {
            RefreshItem(fh);
        }
    }

    void OnUpdateAllHero(object p1, object p2)
    {
        Dictionary<int, FightHero> heros = (Dictionary<int, FightHero>)p1;
        foreach (KeyValuePair<int, FightHero> pair in heros)
        {
            OnUpdateHero(pair.Value, null);
        }
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
        ShowItemDes(string.Empty, null);
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

    void OnHeroHpSupply(object p1, object p2)
    {
        int heroId = (int)p1;
        int supply = (int)p2;
        ShowHpSupply(_heroNodes[heroId].GetComponent<UINode>(), supply);
    }

    void OnHeroMpSupply(object p1, object p2)
    {
        int heroId = (int)p1;
        int supply = (int)p2;
        ShowMpSupply(_heroNodes[heroId].GetComponent<UINode>(), supply);
    }

    void ShowHpSupply(UINode node, int sVal)
    {
        GameObject supply = node.GetRef("Supply").gameObject;
        if (sVal < 0)
        {
            supply.SetActive(false);
            return;
        }
        supply.GetComponent<Text>().text = "+" + sVal;
        DelayAction da = supply.GetComponent<DelayAction>();
        if (da == null)
        {
            da = supply.AddComponent<DelayAction>();
        }
        da.DelaySecond = 1;
        da.DAction = () => { supply.gameObject.SetActive(false); };
        da.StartDelay();
    }

    void ShowMpSupply(UINode node, int sVal)
    {
        GameObject supply = node.GetRef("MpSupply").gameObject;
        if (sVal < 0)
        {
            supply.SetActive(false);
            return;
        }
        supply.GetComponent<Text>().text = "+" + sVal;
        DelayAction da = supply.GetComponent<DelayAction>();
        if (da == null)
        {
            da = supply.AddComponent<DelayAction>();
        }
        da.DelaySecond = 1;
        da.DAction = () => { supply.gameObject.SetActive(false); };
        da.StartDelay();
    }

    void OnShowWin(object p1, object p2)
    {
        List<Item> items = (List<Item>)p1;
        Transform content = _winNode.GetRef("Content");
        content.DestroyChildren();
        foreach (Item item in items)
        {
            GameObject newNodeObj = Instantiate(_modelNode.GetNode("RewardItem").gameObject, content);

            UINode newNode = newNodeObj.GetComponent<UINode>();
            Image bg = newNode.GetRef("Bg").GetComponent<Image>();
            bg.sprite = ResourceSys.Instance.GetFrame(item.Lv.Value);
            Image icon = newNode.GetRef("Icon").GetComponent<Image>();
            icon.sprite = ResourceSys.Instance.GetSprite(item.Icon);
            Text itemName = newNode.GetRef("Name").GetComponent<Text>();
            itemName.text = item.Name;
            if (item.Count.Value > 1)
            {
                itemName.text = item.Name + " * " + item.Count.Value;
            }
            Text itemDes = newNode.GetRef("Des").GetComponent<Text>();
            itemDes.text = item.Description;

            newNodeObj.SetActive(true);
        }
        
        _winNode.gameObject.SetActive(true);
        
    }

    void OnWinConfirmed()
    {
        EventSys.Instance.AddEvent(InputEvent.FightWinConfirm);
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

        _loseNode.gameObject.SetActive(true);
        Text text = _loseNode.GetComponent<UINode>().GetRef("Info").GetComponent<Text>();
        text.text = sb.ToString();

    }

    void OnLoseConfirmed()
    {
        EventSys.Instance.AddEvent(InputEvent.FightLoseConfirm);
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

            Dropable dropable = cg.gameObject.GetComponent<Dropable>();
            if(dropable == null)
            {
                dropable = cg.gameObject.AddComponent<Dropable>();
            }
            dropable.ActionId = e.InstanceId;
            dropable.OnDroped = OnDropOnEnemy;
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
            SetHeroData(fh, node);
            node.GetRef("Hurt").gameObject.SetActive(false);
            node.GetRef("Supply").gameObject.SetActive(false);
            node.GetRef("MpSupply").gameObject.SetActive(false);
            node.GetRef("Selected").gameObject.SetActive(false);
            ShowHurt(node, -1);

            Dropable dropable = go.AddComponent<Dropable>();
            dropable.ActionId = fh.Id;
            dropable.OnDroped = OnDropOnHero;

            Button btn = node.GetRef("Icon").GetComponent<Image>().gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() => { EventSys.Instance.AddEvent(InputEvent.FightSelectHero, fh.Id); });

            _heroNodes.Add(fh.Id, go.transform);
        }

        _rootNode.gameObject.SetActive(true);

        _winNode.gameObject.SetActive(false);
        _loseNode.gameObject.SetActive(false);
    }

    void SetHeroData(FightHero fightHero, UINode node)
    {
        Image ico = node.GetRef("Icon").GetComponent<Image>();
        ico.sprite = ResourceSys.Instance.GetSprite(fightHero.CreatureData.Icon);
        Slider hs = node.GetRef("HpSlider").GetComponent<Slider>();
        hs.value = fightHero.CreatureData.HpPercent;
        Slider ms = node.GetRef("MpSlider").GetComponent<Slider>();
        ms.value = fightHero.CreatureData.MpPercent;
    }

    void OnChangeHero(object p1, object p2)
    {
        //reset hero selected label
        FightHero fh = (FightHero)p1;
        foreach (KeyValuePair<int, Transform> pair in _heroNodes)
        {
            Transform t = pair.Value;
            UINode node = t.GetComponent<UINode>();
            node.GetRef("Selected").gameObject.SetActive(fh.Id == pair.Key);    
        }

        //reset items
        RefreshItem(fh);
    }

    void RefreshItem(FightHero fh)
    {
        _itemNodeRoot.DestroyChildren();
        foreach (var pair in fh.Items)
        {
            Item item = pair.Value;
            GameObject go = Instantiate(_modelNode.GetNode("Item").gameObject);
            go.transform.SetParent(_itemNodeRoot);

            UINode itemNode = go.GetComponent<UINode>();
            Image image = itemNode.GetRef("Image").GetComponent<Image>();
            image.sprite = ResourceSys.Instance.GetSprite(item.Icon);
            Button btn = image.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() => { EventSys.Instance.AddEvent(InputEvent.FightItemClicked, item.Id); });
            Text num = itemNode.GetRef("Text").GetComponent<Text>();
            num.text = item.Count.Value.ToString();
            num.raycastTarget = false;//避免遮挡
            if (item.UsableInFight)
            {
                if(item.JobLimited > 0 && item.JobLimited != ((HeroData)fh.CreatureData).Job )
                {
                    continue;
                }

                Dragable dragable = image.gameObject.AddComponent<Dragable>();
                dragable.HasTail = true;
                dragable.TailSprite = ResourceSys.Instance.GetSprite(GameConstants.CommonDragTail);
                dragable.TailColor = item.TheColor;
                dragable.TailWidth = 20;
                dragable.Canv = _rootNode.GetComponent<Canvas>();
                dragable.ActionId = item.Id;
                dragable.OnDragStart = OnDragFromItem;
            }
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

    /// <summary>
    /// 拖拽在敌人身上
    /// </summary>
    /// <param name="enemyInstanceId"></param>
    void OnDropOnEnemy(int enemyInstanceId)
    {
        EventSys.Instance.AddEvent(InputEvent.FightUseItemToEnemy, enemyInstanceId, _dragItemId);
    }

    /// <summary>
    /// 开始拖拽道具
    /// </summary>
    /// <param name="itemId"></param>
    void OnDragFromItem(int itemId)
    {
        _dragItemId = itemId;
    }

    /// <summary>
    /// 拖拽到英雄身上
    /// </summary>
    /// <param name="heroId"></param>
    void OnDropOnHero(int heroId)
    {
        EventSys.Instance.AddEvent(InputEvent.FightUseItemToHero, heroId, _dragItemId);
    }

    void ShowTipNotSupportYet(object p1, object p2)
    {
        Debug.Log("Not Support Yet");
    }

    void ShowItemDes(object p1, object p2)
    {
        string des = (string)p1;
        _rootNode.GetRef("ItemDes").GetComponent<Text>().text = des;
    }
}
