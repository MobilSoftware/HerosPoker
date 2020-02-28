using System.Collections.Generic;
using UnityEngine;

public class ChipsDrop : MonoBehaviour
{
    public Animator myAnimator;

    public void PlayDropAnimation ()//Play Chips Drop, change setup Mesh dulu
    {
        gameObject.SetActive (true);
        myAnimator.Play ("ChipsDrop");
    }
}
