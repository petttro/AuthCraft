using System;
using Newtonsoft.Json;

namespace AuthCraft.Common.Extensions;

public static class Serialization
{
    public static string JsonSerialize(this object input, bool omitNulls = false)
    {
        return JsonConvert.SerializeObject(
            input,
            Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = omitNulls ? NullValueHandling.Ignore : NullValueHandling.Include
            });
    }

    public static object JsonDeserialize(this string input, Type objectType)
    {
        return JsonConvert.DeserializeObject(input, objectType);
    }

    public static T JsonDeserialize<T>(this string input)
    {
        return JsonConvert.DeserializeObject<T>(input);
    }

    public static string SerializeJsonSafe(this object obj, Action<Exception> exFunc = null)
    {
        if (obj == null)
        {
            return null;
        }

        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch (Exception ex)
        {
            exFunc?.Invoke(ex);
            return null;
        }
    }

    public static T DeserializeJsonSafe<T>(this string obj, Action<Exception> exFunc = null)
    {
        if (obj == null)
        {
            return default(T);
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }
        catch (Exception ex)
        {
            exFunc?.Invoke(ex);
            return default(T);
        }
    }
}
