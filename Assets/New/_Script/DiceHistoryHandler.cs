using UnityEngine;
using UnityEngine.UI;

public class DiceHistoryHandler : MonoBehaviour
{
    public GameObject objFrame;
    public Image imgDice1;
    public Image imgDice2;
    public Image imgDice3;
    public Sprite[] sprFaces;

    private int indexDice1;
    private int indexDice2;
    private int indexDice3;

    public void SetHistory (int index1, int index2, int index3 )
    {
        imgDice1.sprite = sprFaces[index1 - 1];
        imgDice2.sprite = sprFaces[index2 - 1];
        imgDice3.sprite = sprFaces[index3 - 1];
        objFrame.SetActive (true);
        imgDice1.gameObject.SetActive (true);
        imgDice2.gameObject.SetActive (true);
        imgDice3.gameObject.SetActive (true);
    }
}
