using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using highload.FileSystem;

namespace highload.Parser
{
    public class FileParser : IFileParser
    {
        private Regex rx;

        private IFileObject fileObject;
        
        private FileParser()
        {
        }

        public FileParser(IFileObject fileObject, int minimumWordsLength) : this()
        {
            string pattern = $"(\\w+){{{minimumWordsLength},}}";
            this.rx = new Regex(pattern);

            this.fileObject = fileObject;
        }

        public IParseResult Parse()
        {
            string content;
            
            Dictionary<string, int> words = new Dictionary<string, int>(1024);

            using (StreamReader inputReader = new StreamReader(fileObject.Path))
            {
                while (null != (content = inputReader.ReadLine()))
                {
                    MatchCollection matches = rx.Matches(content);
                    
                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];
                        int currentCount = 0;
                        words.TryGetValue(match.Value, out currentCount);

                        currentCount++;
                        words[match.Value] = currentCount;
                    }
                }
            }
            
            IParseResult res = new ParseResult(words);
            return res;
        }

    }
}