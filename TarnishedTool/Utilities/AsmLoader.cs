using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TarnishedTool.Enums;

namespace TarnishedTool.Utilities
{
    public static class AsmLoader
    {
        private const string BytePattern = @"^(?:[\da-f]{2} )*(?:[\da-f]{2}(?=\s|$))";
        
        private static readonly Dictionary<AsmScript, byte[]> Cache = new();

        public static byte[] GetAsmBytes(AsmScript resourceName)
        {
            if (!Cache.TryGetValue(resourceName, out byte[] template))
            {
                template = ParseBytes(GetResourceContent(resourceName.ToString()));
                Cache[resourceName] = template;
            }
            return (byte[])template.Clone();
        }

        private static string GetResourceContent(string resourceName)
        {
            object resource = Properties.Resources.ResourceManager.GetObject(resourceName);
            return resource as string ??
                   throw new ArgumentException($"Resource '{resourceName}' not found or is not a string.");
        }

        private static byte[] ParseBytes(string asmFile)
        {
            return Regex.Matches(asmFile, BytePattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Cast<Match>()
                .SelectMany(m => m.Value.Split(' '))
                .Select(hex => Convert.ToByte(hex, 16))
                .ToArray();
        }
    }
}