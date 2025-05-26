using System;
using System.Collections.Generic;
using System.Text.Json;
using Transformers.Abstractions;

namespace JSONExporter
{
    public class JsonResponseTransformer : IResponseTransformer
    {
        public string Id => "json-transformer";
        public string Name => "JSON";
        public int DisplayOrder => 20;
        public string ContentType => "application/json";

        public string Transform(string executionResult)
        {
            if (string.IsNullOrEmpty(executionResult))
            {
                return "{}";
            }

            try
            {
                var lines = executionResult.Split('\n');
                var result = new List<ExecutionStep>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    int indentLevel = 0;
                    while (indentLevel < line.Length && line[indentLevel] == ' ')
                    {
                        indentLevel++;
                    }

                    string content = line.Trim();

                    if (content.Contains(" -> "))
                    {
                        var parts = content.Split(new[] { " -> " }, StringSplitOptions.None);
                        result.Add(new ExecutionStep
                        {
                            Depth = indentLevel / 4,
                            Source = parts[0],
                            Target = parts[1],
                            Type = "method"
                        });
                    }
                    else
                    {
                        result.Add(new ExecutionStep
                        {
                            Depth = indentLevel / 4,
                            Description = content,
                            Type = "info"
                        });
                    }
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Serialize(new { steps = result }, options);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = ex.Message, originalResult = executionResult });
            }
        }

        private sealed class ExecutionStep
        {
            public int Depth { get; set; }
            public string? Source { get; set; }
            public string? Target { get; set; }
            public string? Description { get; set; }
            public string Type { get; set; } = "info"; 
        }
    }
}