using System.Text;

namespace MoonCommonLib
{
    public class MSharedStringBuilder
    {
        private static readonly StringBuilder builder = new StringBuilder();

        public static StringBuilder Get()
        {
            builder.Clear();
            return builder;
        }
    }
}