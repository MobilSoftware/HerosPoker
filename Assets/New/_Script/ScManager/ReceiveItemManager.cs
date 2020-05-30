using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveItemManager : MonoBehaviour
{
    private static ReceiveItemManager s_Instance = null;
    public static ReceiveItemManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (ReceiveItemManager)) as ReceiveItemManager;
                if (s_Instance == null)
                    Debug.Log ("Could not locate an ReceiveItemManager object. \n You have to have exactly one ReceiveItemManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Canvas canvas;
    public Transform parentItems;
    public ItemReceive prefabItemReceive;
    public Button btnOK;

    private bool isInit;
    private SceneType prevSceneType;

    private void Start ()
    {
        btnOK.onClick.AddListener (Hide);
    }

    public void Show (ItemReceiveData[] items)
    {
        if (items.Length == 0)
            return;

        if (!isInit)
        {
            isInit = true;
            canvas.sortingOrder = (int) SceneType.RECEIVE_ITEM;
        }

        for (int i = 0; i < items.Length; i++)
        {
            ItemReceive ir = Instantiate (prefabItemReceive, parentItems);
            ir.SetData (items[i]);
        }

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.RECEIVE_ITEM;
    }

    public void Hide()
    {
        _SceneManager.instance.activeSceneType = prevSceneType;
        canvas.enabled = false;
        int childCount = parentItems.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy (parentItems.GetChild (i).gameObject);
        }
    }
}
