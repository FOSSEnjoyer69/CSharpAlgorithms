/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if WINDOWS
using Microsoft.Win32;
#endif

namespace CSharpAlgorithms.Computer;

public static class SteamUtils
{
    public static string[] GetSteamInstallDirectories()
    {
        List<string> directories = [];

#if WINDOWS
        var regPaths = new[]
        {
            (hive: RegistryHive.CurrentUser, subKey: @"Software\Valve\Steam", valueName: "SteamPath"),
            (hive: RegistryHive.LocalMachine, subKey: @"SOFTWARE\WOW6432Node\Valve\Steam", valueName: "InstallPath"),
            (hive: RegistryHive.LocalMachine, subKey: @"SOFTWARE\Valve\Steam", valueName: "InstallPath"),


        };

        foreach (var (hive, subKey, valueName) in regPaths)
        {
            try
            {
                using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
                using var key = baseKey.OpenSubKey(subKey);
                var value = key?.GetValue(valueName) as string;
                if (!string.IsNullOrWhiteSpace(value))
                    list.Add(value);
            }
            catch { }
        }

        // Common fallbacks
        var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        if (!string.IsNullOrWhiteSpace(pf))
                list.Add(Path.Combine(pf, "Steam"));

        var pf64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        if (!string.IsNullOrWhiteSpace(pf64))
            list.Add(Path.Combine(pf64, "Steam"));
#elif LINUX
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrWhiteSpace(home))
        {
            list.Add(Path.Combine(home, ".steam", "steam"));
            list.Add(Path.Combine(home, ".local", "share", "Steam"));
        }
#elif MACOS

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrWhiteSpace(home))
            list.Add(Path.Combine(home, "Library", "Application Support", "Steam"));
#endif
        return [.. directories.Select(PathUtils.NormalizePath)
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .Where(Directory.Exists)];
    }
    public static bool TryFindGameLibraryDirectory(string gameName, out DirectoryInfo[] gameDirectories)
    {
        string[] possibleDirs = GetSteamInstallDirectories();
        if (possibleDirs.Length == 0)
        {
            gameDirectories = [];
            return false;
        }


        return true;
    }
}
*/