using System;
using System.IO;
using System.Reflection;

namespace Typefout.Core.Data.Services
{
    public static class EnvService
    {
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string? resourceName = assembly.GetManifestResourceNames()
                 .FirstOrDefault(n => n.EndsWith(".env", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                System.Diagnostics.Debug.WriteLine("Fout: .env resource niet gevonden. Beschikbare resources:");
                foreach (string name in assembly.GetManifestResourceNames())
                {
                    System.Diagnostics.Debug.WriteLine($"Beschikbaar: {name}");
                }
                return;
            }

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Fout: Resource '{resourceName}' kon niet geopend worden.");
                    return;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        string[] parts = line.Split('=', 2);
                        if (parts.Length != 2) continue;

                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
        }

        public static string? Get(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
