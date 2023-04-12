using System.Collections.Generic;

namespace Localization
{
    public class LangFunc
    {
        public static string GetText(string format, params object?[] arg)
        {
            return string.Format(format, arg);
        }
        public static string GetText(DataName dataName, params object?[] arg)
        {
            if (!Library.DefaultText.ContainsKey(dataName))
                return "";
            return string.Format(Library.DefaultText[dataName], arg);
        }
        public static string GetText(LangType langType, DataName dataName, params object?[] arg)
        {
            var dataLang = new Dictionary<DataName, string>();
            
            switch (langType)
            {
                case LangType.Ru:
                    dataLang = LibraryRu.RuText;
                    break;
            }
            
            if (!dataLang.ContainsKey(dataName))
                return "";
            
            return string.Format(dataLang[dataName], arg);
        }
    }
}