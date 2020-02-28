using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PlayRivals
{
    public static class PR_Extensions
    {
        public static string toPrettyDate(this DateTime d, bool bUTC = false)
        {
            TimeSpan s = DateTime.Now.Subtract(d);
            if (bUTC)
                s = DateTime.UtcNow.Subtract(d);

            int dayDiff = (int)s.TotalDays;
            int secDiff = (int)s.TotalSeconds;
            if (dayDiff < 0 || dayDiff >= 31)
                return d.ToShortDateString();

            if (dayDiff == 0)
            {
                if (secDiff < 60)
                    return "just now";

                if (secDiff < 120)
                    return "1 minute ago";

                if (secDiff < 3600)
                    return string.Format("{0} minutes ago", Math.Floor((double)secDiff / 60));

                if (secDiff < 7200)
                    return "1 hour ago";

                if (secDiff < 86400)
                    return string.Format("{0} hours ago", Math.Floor((double)secDiff / 3600));
            }
            if (dayDiff == 1)
                return "yesterday";

            if (dayDiff < 7)
                return string.Format("{0} days ago", dayDiff);

            if (dayDiff < 31)
                return string.Format("{0} weeks ago", Math.Ceiling((double)dayDiff / 7));

            return null;
        }

        public static string toHumanReadableDayTimeLeft(this DateTime obj)
        {
            TimeSpan timeSpan = obj.Subtract(DateTime.Now);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            return timeText;
        }

        public static string toHumanReadableTimeLeft(this DateTime obj)
        {
            TimeSpan timeSpan = obj.Subtract(DateTime.Now);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            return timeText;
        }

        public static string toHumanReadableTimeLeft(this int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds,
                            t.Milliseconds);

            return answer;
        }

        public static string toJson(this Hashtable obj)
        {
            return MiniJSONL.jsonEncode(obj);
        }


        public static string toJson(this Dictionary<string, string> obj)
        {
            return MiniJSONL.jsonEncode(obj);
        }

        public static string toJson(this IDictionary<string, object> obj)
        {
            return MiniJSONL.jsonEncode(obj);
        }

        public static string toJson(this ArrayList obj)
        {
            return MiniJSONL.jsonEncode(obj);
        }

        public static string toJson(this JArray obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static ArrayList arrayListFromJson(this string json)
        {
            return MiniJSONL.jsonDecode(json) as ArrayList;
        }


        public static ArrayList toArrayList(this string json)
        {
            return MiniJSONL.jsonDecode(json) as ArrayList;
        }

        public static JArray toJArrayList(this string json)
        {
            return JArray.Parse(json) as JArray;
        }

        public static List<T> toList<T>(this ArrayList arrayList)
        {
            return arrayList.Cast<T>().ToList();
        }

        public static List<T> ToGenericList<T>(this ArrayList arrayList)
        {
            List<T> list = new List<T>(arrayList.Count);
            foreach (T instance in arrayList)
            {
                list.Add(instance);
            }
            return list;
        }


        public static Hashtable toHashtable(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return new Hashtable();

            try
            {
                return MiniJSONL.jsonDecode(json) as Hashtable;
            }
            catch (Exception ex) { }

            return new Hashtable();
        }

        public static Hashtable toHashtable(this JObject data)
        {
            string strJSON = data.ToString();
            if (string.IsNullOrEmpty(strJSON))
                return new Hashtable();

            return MiniJSONL.jsonDecode(strJSON) as Hashtable;
        }

        public static JObject toJObject(this string json)
        {
            return JObject.Parse(json);
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this Hashtable table)
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            foreach (DictionaryEntry kvp in table)
                dict.Add((K)kvp.Key, (V)kvp.Value);
            return dict;
        }

#region JArray
        public static List<string> toList(this JArray arrayList)
        {
            List<string> newList = new List<string>();
            foreach (string obj in arrayList)
            {
                newList.Add(obj);
            }
            return newList;
        }
#endregion

        //public static Dictionary<string, object> toDictionary(this string json)
        //{
        //    Hashtable dataTable = MiniJSONL.jsonDecode(json) as Hashtable;
        //    return dataTable.ToDictionary<string, object>();
        //}

        public static string CutoutString(this string content, int length = 50)
        {
            if (content.Length > length)
                return content.Remove(50, content.Length - length);
            else
                return content;
        }

        public static string CutoutShortString(this string content)
        {
            if (content.Length > 18)
                return content.Remove(18, content.Length - 18);
            else
                return content;
        }

        public static string TrimFirstWhiteSpaces(this string origString)
        {
            char[] charArray = origString.ToCharArray();

            int index = 0;
            foreach (char c in charArray)
            {
                //Debug.Log(c);
                if (!char.IsWhiteSpace(c) && c != '\n' && c != '\r')
                    break;

                index++;
            }

            //Now remove string until that index
            string strNew = origString.Remove(0, index);
            //Debug.Log("Original : " + origString);
            //Debug.Log("New : " + strNew);
            return strNew;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}