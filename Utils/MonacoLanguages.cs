namespace DACS2.Utils
{
    public static class MonacoLanguages
    {
        public static readonly Dictionary<string, string> LanguageMap = new()
        {
            { "C#", "csharp" },
            { "Python", "python" },
            { "Javascript", "javascript" },
            { "Java", "java" },
            { "HTML", "html" },
            { "CSS", "css" }
        };

        public static string GetMonacoLanguage(string ngonNgu)
        {
            return LanguageMap.TryGetValue(ngonNgu, out var lang) ? lang : "plaintext";
        }
    }
}
