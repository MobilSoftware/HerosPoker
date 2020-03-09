using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class SlotoManagerScript : MonoBehaviour
{
    public ReelScript[] reels;
    public IconScript[] slotIcons;
    public Sprite[] iconSprite;
    public GameObject objSloto;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Encrypt("myValueThatIWantToEncrypt"));
        Debug.Log(Encrypt("887957008303212"));
    }

    public void Show ()
    {
        objSloto.SetActive (true);
        Init ();
    }

    public void Hide ()
    {
        objSloto.SetActive (false);
    }

    private void Init()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Init(iconSprite);
        }
    }

    public void OnMouseUp(ButtonScript.ButtonType type)
    {
        if(type == ButtonScript.ButtonType.Spin)
        {
            StartCoroutine(StartSpin());
        } else if (type == ButtonScript.ButtonType.Stop)
        {
            StartCoroutine(StopSpin());
        }
    }

    IEnumerator StartSpin()
    {
        if (reels[0].spin == 0)
        {
            for (int i = 0; i < slotIcons.Length; i++)
            {
                slotIcons[i].fixedValue = false;
            }
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StartSpin();
                yield return new WaitForSeconds(0.1f);
            }
            //StartCoroutine(HitAPI());
        }
    }

    IEnumerator StopSpin()
    {
        if (reels[0].spin == 2)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].StopSpin();
                yield return new WaitForSeconds(0.1f * Random.Range(1, 11));
            }
        }
    }

    IEnumerator HitAPI()
    {
        int[] tempValue = new int[16];
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            tempValue = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        }
        else if (rand == 1)
        {
            tempValue = new int[] { 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 };
        }
        else if (rand == 2)
        {
            tempValue = new int[] { 2, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4, 0, 1, 2, 3, 4 };
        }
        yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
        for (int i = 0; i < slotIcons.Length; i++)
        {
            slotIcons[i].SetIconValue(tempValue[i], iconSprite[tempValue[i] * reels[0].maxIconBlur + 0]);
            slotIcons[i].fixedValue = true;
        }
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].spin = 3;
        }
    }

    static readonly string PasswordHash = "yourkey1";
    static readonly string SaltKey = "yourkey2";
    static readonly string VIKey = "yourkey3yourkey3";

    public string Encrypt(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
        var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

        byte[] cipherTextBytes;

        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memoryStream.ToArray();
                cryptoStream.Close();
            }
            memoryStream.Close();
        }
        return System.Convert.ToBase64String(cipherTextBytes);
    }

    public string Decrypt(string encryptedText)
    {
        byte[] cipherTextBytes = System.Convert.FromBase64String(encryptedText);
        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

        var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
    }
}
