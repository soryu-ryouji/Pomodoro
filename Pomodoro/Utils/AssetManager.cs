using System.IO;
using System.Text;

namespace Pomodoro.Utils;

public class AssetManager
{
    private static readonly string ConfigFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Pomodoro"
    );

    private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "pomodoro.config");

    public static List<(string name, object value)> LoadConfig()
    {
        if (!File.Exists(ConfigFilePath))
        {
            SaveConfig(
            [
                ("Work Time", "25"),
                ("Break Time", "5")
            ]);
        }

        var configLines = File.ReadLines(ConfigFilePath);
        var config = configLines.Select(ParserConfigLine);

        return config.ToList();
    }

    private static (string name, object value) ParserConfigLine(string text)
    {
        var index = text.IndexOf('=');
        return (name: text[..index], value: text[(index + 1)..]);
    }

    public static void SaveConfig(List<(string name, object value)> configs)
    {
        var sb = new StringBuilder();
        foreach (var item in configs)
        {
            sb.AppendLine($"{item.name}={item.value}");
        }

        if (!Directory.Exists(ConfigFolderPath)) Directory.CreateDirectory(ConfigFolderPath);
        File.WriteAllText(ConfigFilePath, sb.ToString());
    }
}