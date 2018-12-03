using System.Text;

namespace MFrameWork
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