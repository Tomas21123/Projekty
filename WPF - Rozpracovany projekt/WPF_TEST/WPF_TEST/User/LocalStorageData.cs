using System;
using System.IO;
using System.Text.Json;

namespace WPF_TEST.User
{
    public class LocalStorageData
    {
        public string LastEmail { get; set; } = string.Empty;
        public int LastUserId { get; set; } = 0;
        public string SavedPasswordEncrypted { get; set; } = string.Empty;
    }

    public static class LocalStorage
    {
        private static readonly string folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WPF_TEST");

        private static readonly string filePath = Path.Combine(folderPath, "user_settings.json");

        private static LocalStorageData data = new LocalStorageData();

        static LocalStorage()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    data = JsonSerializer.Deserialize<LocalStorageData>(json) ?? new LocalStorageData();
                }
            }
            catch
            {
                data = new LocalStorageData();
            }
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch { /* ignorujeme chyby zápisu */ }
        }

        public static string LastEmail
        {
            get => data.LastEmail;
            set { data.LastEmail = value ?? string.Empty; Save(); }
        }

        public static int LastUserId
        {
            get => data.LastUserId;
            set { data.LastUserId = value; Save(); }
        }

        public static string SavedPasswordEncrypted
        {
            get => data.SavedPasswordEncrypted;
            set { data.SavedPasswordEncrypted = value ?? string.Empty; Save(); }
        }
    }
}
