using System;
using System.IO;

namespace highload.Settings
{
    public class CurrentSettings : ICurrentSettings
    {
        private const int MINIMUM_NUMBER_OF_THREADS = 1;
        private const int DEFAULT_NUMBER_OF_THREADS = 8;
        private const int MAXIMUM_NUMBER_OF_THREADS = 16;
        
        private const int MINIMUM_NUMBER_OF_ARGUMENTS = 1;
        private const int MAXIMUM_NUMBER_OF_ARGUMENTS = 3;

        private const int MINIMUM_WORD_LENGTH = 1;
        private const int DEFAULT_WORD_LENGTH = 8;
        private const int MAXIMUM_WORD_LENGTH = 128;

        private const string DIRECTORY_NOT_EXIST_MESSAGE = "Invalid arguments: directory not exists";
        private const string WRONG_NUMBER_OF_THREADS = "Invalid argument: wrong number of threads";
        private const string WRONG_WORD_LENGTH = "Invalid argument: wrong word length";
        public string DataPath { get; private set; }

        public int NumberOfThreads { get; private set; }

        public int MinimumWordLength { get; private set; }

        private CurrentSettings()
        {
        }

        private CurrentSettings(string path) : this(path, DEFAULT_NUMBER_OF_THREADS)
        {
            this.DataPath = path;
        }

        public CurrentSettings(string path, int numberOfThreads) : this(path, numberOfThreads, DEFAULT_WORD_LENGTH)
        {
            this.DataPath = path;
            this.NumberOfThreads = numberOfThreads;
        }

        public CurrentSettings(string path, int numberOfThreads, int minimumWordLenght)
        {
            this.DataPath = path;
            this.NumberOfThreads = numberOfThreads;
            this.MinimumWordLength = minimumWordLenght;
        }

        public static CurrentSettings ParseSettings(string[] args)
        {
            string dataPath;
            int numberOfThreads;
            int minimumWordLenght;

            if (args.Length < MINIMUM_NUMBER_OF_ARGUMENTS || args.Length > MAXIMUM_NUMBER_OF_ARGUMENTS)
            {
                PrintHelp();
                return null;
            }

            try
            {
                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine(DIRECTORY_NOT_EXIST_MESSAGE);
                    return null;
                }

                dataPath = Path.GetFullPath(args[0]);
                numberOfThreads = DEFAULT_NUMBER_OF_THREADS;
                minimumWordLenght = DEFAULT_WORD_LENGTH;

                int threads = DEFAULT_NUMBER_OF_THREADS;
                if (args.Length > 1)
                {
                    threads = ParseInteger(args[1], MINIMUM_NUMBER_OF_THREADS, MAXIMUM_NUMBER_OF_THREADS, WRONG_NUMBER_OF_THREADS);
                    if (threads == Int32.MinValue)
                        return null;

                    numberOfThreads = threads;
                    minimumWordLenght = DEFAULT_WORD_LENGTH;
                }
                
                if (args.Length == 3)
                {
                    int wordLen = ParseInteger(args[2], MINIMUM_WORD_LENGTH, MAXIMUM_WORD_LENGTH, WRONG_WORD_LENGTH);
                    if (wordLen == Int32.MinValue)
                        return null;

                    minimumWordLenght = wordLen;
                }
                
                return new CurrentSettings(dataPath, numberOfThreads, minimumWordLenght);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                return null;
            }
            
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: highload <\"path\"> [number of threads, 8 by default, 16 maximum] [minimum word lenght, 8 by default, 128 maximum]");
            Console.WriteLine("Sample: highload \"C:\test\\\" 4 8");
        }

        private static int ParseInteger(string number, int minimum, int maximum, string errorValue)
        {
            int parsed = 0;
            bool parseResult = int.TryParse(number, out parsed);

            if (!parseResult || parsed < minimum || parsed > maximum)
            {
                Console.WriteLine(errorValue);
                return Int32.MinValue;
            }
            
            return parsed;
        }
        
    }
}