using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lomztein.AdvDiscordCommands.Autodocumentation
{
    public class TypeDescriptor
    {
        private static readonly string[] SuperscriptNumbers = new string[] { "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹", "¹⁰", "¹¹", "¹²", "¹³", "¹⁴", "¹⁵" };
        private static readonly string ArrayIndicator = "[]";
        private static readonly string ArrayText = "A list of [ArrayElementPlaceholderType] values.";
        private static readonly string NestedRegex = "\\[(.*?)\\]";

        private readonly List<Type> _described = new List<Type>();
        private readonly bool _layman;

        public TypeDescriptor (bool layman)
        {
            _layman = layman;
        }

        public void Add(Type type) {
            if (!_described.Contains(type) && type != null)
            {
                _described.Add(type);
                AddNested(type);
            }
        }

        public void AddNested (Type type)
        {
            foreach (var nested in GetNested(GetDescription(type)))
            {
                Add(nested);
            }
        }

        private Type ElementType(Type type) => type.IsArray ? type.GetElementType() : type;

        public string GetDescription (Type type)
        {
            if (type == null)
            {
                return "Null Type";
            }

            Add(type);
            Type elementType = ElementType(type);
            return ReplaceNested (type.IsArray ? ArrayText.Replace ("[ArrayElementPlaceholderType]", $"[{elementType.FullName}]") : TypeDescriptions.GetDescription(elementType));
        }
        
        public string GetName (Type type, bool withSuperscript)
        {
            if (type == null)
            {
                return "Null Type";
            }

            Add(type);
            Type elementType = ElementType(type);
            int index = _described.IndexOf(type);
            string suffix = (type.IsArray ? ArrayIndicator : "") + (withSuperscript ? SuperscriptNumbers[index] : "");
            return TypeDescriptions.GetName(elementType, _layman) + suffix;
        }

        private IEnumerable<Type> GetNested (string input)
        {
            Regex regex = new Regex(NestedRegex);
            var matches = regex.Matches(input);
            return matches.Select(x => GetType(x.Value.Substring (1, x.Value.Length - 2), true, true)).ToList ();
        }

        private string ReplaceNested (string input)
        {
            string result = input;

            Regex regex = new Regex(NestedRegex);
            var matches = regex.Matches (input);
            var nested = GetNested(input);

            for (int i = 0; i < matches.Count; i++)
            {
                result = result.Replace(matches[i].Value, GetName(nested.ToArray ()[i], true));
            }

            return result;
        }

        public string[] GetDescriptions ()
        {
            List<string> descriptions = new List<string>();
            for (int i = 0; i < _described.Count; i++)
            {
                Type cur = _described[i];
                descriptions.Add($"**{i+1} - {GetName(cur, false)}**: {GetDescription(cur)}");
            }
            return descriptions.ToArray();
        }

        // Quickly stole this off StackOverflow lol.
        // Credits goes to here: https://stackoverflow.com/a/11811046
        private static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
        {
            var type = Type.GetType(typeName, false, ignoreCase);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName, false, ignoreCase);
                if (type != null)
                    return type;
            }

            if (throwOnError)
            {
                throw new InvalidOperationException($"Could not load type '{typeName}' from any currently loaded assembly.");
            }
            return null;
        }
    }
}
