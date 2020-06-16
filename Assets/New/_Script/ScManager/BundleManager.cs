using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum DownloadType
{
    THUMB,
    ASSET
}

public class BundleManager : MonoBehaviour
{
    private static BundleManager s_Instance = null;

    public static BundleManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType (typeof (BundleManager)) as BundleManager;
                if (s_Instance == null)
                    Logger.D ("Could not locate an BundleManager object. \n You have to have exactly one BundleManager in the scene.");
            }
            return s_Instance;
        }
    }

    public Text txtVersion;
    public Text txtStatusBar;
    public Image fillProgressBar;
    [HideInInspector]
    public bool bLoadingScenes;
    private JGetVersion jGetVersion;
    private string dlSceneFolder = "scene";
    private string dlSceneName = "scene.unity3d";
    private string dlThumbFolder = "thumb";
    private string dlThumbName = "image";
    private string dlAssetFolder = "asset";
    private string dlAssetName = "bundle.unity3d";
    private int downloadCount;
    private int downloadedCount;


    public void ProcessGetVersion (JGetVersion json )
    {
        jGetVersion = json;
        string[] versionLatest = json.version.new_ver.Split ('.');
        string[] versionRequired = json.version.old_ver.Split ('.');
        string[] versionClient = Application.version.Split ('.');
        txtVersion.text = "v" + Application.version;

        int bugFixLatest = Convert.ToInt32 (versionLatest[2]);
        int bugFixRequired = Convert.ToInt32 (versionRequired[2]);
        int bugFixClient = Convert.ToInt32 (versionClient[2]);

        if (bugFixClient < bugFixRequired)  //must update
            MessageManager.instance.Show (this.gameObject, "Mohon perbarui aplikasi", ButtonMode.OK, -2, "Perbarui");
        else if (bugFixClient > bugFixRequired && bugFixClient < bugFixLatest)  //optional update
            MessageManager.instance.Show (this.gameObject, "Versi terbaru ditemukan, apakah anda ingin memperbarui aplikasi?", ButtonMode.OK_CANCEL, -3, "Perbarui", "Tidak");
        else
            Download ();
    }

    private void Download ()
    {
        Logger.E ("downloading");
        DownloadScene ();
        DownloadItem ();
        StartCoroutine (_Loading ());
    }

    private string GetSceneDownloadPath (int _id, int _version )
    {
        string dlPath = dlSceneFolder + "/" + _id + "/" + _version;
        dlPath = Path.Combine (Application.persistentDataPath, dlPath);     //ex: persistent/dlSceneFolder/1/2
        return dlPath;
    }

    private void DownloadScene ()
    {
        JScene[] scenes = jGetVersion.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            JScene scene = scenes[i];
            string scenePath = GetSceneDownloadPath (scene.scene_id, scene.asset_version);
            string[] split = scene.asset_url.Split ('/');
            string scenePathWithFilename = Path.Combine (scenePath, dlSceneName);      //ex: persistent/dlSceneFolder/1/2/sehome.unity3d
            if (scene.asset_url.Length > 0 && !File.Exists (scenePathWithFilename))
            {
                downloadCount++;
                StartCoroutine (_DownloadScene (scenePath, scene.asset_url));
                if (scene.asset_version > 0)
                {
                    for (int x = 0; x < scene.asset_version; x++)
                    {
                        string previousVersionPath = GetSceneDownloadPath (scene.scene_id, x);
                        if (Directory.Exists (previousVersionPath))
                            Directory.Delete (previousVersionPath, true);
                    }
                }
            }
        }
    }

    IEnumerator _DownloadScene (string _path, string _url )
    {
        UnityWebRequest www = UnityWebRequest.Get (_url);
        yield return www.SendWebRequest ();
        string[] split = _url.Split ('/');
        string path = Path.Combine (_path, dlSceneName);
        DownloadHandler handler = www.downloadHandler;
        if (www.isNetworkError || www.isHttpError)
            Logger.E (www.error);
        else
        {
            try
            {
                Directory.CreateDirectory (_path);
                File.WriteAllBytes (path, handler.data);
            }
            catch (Exception e)
            {
                Logger.E ("Error: " + e.Message);
            }
        }
        downloadedCount++;
    }

    private string GetItemDownloadPath (DownloadType dt, int _type, int _id, int _version )
    {
        string dlPath = string.Empty;
        string folder = string.Empty;
        if (dt == DownloadType.THUMB)
            folder = dlThumbFolder;
        else if (dt == DownloadType.ASSET)
            folder = dlAssetFolder;
        else
            Logger.E ("download type unknown");

        dlPath = folder + "/" + _type + "/" + _id + "/" + _version;
        dlPath = Path.Combine (Application.persistentDataPath, dlPath);
        return dlPath;
    }

    private void DownloadItem ()
    {
        JItem[] items = jGetVersion.items;

        for (int i = 0; i < items.Length; i++)
        {
            JItem item = items[i];
            string thumbPath = GetItemDownloadPath (DownloadType.THUMB, item.item_type_id, item.item_id, item.asset_version);
            string thumbPathWithFilename = thumbPath + "/image";
            string assetPath = GetItemDownloadPath (DownloadType.ASSET, item.item_type_id, item.item_id, item.asset_version);
            //Logger.E (assetPath);
            string assetPathWithFilename = assetPath + "/bundle.unity3d";

            if (item.thumb_url.Length > 0 && !File.Exists (thumbPathWithFilename))
            {
                downloadCount++;
                StartCoroutine (_DownloadThumb (thumbPath, item.thumb_url));

                if (item.asset_version > 0)
                {
                    for (int x = 0; x < item.asset_version; x++)
                    {
                        string previousVersionPath = GetItemDownloadPath (DownloadType.THUMB, item.item_type_id, item.item_id, x);
                        if (Directory.Exists (previousVersionPath))
                            Directory.Delete (previousVersionPath, true);
                    }
                }
            }
            if (item.asset_url.Length > 0 && !File.Exists (assetPathWithFilename))
            {
                downloadCount++;
                StartCoroutine (_DownloadAsset (assetPath, item.asset_url));

                if (item.asset_version > 0)
                {
                    for (int x = 0; x < item.asset_version; x++)
                    {
                        string previousVersionPath = GetItemDownloadPath (DownloadType.ASSET, item.item_type_id, item.item_id, x);
                        if (Directory.Exists (previousVersionPath))
                            Directory.Delete (previousVersionPath, true);
                    }
                }
            }
        }
    }

    IEnumerator _DownloadThumb ( string _path, string _url )
    {
        UnityWebRequest www = UnityWebRequest.Get (_url);
        yield return www.SendWebRequest ();
        string path = Path.Combine (_path, dlThumbName);
        DownloadHandler handler = www.downloadHandler;
        if (www.isNetworkError || www.isHttpError)
        {
            Logger.E (_url);
            Logger.E (www.error);
        }
        else
        {
            try
            {
                Directory.CreateDirectory (_path);
                File.WriteAllBytes (path, handler.data);
                //Logger.E (_path);
            }
            catch (Exception e)
            {
                Logger.E ("Error: " + e.Message);
            }
        }
        downloadedCount++;
    }

    IEnumerator _DownloadAsset ( string _path, string _url )
    {
        UnityWebRequest www = UnityWebRequest.Get (_url);
        yield return www.SendWebRequest ();
        string path = Path.Combine (_path, dlAssetName);
        DownloadHandler handler = www.downloadHandler;
        if (www.isNetworkError || www.isHttpError)
            Logger.E (www.error);
        else
        {
            try
            {
                Directory.CreateDirectory (_path);
                File.WriteAllBytes (path, handler.data);
                //Logger.E ("saved path: " + _path);
            }
            catch (Exception e)
            {
                Logger.E ("Error: " + e.Message);
            }
        }
        downloadedCount++;
    }

    IEnumerator _Loading ()
    {
        while (downloadedCount != downloadCount && downloadCount > 1)
        {
            float fDownloaded = downloadedCount;
            float fDownload = downloadCount;
            float percentage = (fDownloaded / fDownload) * 100f;
            percentage = (float) Math.Floor (percentage);
            //txtStatusBar.text = "Mengunduh aset: " + percentage + "%";
            txtStatusBar.text = "Mengunduh aset: " + fDownloaded + " / " + fDownload;
            fillProgressBar.fillAmount = percentage / 100f;
            //progressBar.SetAmount (fDownloaded / fDownload);
            yield return null;
        }

        //progressBar.SetAmount (0f);

        bLoadingScenes = false;
        int counter = 1;
        fillProgressBar.fillAmount = 1;
        txtStatusBar.text = "Memuat permainan .";
        _SceneManager.instance.LoadAllScenes ();
        while (!bLoadingScenes)
        {
            txtStatusBar.text += " .";
            counter++;
            if (counter > 3)
            {
                txtStatusBar.text = "Memuat permainan .";
                counter = 1;
            }
            yield return _WFSUtility.wfs03;
        }

        Logger.E ("loading done");

        //SceneManager.UnloadSceneAsync ("SeSplash");
    }

    public string GetSceneLoadPath (int _id )
    {
        string loadPath = string.Empty;
        List<int> version = new List<int> ();
        string halfLoadPath = dlSceneFolder + "/" + _id;
        loadPath = Path.Combine (Application.persistentDataPath, halfLoadPath);
        if (Directory.Exists (loadPath))
        {
            //load from persistent data path
            string[] subDirectories = Directory.GetDirectories (loadPath);
            for (int i = 0; i < subDirectories.Length; i++)
            {
                version.Add (Convert.ToInt32 (subDirectories[i].Remove (0, loadPath.Length + 1)));
            }
            version.Sort ();
            loadPath = loadPath + "/" + version[version.Count - 1] + "/" + "scene.unity3d";     //ex: /Users/michael/Library/Application Support/Myplay/PokerHeroes/scene/2/1/scene.unity3d
        }
        else
        {
            //load from streaming asset
            string filename = string.Empty;
            if (_id == 1)
                filename = "shared";
            else if (_id == (int) SceneType.MESSAGE)
            {
                Logger.E ("id = SceneType.MESSAGE");
                return filename;
            }
            else
                filename = "se" + ((SceneType) _id).ToString ().ToLower();

            loadPath = Path.Combine (Application.streamingAssetsPath, filename);    //ex: /Users/michael/Unity Projects/PokerHeroes/Assets/StreamingAssets/sebegin
        }

        return loadPath;
    }

    public string GetItemLoadPath (DownloadType _dt, int _typeID, int _id )
    {
        string loadPath = string.Empty;
        string folderName = string.Empty;
        string itemName = string.Empty;
        if (_dt == DownloadType.THUMB)
        {
            folderName = dlThumbFolder;
            itemName = dlThumbName;
        } else if (_dt == DownloadType.ASSET)
        {
            folderName = dlAssetFolder;
            itemName = dlAssetName;
        }

        List<int> version = new List<int> ();
        string halfLoadPath = folderName + "/" + _typeID + "/" + _id;
        loadPath = Path.Combine (Application.persistentDataPath, halfLoadPath);
        if (Directory.Exists (loadPath))
        {
            string[] subDirectories = Directory.GetDirectories (loadPath);
            for (int i = 0; i < subDirectories.Length; i++)
            {
                version.Add (Convert.ToInt32 (subDirectories[i].Remove (0, loadPath.Length + 1)));
            }
            version.Sort ();
            loadPath = loadPath + "/" + version[version.Count - 1] + "/" + itemName;
        }

        return loadPath;
    }

    public void LoadImage ( RawImage _rawImage, string _path )
    {
        StartCoroutine (_LoadImage (_rawImage, _path));
    }

    private IEnumerator _LoadImage ( RawImage imgIcon, string _path )
    {
        WWW www = new WWW ("file:///" + _path);
        yield return www;
        imgIcon.texture = www.texture;
    }

    public void OnPositiveClicked (int returnCode )
    {
        switch (returnCode)
        {
            case -2:
            case -3: Logger.E ("Going to Playstore"); break;
        }
    }

    public void OnNegativeClicked (int returnCode )
    {
        switch (returnCode)
        {
            case -3:
                MessageManager.instance.Hide ();
                Download ();
                break;
        }
    }
}
