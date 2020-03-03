using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    public Button btnPoker;

    private void Start ()
    {
        btnPoker.onClick.AddListener (OnPoker);
    }

    public void OnPoker()
    {
        _SceneManager.instance.SetActivePoker ();
    }
}
