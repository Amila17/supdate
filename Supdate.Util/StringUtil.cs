using System;
using System.Security.Cryptography;
using System.Text;

namespace Supdate.Util
{
  public class StringUtil
  {
    public static string Pluralise(string singular, string plural, int val)
    {
      return (val > 0) ? plural : singular;
    }
    public static byte[] GetHash(string inputString)
    {
      HashAlgorithm algorithm = MD5.Create();  //or use SHA1.Create();
      return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
      StringBuilder sb = new StringBuilder();
      foreach (byte b in GetHash(inputString))
        sb.Append(b.ToString("X2"));

      return sb.ToString();
    }

    public static string Ordinal(int number)
    {
      string suffix;

      var ones = number % 10;
      var tens = (int)Math.Floor(number / 10M) % 10;

      if (tens == 1)
      {
        suffix = "th";
      }
      else
      {
        switch (ones)
        {
          case 1:
            suffix = "st";
            break;

          case 2:
            suffix = "nd";
            break;

          case 3:
            suffix = "rd";
            break;

          default:
            suffix = "th";
            break;
        }
      }
      return string.Format("{0}{1}", number, suffix);
    }
  }
}
