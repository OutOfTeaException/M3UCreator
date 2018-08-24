using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Reflection;

namespace M3UCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "M3UCreator";
            app.Description = "Creates an m3u file from a directory that contains mp3 files";
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", () => {
                return string.Format("Version {0}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            });

            var argMp3Directory = app.Argument("mp3Directory", "Directory of the mp3 files");
            var optionOutputDirectory = app.Option("-o|--outputDirectory", "Optionally specify the directory, where the m3u file is generated", CommandOptionType.SingleValue);
            var optionM3uName = app.Option("-n|--m3uName", "Optionally specify the name of the generated m3u file", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (String.IsNullOrEmpty(argMp3Directory.Value))
                {
                    Console.WriteLine("The argument 'mp3Directory' is mandatory!");
                    app.ShowHelp();
                    return -1;
                }

                if (!Directory.Exists(argMp3Directory.Value))
                {
                    Console.WriteLine($"The specified directory containing the mp3 files does not exists: '{argMp3Directory.Value}'!");
                    return -2;
                }

                Console.WriteLine("Generating m3u file...");
                var m3uCreator = new M3UCreator();
                m3uCreator.CreateM3u(argMp3Directory.Value, optionM3uName.Value(), optionOutputDirectory.Value());

                return 0;
            });


            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                // You'll always want to catch this exception, otherwise it will generate a messy and confusing error for the end user.
                // the message will usually be something like:
                // "Unrecognized command or argument '<invalid-command>'"
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
            }
        }
    }
}
