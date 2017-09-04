﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Web;

public static class Config
{
    private static JObject _Items;
    private static bool noCache = true;

    private static JObject BuildItems() => 
        JObject.Parse(File.ReadAllText(HttpContext.Current.Server.MapPath("config.json")));

    public static int GetInt(string key) => 
        GetValue<int>(key);

    public static string GetString(string key) => 
        GetValue<string>(key);

    public static string[] GetStringList(string key) => 
        (from x in Items[key] select x.Value<string>()).ToArray<string>();

    public static T GetValue<T>(string key) => 
        Items[key].Value<T>();

    public static JObject Items
    {
        get
        {
            if (noCache || (_Items == null))
            {
                _Items = BuildItems();
            }
            return _Items;
        }
    }
}

