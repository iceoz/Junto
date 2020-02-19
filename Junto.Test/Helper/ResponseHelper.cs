using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Junto.Test.Helper
{
    public static class ResponseHelper
    {
        public static T Deserialize<T>(this ObjectResult result)
        {
            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<T>(json);
            return data;
        }

        public static List<T> ToList<T>(this ObjectResult result)
        {
            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<List<T>>(json);
            return data;
        }

        public static Dictionary<string, string> ToDictionary(this ObjectResult result)
        {
            var json = JsonSerializer.Serialize(result.Value);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return data;
        }
    }
}
