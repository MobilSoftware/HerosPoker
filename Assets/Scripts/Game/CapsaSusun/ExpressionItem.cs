using UnityEngine;

public class ExpressionItem : MonoBehaviour
{
    public int id = 0;
    [SerializeField] UnityEngine.UI.Image imgExpression;
    [SerializeField] UnityEngine.UI.Button btn;

    public void SetupItem(int id, Sprite _img)
    {
        this.id = id;
        imgExpression.sprite = _img;

        btn.onClick.AddListener(onClick);
    }

    private void onClick()
    {

    }
}
