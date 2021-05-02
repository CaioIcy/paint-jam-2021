using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlienWealth
{
    Poor,
    Mid,
    Rich,
}

public class AlienController : MonoBehaviour
{
    public SpriteRenderer Renderer;

    [SerializeField] float _moveDuration = 1.5f;
    bool _moving = false;

    public UIGameplay _ui;
    public Inventory Inventory;

    Alien _alien;
    AlienWealth _wealth;


    void OnValidate()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
        _ui = FindObjectOfType<UIGameplay>(true);
        Inventory = FindObjectOfType<Inventory>(true);
    }

    public void Setup(Alien alien)
    {
        _alien = alien;
        Renderer.sprite = alien.Sprite;

        var rand = new System.Random();
        _wealth = (AlienWealth)rand.Next(0, 3);
    }


    float _targetX;
    public void EnterScene()
    {
        Renderer.flipX = false;
        UIPopupItem.CanInteract = false;
        _lerpStartTime = Time.time;
        _prevPos = transform.localPosition.x;
        _targetX = 0f;
        _moving = true;

        _cbStopMoving = () =>
        {
            UIPopupItem.CanInteract = true;
            _ui.ShowAlienText(_alien.Messages[0]);
            AudioManager.ins.Play(AudioManager.ins.Aliens);
        };

        FindObjectOfType<Roacher>(true).MaybeSpawn();
    }

    Action _cbStopMoving = null;
    float _prevPos = 0.0f;
    float _lerpStartTime;

    public void ExitScene()
    {
        UIPopupItem.CanInteract = false;
        Renderer.flipX = true;
        _prevPos = transform.localPosition.x;
        _targetX = -15f;
        _lerpStartTime = Time.time;
        _moving = true;
        _cbStopMoving = () =>
        {
            UIPopupItem.CanInteract = true;
        };
    }

    private void Update()
    {
        if(_moving)
        {
            var since = Time.time - _lerpStartTime;
            var pct = since / _moveDuration;

            var pos = transform.localPosition;
            pos.x = Mathf.Lerp(_prevPos, _targetX, Easing.Cubic.InOut(pct));
            if (Mathf.Abs(pos.x-_targetX) < 1f/1e2f)
            {
                _moving = false;
                pos.x = _targetX;
                _cbStopMoving?.Invoke();
            }
            transform.localPosition = pos;
        }
    }

    string _itemID;
    public void Barter(string itemID)
    {
        _itemID = itemID;
        StartCoroutine(BarterProc(itemID));
    }

    static readonly List<string> MsgsNo = new List<string>
    {
        "Thanks, but no thanks.",
        "I'll pass.",
        "That's preposterous.",
        "I don't have that much money!",
        "Who are you trying to swindle?",
    };
    string MsgNo()
    {
        return MsgsNo[UnityEngine.Random.Range(0, MsgsNo.Count)];
    }

    static readonly List<string> MsgsYes = new List<string>
    {
        "That'll do. Thank you very much.",
        "I'll take it.",
        "Seems fine to me!",
        "This was a good deal.",
        "Wow, nice. I've always wanted this!",
    };
    string MsgYes()
    {
        return MsgsYes[UnityEngine.Random.Range(0, MsgsYes.Count)];
    }

    static readonly List<string> MsgsOffer = new List<string>
    {
        "Hmmm... how about... <money>?",
        "What do you think of this price? <money>.",
        "I'll buy it at a high price!",
        "I only have about this much. I think. Maybe. <money>.",
        "Would you give it up for this much? <money>.",
    };
    string MsgOffer(int want)
    {
        return MsgsOffer[UnityEngine.Random.Range(0, MsgsOffer.Count)].Replace("<money>", want.ToString());
    }

    float TimeForText(string text)
    {
        return (text.Length * 0.025f) + 2f;
    }

    IEnumerator BarterProc(string itemID)
    {
        UIPopupItem.CanInteract = false;
        var interest = 0;
        foreach (var itr in _alien.Interests)
        {
            if (itr.ItemID == itemID)
            {
                interest = itr.Interest;
            }
        }
        if(_alien.Behaviour == Alien.BvAll)
        {
            var rand = new System.Random();
            interest = rand.Next(200, 1000);
        }

        if (interest == 0)
        {
            Debug.Log($"REJECT BARTER [{interest}]");

            var msg = MsgNo();
            _ui.ShowAlienText(msg);
            AudioManager.ins.Play(AudioManager.ins.Incorrect);
            yield return new WaitForSeconds(TimeForText(msg));
            ExitScene();
            _ui.HideAlienText();
            yield return new WaitForSeconds(2f);
            God.ins.CheckNext();
        }
        else
        {
            _ui.ShowAlienText("...");
            yield return new WaitForSeconds(1f);
            var want = InterestByWealth(interest);
            var msg = MsgOffer(want);
            _ui.ShowAlienText(msg);
            yield return new WaitForSeconds(TimeForText(msg));
            FindObjectOfType<UIPopupBarter>(true).Show(want);
            _want = want;
        }
    }

    int _want;

    int InterestByWealth(int interest)
    {
        var mult = UnityEngine.Random.Range(0.94f, 1.06f);
        interest = Mathf.CeilToInt(interest * mult);
        switch (_wealth)
        {
            case AlienWealth.Poor:
                interest /= 10;
                break;
            case AlienWealth.Rich:
                interest = Mathf.CeilToInt(interest * UnityEngine.Random.Range(1.45f, 1.6f));
                break;
        }
        return interest;
    }

    public void Offer(int amount)
    {
        Debug.Log($"OFFER={amount}");
        StartCoroutine(OfferProc(amount));
    }

    static readonly Dictionary<AlienWealth, int> WealthPotential = new Dictionary<AlienWealth, int>
    {
        { AlienWealth.Poor, 5 },
        { AlienWealth.Mid, 50 },
        { AlienWealth.Rich, 500 },
    };

    public int _fullPotential = 0;
    public int _fullActual = 0;

    IEnumerator OfferProc(int amount)
    {
        var rand = new System.Random();
        var amountOfRet = rand.Next(1, 3);
        while(amountOfRet-- > 0)
        {
            _ui.ShowAlienText("...");
            yield return new WaitForSeconds(1f);
        }

        var rejected = false;

        var msg = "";
        if(_alien.Behaviour == Alien.BvNone)
        {
            Debug.Log($"REJECT offer NONE [{amount}]");
            msg = MsgNo();
            _ui.ShowAlienText(msg);
            AudioManager.ins.Play(AudioManager.ins.Incorrect);
            rejected = true;
        }
        else {
            var okWant = false;
            if (amount > _want) {
                if(amount - _want <= WealthPotential[_wealth])
                {
                    okWant = true;
                }
            }
            else
            {
                // sure, offered less lol
                okWant = true;
            }

            if (okWant)
            {
                msg = MsgYes();
                _ui.ShowAlienText(msg);
                AudioManager.ins.Play(AudioManager.ins.Correct);
                Inventory.DeltaMoney(amount);
                Inventory.RemoveItem(_itemID);
            }
            else
            {
                Debug.Log($"REJECT offer !OKwANT [{amount}]");
                rejected = true;
                AudioManager.ins.Play(AudioManager.ins.Incorrect);
                msg = MsgNo();
                _ui.ShowAlienText(msg);
            }
        }

        var potential = WealthPotential[_wealth];
        var actual = rejected ? 0 : potential - ((_want + potential) - amount);

        _fullPotential += potential;
        _fullActual += actual;

        //Debug.Log($"wanted=[{_want}], offered=[{amount}]");
        //Debug.Log($"potential=[{potential}], actual=[{actual}]");

        yield return new WaitForSeconds(TimeForText(msg));
        ExitScene();
        _ui.HideAlienText();
        yield return new WaitForSeconds(2f);
        God.ins.CheckNext();
    }
}
