using UnityEngine;
using UnityEngine.UI;

public class ThrowItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] prefabItem;
    [SerializeField] GameObject[] fxItem;
    [SerializeField] Button btnClose;

    [SerializeField] bool[] curveThrow;
    GameType _gameRunning = GameType.TexasPoker;

    int targetParasite;

    [SerializeField] Text txtName, txtMoney, txtPlayed, txtWinRate, txtLevel;
    [SerializeField] Image icGender, icCurrency;

    private void Start()
    {
        btnClose.onClick.AddListener(Hide);
    }

    public void Show()
    {
        gameObject.SetActive (true);
    }

    public void SetupMenuPoker(Transform _to, int _target)
    {
        _gameRunning = GameType.TexasPoker;
        Show();

        transform.position = _to.position;
        targetParasite = _target;

        _PlayerPokerActor targetActor = _PokerGameManager.instance.stockParasite[_target].uiPlayerPoker;
        ExitGames.Client.Photon.Hashtable playerProperties = targetActor.myPlayer.CustomProperties;

        txtName.text = targetActor._myName;
        txtMoney.text = targetActor.txtMyMoney.text;
        icCurrency.sprite = targetActor.imgBetAmount[0].sprite;
        //icGender.sprite = (int)playerProperties[PhotonEnums.Player.Gender] == 1 ? HomeSceneManager.Instance.iconMale : HomeSceneManager.Instance.iconFemale;

        txtWinRate.text = (int)playerProperties[PhotonEnums.Player.WinRate] +"%";
        txtPlayed.text = (int)playerProperties[PhotonEnums.Player.TotalPlayed] + "";
        txtLevel.text = (int)playerProperties[PhotonEnums.Player.LevelChar] + "";


    }

    public void Hide()
    {
        gameObject.SetActive (false);
    }

    public void OnClickItem(int idx)
    {
        if(_gameRunning == GameType.TexasPoker)
            _PokerGameManager.instance.unsortedPlayers[0]._myParasitePlayer.SendThrowItem(idx,targetParasite);

        Hide();
    }

    public void ThrowTheItemPoker(int idxItem, int from, int to)
    {
        GameObject o = Instantiate(prefabItem[idxItem]);

        o.transform.position = _PokerGameManager.instance.stockParasite[from].uiPlayerPoker.postThrowTarget.position;
        Vector3 target = _PokerGameManager.instance.stockParasite[to].uiPlayerPoker.postThrowTarget.position;

        o.transform.LookAt(target);

        if (curveThrow[idxItem])
        {
            Vector3 middleVector = new Vector3(target.x - o.transform.position.x, 50f, target.z - o.transform.position.z);

            Vector3 startAngle = o.transform.position - (middleVector * 0.25f);
            Vector3 endAngle = new Vector3(target.x + (middleVector.x * 0.25f), target.y - (middleVector.y * 0.25f), target.z + (middleVector.z * 0.25f));

            LeanTween.moveSpline(o, new Vector3[] { startAngle, o.transform.position, target, endAngle }, 0.8f).setEaseInQuad().setOnComplete(() => { CompleteThrow(o, _PokerGameManager.instance.stockParasite[to].uiPlayerPoker.hero, idxItem); });
        }
        else
            LeanTween.move(o, target, 0.8f).setOnComplete(() => { CompleteThrow(o, _PokerGameManager.instance.stockParasite[to].uiPlayerPoker.hero, idxItem); });
    }

    void CompleteThrow(GameObject o, Hero parentTarget, int idxItem)
    {
        Destroy(o);

        GameObject fx = Instantiate(fxItem[idxItem]);

        fx.transform.SetParent(parentTarget.transform);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localEulerAngles = new Vector3(0, 180f, 0);

        //parentTarget.PlayAvaterExpressionHit(idxItem);

        Destroy(fx, 2f);
    }
}