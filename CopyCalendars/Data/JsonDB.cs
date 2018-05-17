using System;
using System.IO;
using Newtonsoft.Json;
namespace CopyCalendars.Data
{
    public class JsonDB<T>
    {
        private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CopyCalendars");
        private static string _filePath = Path.Combine(_path, "db.json");
        public JsonDB()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public void Write(T model)
        {
            string json = JsonConvert.SerializeObject(model);
            File.WriteAllText(_filePath, json);
        }
        public T Read()
        {
            if (!File.Exists(_filePath))
            {
                return default(T);
            }

            string json = File.ReadAllText(_filePath);

            if (String.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
