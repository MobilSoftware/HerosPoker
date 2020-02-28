using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Rays.Utilities
{
    public static class Digest
    {
        public const string PHRASE = "88A4A072948F6FB6DD6997A04C240CFE";
        public const string SALT = "57D056ED0984166336B7879C2AF3657F";

        public static string Write ( string input, string salt = SALT, string phrase = PHRASE )
        {
            byte[] key, iv;
            DeriveKeyAndIV (Encoding.UTF8.GetBytes (phrase), Encoding.UTF8.GetBytes (salt), out key, out iv);
            //Debug.Log(Encoding.UTF8.GetString(key) + " = " + Encoding.UTF8.GetString(iv));
            return Convert.ToBase64String (WriteStringToBytes (input, key, iv));
        }

        public static string Read ( string inputBase64, string salt = SALT, string phrase = PHRASE )
        {
            byte[] key, iv;
            DeriveKeyAndIV (Encoding.UTF8.GetBytes (phrase), Encoding.UTF8.GetBytes (salt), out key, out iv);
            return ReadStringFromBytes (Convert.FromBase64String (inputBase64), key, iv);
        }

        private static void DeriveKeyAndIV ( byte[] data, byte[] salt, out byte[] key, out byte[] iv )
        {
            MD5 hash = new MD5CryptoServiceProvider ();
            key = new byte[32];
            key = hash.ComputeHash (data);
            StringBuilder sb = new StringBuilder ();
            for (int i = 0; i < key.Length; i++) sb.Append (key[i].ToString ("x2"));
            key = Encoding.UTF8.GetBytes (sb.ToString ());

            sb = new StringBuilder ();
            byte[] tempsalt = new byte[32];
            tempsalt = hash.ComputeHash (salt);
            for (int i = 0; i < tempsalt.Length; i++) sb.Append (tempsalt[i].ToString ("x2"));
            tempsalt = Encoding.UTF8.GetBytes (sb.ToString ());
            iv = new byte[16];
            for (int i = 0; i < tempsalt.Length; i++) if (i % 2 == 0) iv[i / 2] = tempsalt[i];
        }

        static byte[] WriteStringToBytes ( string plainText, byte[] Key, byte[] IV )
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException ("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException ("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException ("IV");
            byte[] encrypted;

            // Create an RijndaelManaged object with the specified key and IV. 
            using (RijndaelManaged cipher = new RijndaelManaged ())
            {
                cipher.Key = Key;
                cipher.IV = IV;
                ICryptoTransform encryptor = cipher.CreateEncryptor (cipher.Key, cipher.IV);

                using (MemoryStream msEncrypt = new MemoryStream ())
                {
                    using (CryptoStream csEncrypt = new CryptoStream (msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter (csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write (plainText);
                        }
                        encrypted = msEncrypt.ToArray ();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }

        static string ReadStringFromBytes ( byte[] cipherText, byte[] Key, byte[] IV )
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException ("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException ("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException ("IV");

            string plaintext = null;
            // Create an RijndaelManaged object with the specified key and IV. 
            using (var cipher = new RijndaelManaged ())
            {
                cipher.Key = Key;
                cipher.IV = IV;
                ICryptoTransform decryptor = cipher.CreateDecryptor (cipher.Key, cipher.IV);

                using (MemoryStream msDecrypt = new MemoryStream (cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream (msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader (csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string.
                            plaintext = srDecrypt.ReadToEnd ();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}