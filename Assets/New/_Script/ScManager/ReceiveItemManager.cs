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
    public Transform trFrame;
    public Transform parentItems;
    public ItemReceive prefabItemReceive;
    public Button btnOK;

    [HideInInspector]
    public List<ItemReceiveData> itemsData;

    private bool isInit;
    private SceneType prevSceneType;

    private void Start ()
    {
        btnOK.onClick.AddListener (Hide);
        itemsData = new List<ItemReceiveData> ();
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
        trFrame.localScale = Vector3.zero;

        canvas.enabled = true;
        prevSceneType = _SceneManager.instance.activeSceneType;
        _SceneManager.instance.activeSceneType = SceneType.RECEIVE_ITEM;
        trFrame.LeanScale (Vector3.one, _SceneManager.TWEEN_DURATION);
    }

    public void Hide()
    {
        trFrame.LeanScale (Vector3.zero, _SceneManager.TWEEN_DURATION).setOnComplete
        (() =>
        {
            _SceneManager.instance.activeSceneType = prevSceneType;
            canvas.enabled = false;
            int childCount = parentItems.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy (parentItems.GetChild (i).gameObject);
            }
        });
    }

    public void ShowCombined ()
    {
        if (itemsData.Count == 0)
            return;

        Show (itemsData.ToArray ());
        itemsData.Clear ();
    }
}
