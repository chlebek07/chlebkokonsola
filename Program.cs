using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ChlebkoKonsola
{
    internal class Program
    {
        static List<string> commandHistory = new();
        static Dictionary<string, string> variables = new();
        static DateTime startTime = DateTime.Now;
        static string currentDirectory = Directory.GetCurrentDirectory();

        static void Main()
        {
            Console.Title = "chlebko-konsola v2.0";
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("== chlebko-konsola v2.0 == wpisz 'help' by wyświetlić pomoc");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"{Environment.UserName}@chlebko:{currentDirectory}> ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine()?.Trim() ?? "";
                commandHistory.Add(input);

                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.Split(' ', 2);
                string command = parts[0].ToLower();
                string args = parts.Length > 1 ? parts[1] : "";

                switch (command)
                {
                    case "help": ShowHelp(); break;
                    case "exit": return;
                    case "cls": Console.Clear(); break;
                    case "autor": Console.WriteLine("Stworzone przez: chlebek07 © 2025"); break;
                    case "whoami": Console.WriteLine(Environment.UserName); break;
                    case "ver": Console.WriteLine(Environment.OSVersion); break;
                    case "time": Console.WriteLine(DateTime.Now.ToString("HH:mm:ss")); break;
                    case "date": Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd")); break;
                    case "uptime": Console.WriteLine((DateTime.Now - startTime).ToString()); break;
                    case "pwd": Console.WriteLine(currentDirectory); break;
                    case "cd": ChangeDirectory(args); break;
                    case "ls": ListFiles(); break;
                    case "echo": Console.WriteLine(args); break;
                    case "history": foreach (var cmd in commandHistory) Console.WriteLine(cmd); break;
                    case "clearhistory": commandHistory.Clear(); Console.WriteLine("Historia wyczyszczona."); break;
                    case "set": SetVar(args); break;
                    case "get": GetVar(args); break;
                    case "vars": foreach (var kv in variables) Console.WriteLine($"{kv.Key}={kv.Value}"); break;
                    case "clearvars": variables.Clear(); Console.WriteLine("Wyczyszczono zmienne."); break;
                    case "calc": SimpleCalc(args); break;
                    case "sleep": if (int.TryParse(args, out int ms)) Thread.Sleep(ms); break;
                    case "beep": Console.Beep(); break;
                    case "color": ChangeColor(args); break;
                    case "ipconfig": ShowIpInfo(); break;
                    case "mkdir": TryMakeDir(args); break;
                    default: Console.WriteLine("Nieznana komenda. Użyj 'help'."); break;
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Lista komend:");
            Console.WriteLine(" help           - pokaż pomoc");
            Console.WriteLine(" exit           - zamknij konsolę");
            Console.WriteLine(" cls            - wyczyść ekran");
            Console.WriteLine(" autor          - informacje o autorze");
            Console.WriteLine(" whoami         - nazwa użytkownika");
            Console.WriteLine(" ver            - wersja systemu");
            Console.WriteLine(" uptime         - czas działania konsoli");
            Console.WriteLine(" pwd            - pokaż ścieżkę");
            Console.WriteLine(" cd [folder]    - zmień katalog");
            Console.WriteLine(" ls             - pokaż pliki w katalogu");
            Console.WriteLine(" echo tekst     - wyświetl tekst");
            Console.WriteLine(" history        - pokaż historię komend");
            Console.WriteLine(" clearhistory   - wyczyść historię");
            Console.WriteLine(" set x=wartość  - ustaw zmienną");
            Console.WriteLine(" get x          - pokaż zmienną");
            Console.WriteLine(" vars           - pokaż wszystkie zmienne");
            Console.WriteLine(" clearvars      - wyczyść zmienne");
            Console.WriteLine(" calc 2+2       - proste liczenie");
            Console.WriteLine(" sleep ms       - pauza w ms");
            Console.WriteLine(" beep           - dźwięk systemowy");
            Console.WriteLine(" color red      - zmień kolor tekstu");
            Console.WriteLine(" ipconfig       - info o sieci");
            Console.WriteLine(" mkdir [nazwa]  - utwórz folder");
        }

        static void ChangeDirectory(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) return;
                string newPath = Path.GetFullPath(Path.Combine(currentDirectory, path));
                if (Directory.Exists(newPath))
                {
                    currentDirectory = newPath;
                    Directory.SetCurrentDirectory(newPath);
                }
                else
                    Console.WriteLine("Katalog nie istnieje.");
            }
            catch { Console.WriteLine("Błąd zmiany katalogu."); }
        }

        static void ListFiles()
        {
            try
            {
                var dirs = Directory.GetDirectories(currentDirectory);
                var files = Directory.GetFiles(currentDirectory);

                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var d in dirs) Console.WriteLine("[D] " + Path.GetFileName(d));
                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var f in files) Console.WriteLine("    " + Path.GetFileName(f));
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch { Console.WriteLine("Błąd wyświetlania plików."); }
        }

        static void SetVar(string arg)
        {
            var split = arg.Split("=", 2);
            if (split.Length == 2)
            {
                variables[split[0].Trim()] = split[1].Trim();
                Console.WriteLine($"Zmienna '{split[0]}' ustawiona.");
            }
            else Console.WriteLine("Użycie: set nazwa=wartość");
        }

        static void GetVar(string key)
        {
            if (variables.TryGetValue(key.Trim(), out var value))
                Console.WriteLine(value);
            else Console.WriteLine("Zmienna nie istnieje.");
        }

        static void SimpleCalc(string expr)
        {
            try
            {
                var dt = new System.Data.DataTable();
                var result = dt.Compute(expr, "");
                Console.WriteLine($"= {result}");
            }
            catch { Console.WriteLine("Błąd w wyrażeniu."); }
        }

        static void ChangeColor(string color)
        {
            try
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
            }
            catch { Console.WriteLine("Zły kolor. Przykład: red, green, yellow."); }
        }

        static void ShowIpInfo()
        {
            try
            {
                var output = new Process
                {
                    StartInfo = new ProcessStartInfo("ipconfig")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                output.Start();
                Console.WriteLine(output.StandardOutput.ReadToEnd());
            }
            catch { Console.WriteLine("Nie udało się uzyskać informacji o sieci."); }
        }

        static void TryMakeDir(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return;
                string path = Path.Combine(currentDirectory, name);
                Directory.CreateDirectory(path);
                Console.WriteLine($"Utworzono folder: {path}");
            }
            catch { Console.WriteLine("Błąd tworzenia folderu."); }
        }
    }
}
