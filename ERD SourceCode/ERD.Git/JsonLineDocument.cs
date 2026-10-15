using System.Text.Json;
using System.Text.Json.Nodes;

namespace ERD.Git
{
    internal class JsonLineDocument
    {
        public List<string> Lines { get; } = new();

        public Dictionary<string, (int Start, int End)> NodeRanges { get; } = new();

        public static JsonLineDocument Create(JsonNode? node)
        {
            JsonLineDocument document = new();

            document.WriteNode(node, "$", 0, string.Empty, true);

            return document;
        }

        private void WriteNode(JsonNode? node, string path, int indent, string prefix, bool isLast)
        {
            int start = Lines.Count;
            string indentation = new(' ', indent);
            string comma = isLast ? string.Empty : ",";

            if (node is JsonObject obj)
            {
                if (obj.Count == 0)
                {
                    Lines.Add($"{indentation}{prefix}{{}}{comma}");
                }
                else
                {
                    Lines.Add($"{indentation}{prefix}{{");

                    int index = 0;

                    foreach (KeyValuePair<string, JsonNode?> property in obj)
                    {
                        string childPath = GetPropertyPath(path, property.Key);

                        WriteNode(property.Value, childPath, indent + 2, $"{JsonSerializer.Serialize(property.Key)}: ", ++index == obj.Count);
                    }

                    Lines.Add($"{indentation}}}{comma}");
                }
            }
            else if (node is JsonArray array)
            {
                if (array.Count == 0)
                {
                    Lines.Add($"{indentation}{prefix}[]{comma}");
                }
                else
                {
                    Lines.Add($"{indentation}{prefix}[");

                    for (int index = 0; index < array.Count; index++)
                    {
                        WriteNode(array[index], $"{path}[{index}]", indent + 2, string.Empty, index == array.Count - 1);
                    }

                    Lines.Add($"{indentation}]{comma}");
                }
            }
            else
            {
                Lines.Add($"{indentation}{prefix}{node?.ToJsonString() ?? "null"}{comma}");
            }

            NodeRanges[path] = (start, Lines.Count - 1);
        }

        private static string GetPropertyPath(string parentPath, string propertyName)
        {
            // Matches the normal $.PropertyName paths used in your JSON.
            if (propertyName.Length > 0
                && (char.IsLetter(propertyName[0]) || propertyName[0] == '_')
                && propertyName.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                return $"{parentPath}.{propertyName}";
            }

            // Bracket notation for property names containing spaces or punctuation.
            return $"{parentPath}[{JsonSerializer.Serialize(propertyName)}]";
        }
    }
}
