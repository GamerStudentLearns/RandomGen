using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class PlayerPrefsKeys
{
    public static List<string> GetAllKeys()
    {
        var keys = new List<string>();

        var type = typeof(PlayerPrefs);
        var field = type.GetField("s_PlayerPrefs", BindingFlags.NonPublic | BindingFlags.Static);

        if (field != null)
        {
            var dict = field.GetValue(null) as System.Collections.IDictionary;
            if (dict != null)
            {
                foreach (var key in dict.Keys)
                    keys.Add(key.ToString());
            }
        }

        return keys;
    }
}
