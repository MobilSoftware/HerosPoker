using UnityEngine;

public class FxTableSpotlite : MonoBehaviour
{
    public GameObject[] objTableLites;

    public void ShowFxSpotlight (int indexPlayer )      //indexPlayer = 0 -> myPlayer
    {
        DeactiveSpot();

        if(indexPlayer < objTableLites.Length)
            objTableLites[indexPlayer].SetActive(true);
    }

    public void DeactiveSpot()
    {
        for (int i = 0; i < objTableLites.Length; i++)
            objTableLites[i].SetActive(false);
    }
}
