using System.Globalization;
using System.Text.RegularExpressions;

namespace FftWrap.Codegen
{
    public static class Utils
    {
        public static string ConvertMethodNames(Method origin)
        {
            string coreName = Regex.Match(origin.Name, @"X[(](?<core>\w+)[)]").Groups["core"].ToString();

            coreName = coreName.Replace('_', ' ');
            coreName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(coreName);
            coreName = coreName.Replace(" ", "");

            return coreName;
        }

        public static string ConvertTypeNames(Method origin)
        {
            if (origin.ReturnTypeIsPointer)
                return "IntPtr";

            if (origin.ReturnType == @"X(plan)")
                return "IntPtr";

            return origin.ReturnType;
        }


        public static string ConvertTypeNames(Parameter origin)
        {
            if (origin.IsPointer)
                return "IntPtr";

            if (origin.Type == @"X(plan)")
                return "IntPtr";

            return origin.Type;
        }
    }
}
