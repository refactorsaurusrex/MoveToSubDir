using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentConsole.Library;
using static System.ConsoleColor;

namespace MoveToSubDir
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Move To SubDir";

            try
            {
                MoveFiles(args);
            }
            catch (Exception ex)
            {
                "Crap! Something went wrong".WriteLine(2);
                ex.WriteLineWait();
            }
        }

        static void MoveFiles(string[] filePaths)
        {
            if (filePaths.Length == 0)
                return;

            var firstName = Path.GetFileName(filePaths[0]) ?? "";
            var dashIndex = firstName.IndexOf('-');
            var subDir = "";
            if (dashIndex > 0)
            {
                subDir = firstName.Substring(0, dashIndex).Trim();
                $"Suggested subdirectory name: {subDir}".WriteLine();
            }

            Console.WriteLine("Press enter to use suggested name");
            Console.Write("Or, type a different name here: ");
            var result = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(result))
            {
                subDir = Regex.Replace(result, @"[\<\>\:""\/\\\|?\*]", "_");
            }
            
            var parentDir = Path.GetDirectoryName(filePaths[0]) ?? "";

            if (subDir == "" || parentDir == "")
                return;

            var subDirFullName = Path.Combine(parentDir, subDir);

            if (!Directory.Exists(subDirFullName))
                Directory.CreateDirectory(subDirFullName);

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(fileName))
                    continue;

                var destinationFilePath = Path.Combine(subDirFullName, fileName);

                if (File.Exists(destinationFilePath))
                {
                    "The following file already exists at the destination location:".WriteLine(Red);
                    $"'{destinationFilePath}'".WriteLine();
                    "Overwrite? (Y/N)".WriteLine(Red);
                    var answer = Console.ReadKey(true);

                    if (answer.Key != ConsoleKey.Y)
                    {
                        $"Skipping '{destinationFilePath}'.".WriteLine();
                        continue;
                    }
                }

                File.Move(filePath, destinationFilePath);
                $"Moved '{destinationFilePath}'".WriteLine();
            }

            "Done! Press any key to exit...".WriteLineWait();
        }
    }
}
