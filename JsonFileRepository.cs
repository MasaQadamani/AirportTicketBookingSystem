using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AirportTicketBookingSystem
{
    internal class JsonFileRepository<T>
    {
        private readonly string filePath;

        public JsonFileRepository(string filePath)
        {
            this.filePath = filePath;
        }

        public List<T> Load()
        {
            if (!File.Exists(filePath))
                return new List<T>();

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public void Save(List<T> items)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(items, options);
            File.WriteAllText(filePath, json);
        }
    }
}