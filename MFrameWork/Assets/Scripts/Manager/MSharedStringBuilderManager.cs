using System.Text;

namespace MFrameWork
{
    //全局StringBuilder
    public class MSharedStringBuilderManager
    {
        private static readonly StringBuilder builder = new StringBuilder();

        public static StringBuilder Get()
        {
            builder.Clear();
            return builder;
        }
    }
}