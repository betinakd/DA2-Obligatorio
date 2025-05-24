using System;
using System.Text;
using System.Web;
using Transformers.Abstractions;

namespace HTMLExporter
{
    public class HtmlResponseTransformer : IResponseTransformer
    {
        public string Id => "html-transformer";
        public string Name => "HTML";
        public int DisplayOrder => 10;
        public string ContentType => "text/html";

        public string Transform(string executionResult)
        {
            if (string.IsNullOrEmpty(executionResult))
            {
                return "<div class='execution-empty'>No hay resultados para mostrar</div>";
            }

            StringBuilder html = new StringBuilder();

            html.Append(@"
            <style>
                .execution-container {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    background-color: #f8f9fa;
                    border-radius: 6px;
                    border: 1px solid #dee2e6;
                    padding: 15px;
                    margin: 10px 0;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
                }
                .execution-title {
                    font-size: 18px;
                    color: #343a40;
                    margin-bottom: 15px;
                    padding-bottom: 10px;
                    border-bottom: 1px solid #dee2e6;
                }
                .execution-line {
                    padding: 5px 0;
                    margin: 2px 0;
                    line-height: 1.5;
                    display: flex;
                    align-items: baseline;
                }
                .execution-line:hover {
                    background-color: #f1f3f5;
                    border-radius: 4px;
                }
                .execution-indent {
                    color: #adb5bd;
                    margin-right: 8px;
                    font-family: monospace;
                    user-select: none;
                }
                .execution-method {
                    color: #0366d6;
                    font-weight: bold;
                }
                .execution-arrow {
                    color: #6c757d;
                    margin: 0 8px;
                    font-weight: bold;
                }
                .execution-result {
                    color: #28a745;
                }
                .execution-info {
                    color: #495057;
                    font-style: italic;
                }
                .execution-nested {
                    margin-left: 20px;
                    padding-left: 10px;
                    border-left: 2px solid #dee2e6;
                }
            </style>
            ");

            html.Append("<div class='execution-container'>");
            html.Append("<div class='execution-title'>Resultado de Ejecución</div>");

            int previousIndent = 0;
            bool insideNestedBlock = false;

            string[] lines = executionResult.Split('\n');
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                int indentCount = 0;
                while (indentCount < line.Length && line[indentCount] == ' ')
                {
                    indentCount++;
                }

                if (indentCount > previousIndent)
                {
                    html.Append("<div class='execution-nested'>");
                    insideNestedBlock = true;
                }
                else if (indentCount < previousIndent && insideNestedBlock)
                {
                    int levelsToClose = (previousIndent - indentCount) / 4;
                    for (int i = 0; i < levelsToClose; i++)
                    {
                        html.Append("</div>");
                    }

                    if (indentCount == 0)
                    {
                        insideNestedBlock = false;
                    }
                }

                previousIndent = indentCount;

                html.Append("<div class='execution-line'>");

                if (indentCount > 0)
                {
                    html.Append($"<span class='execution-indent'>{new string('·', indentCount)}</span>");
                }

                string content = line.Trim();

                if (content.Contains(" -> "))
                {
                    var parts = content.Split(new[] { " -> " }, StringSplitOptions.None);
                    html.Append($"<span class='execution-method'>{HttpUtility.HtmlEncode(parts[0])}</span>");
                    html.Append("<span class='execution-arrow'>→</span>");
                    html.Append($"<span class='execution-result'>{HttpUtility.HtmlEncode(parts[1])}</span>");
                }
                else
                {
                    html.Append($"<span class='execution-info'>{HttpUtility.HtmlEncode(content)}</span>");
                }

                html.Append("</div>");
            }

            if (insideNestedBlock)
            {
                int levelsToClose = previousIndent / 4;
                for (int i = 0; i < levelsToClose; i++)
                {
                    html.Append("</div>");
                }
            }

            html.Append("</div>");

            return html.ToString();
        }
    }
}