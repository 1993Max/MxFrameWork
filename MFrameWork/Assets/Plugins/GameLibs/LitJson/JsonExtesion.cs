using System.IO;

namespace LitJson
{
    public static class JsonExtesion
    {
        public static void SaveObject<T>(string path, T obj, bool isPrettyPrint = true)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (TextWriter tw = new StreamWriter(fs))
                {
                    JsonMapper.ToJson(obj, new JsonWriter(tw){PrettyPrint = isPrettyPrint });
                }
            }
        }
    }
}