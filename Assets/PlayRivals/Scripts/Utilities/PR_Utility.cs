#define JSONDotNET

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//License Type : MIT
//Copyright (C) <2013> <JoyDash Pte Ltd, Singapore>
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;


#if PARSE
using Parse;
#endif

#if JSONDotNET
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
#endif

namespace PlayRivals
{
    public class SortBySlotIndexHelper : IComparer<PhotonPlayer>
    {
        public int Compare(PhotonPlayer x, PhotonPlayer y)
        {
            PhotonPlayer playerX = x;
            PhotonPlayer playerY = y;
            //Sort by descending scores order
            int scoreX = PhotonUtility.GetPlayerProperties<int>(playerX, PhotonEnums.Player.SlotIndex);
            int scoreY = PhotonUtility.GetPlayerProperties<int>(playerY, PhotonEnums.Player.SlotIndex);
            return (scoreX).CompareTo(scoreY);
        }
    }

    public class UrlShortener
    {
        private static String ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static int BASE = 62;

        public static String Encode(int num)
        {
            StringBuilder sb = new StringBuilder();

            while (num > 0)
            {
                sb.Append(ALPHABET[(num % BASE)]);
                num /= BASE;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                builder.Append(sb[i]);
            }
            return builder.ToString();
        }

        public static int Decode(String str)
        {
            int num = 0;

            for (int i = 0, len = str.Length; i < len; i++)
            {
                num = num * BASE + ALPHABET.IndexOf(str[(i)]);
            }

            return num;
        }
    }

    public class PR_Utility : MonoBehaviour
    {
        #region Singleton
        // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
        private static PR_Utility s_Instance = null;

        // This defines a static instance property that attempts to find the manager object in the scene and
        // returns it to the caller.
        public static PR_Utility instance
        {
            get
            {
                if (s_Instance == null)
                {
                    // This is where the magic happens.
                    //  FindObjectOfType(...) returns the first PR_Utility object in the scene.
                    s_Instance = FindObjectOfType(typeof(PR_Utility)) as PR_Utility;
                    if (s_Instance == null)
                        Debug.Log("Could not locate an PR_Utility object. \n You have to have exactly one PR_Utility in the scene.");
                }
                return s_Instance;
            }
        }

        // Ensure that the instance is destroyed when the game is stopped in the editor.
        void OnApplicationQuit()
        {
            s_Instance = null;
        }
        #endregion
        
        /// <summary>
        /// Creating Date Time
        /// </summary>
        /// <param name="d"></param>
        /// <param name="bUTC"></param>
        /// <returns></returns>
        public static string GetPrettyDate(DateTime d, bool bUTC = true)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(d);

            if (bUTC)
                s = DateTime.UtcNow.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return d.ToShortDateString();
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes ago",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago",
                Math.Ceiling((double)dayDiff / 7));
            }
            return null;
        }

        //Using Json.et
        #region JSON.NET
        public static string ParseString(JObject data, string name, bool bCleanLine = false)
        {
            if (data != null && data[name] != null)
            {
                string strFinal = data[name].ToString();//.Replace("&#x0040;", "@");
                if (bCleanLine)
                    strFinal = strFinal.Replace("\"", "").Replace("[", "").Replace("]", "");

                return strFinal;
            }

            return "";
        }

        public static JArray ParseArray(JObject data, string name, int defaultNum = 0)
        {
            if (data != null && data[name] != null)
            {
                if (data[name].GetType() == typeof(JArray))
                    return (JArray)data[name];
                else
                    return new JArray();

            }
            return new JArray();
        }
        #endregion

        public static string ParseString(Hashtable data, string name)
        {
            if (data != null && data[name] != null)
            {
                if (data[name].GetType() != typeof(string))
                {
                    Debug.Log("Invalid type string and parse with : " + data[name].GetType());
                    return "";
                }

                //print((data[name].GetType()));
                string strFinal = ((String)data[name]).Replace("&#x0040;", "@");
                return strFinal;
            }

            return "";
        }

        public static int ParseInt(Hashtable data, string name)
        {
            int val = 0;
            if (data != null && data[name] != null)
            {
                if (int.TryParse(data[name].ToString(), out val))
                    return int.Parse(data[name].ToString());
            }

            return 0;
        }

        public static double ParseDouble(Hashtable data, string name)
        {
            double val = 0.0;
            if (data != null && data[name] != null)
            {
                if (double.TryParse(data[name].ToString(), out val))
                    return double.Parse(data[name].ToString());
            }

            return 0.0;
        }

        public static DateTime ParseDate(string strDateTime)
        {
            DateTime dateVal;
            if (DateTime.TryParse(strDateTime, out dateVal))
                return DateTime.Parse(strDateTime);

            return DateTime.UtcNow;
        }

        public static DateTime ParseDateFromString(string name)
        {
            DateTime dateVal;
            if (DateTime.TryParse(name, out dateVal))
                return DateTime.Parse(name);

            return DateTime.Now;
        }

        public static DateTime ParseDateFromTick(long tick)
        {
            return new DateTime(tick);
        }

        public static string GetTimeFromTick(long tick)
        {
            int minutes = (int)(tick / 60f);
            int hours = (int)(minutes / 60f);
            int seconds = (int)(tick % 60f);

            return string.Format("{0} : {1} : {2} sec(s)", Mathf.Abs(hours), Mathf.Abs(minutes), Mathf.Abs(seconds));
        }

        public static bool IsValidEmailAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            else
            {
                var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                return regex.IsMatch(s) && !s.EndsWith(".");
            }
        }
    }
}