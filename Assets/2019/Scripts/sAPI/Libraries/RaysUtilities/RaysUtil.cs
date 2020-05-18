using System;
using System.Linq;

namespace Rays.Utilities
{
    public static class Util
    {
        public const string ALPHANUMERIC = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RandomChar ( int charlen, string charset = ALPHANUMERIC )
        {
            string output = "";
            while (charlen-- > 0)
            {
                output += charset[UnityEngine.Random.Range (0, charset.Length)];
            }
            return output;
        }

        public static void Print ( string log, string prefix = "Log = " )
        {
            UnityEngine.Debug.Log (prefix + log);
        }

        public static void Print ( int log, string prefix = "Log = " )
        {
            Print (log.ToString (), prefix);
        }

        public static void Print ( string[] log, string prefix = "Log = " )
        {
            Print (String.Join (",", log), prefix);
        }

        public static void Print ( int[] log, string prefix = "Log = " )
        {
            Print (String.Join (",", new System.Collections.Generic.List<int> (log).ConvertAll (i => i.ToString ()).ToArray ()), prefix);
        }

        public static string PlayerID2SearchTag ( int id )
        {
            return (id + 4096).ToString ("X");
        }

        public static int SearchTag2PlayerID ( string id )
        {
            return Convert.ToInt32 (id, 16) - 4096;
        }
    }
}