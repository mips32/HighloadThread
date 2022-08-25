using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using highload.FileSystem;
using highload.Parser;
using highload.Settings;

namespace highload.Threading
{
    public class ThreadManager
    {
        private ConcurrentQueue<IParseResult> results;

        private ManualResetEvent[] events;
        
        private ICurrentSettings settings;

        private IFolderObject folder;
        
        private List<List<IFileObject>> files;

        private ThreadManager()
        {
        }

        public ThreadManager(ICurrentSettings settings, IFolderObject folder) : this()
        {
            this.settings = settings;
            this.folder = folder;

            this.files = new List<List<IFileObject>>(this.settings.NumberOfThreads);
            
            this.events = new ManualResetEvent[this.settings.NumberOfThreads];
            for (int i = 0; i < this.settings.NumberOfThreads; i++)
            {
                this.events[i] = new ManualResetEvent(false);
            }

            this.results = new ConcurrentQueue<IParseResult>();
        }

        private void SeparateFiles()
        {

            for (int i = 0; i < this.settings.NumberOfThreads; i++)
            {
                this.files.Add(new List<IFileObject>());
            }

            for (int i = 0; i < this.folder.DataFiles.Count; i+=this.settings.NumberOfThreads)
            {
                for (int j = 0; j < this.settings.NumberOfThreads; j++)
                {
                    if (i + j >= this.folder.DataFiles.Count)
                        break;

                    this.files[j].Add(this.folder.DataFiles[i + j]);
                }
            }
            
        }
        
        public List<KeyValuePair<string, int>> Go()
        {
            // Separate files
            this.SeparateFiles();

            // Parse files
            IParseResult[] results = new IParseResult[this.settings.NumberOfThreads];
            Thread[] threads = new Thread[this.settings.NumberOfThreads];
            for (int i = 0; i < this.settings.NumberOfThreads; i++)
            {
                WorkerParameters parameters = new WorkerParameters(this.files[i], this.settings.MinimumWordLength, i);
                threads[i] = new Thread(new ParameterizedThreadStart(Worker));
                
                threads[i].Start(parameters);
            }
            
            WaitHandle.WaitAll(this.events, TimeSpan.FromMilliseconds(30000));
            
            int threadsFinished = this.results.Count;
            for (int i = 0; i < threadsFinished; i++)
            {
                IParseResult result;
                bool isOk = this.results.TryDequeue( out result);
                if (isOk)
                {
                    results[i] = result;
                }
            }

            // Group results
            Dictionary<string, int> total = new Dictionary<string, int>(results.Length);
            foreach (IParseResult parseResult in results)
            {
                foreach (KeyValuePair<string, int> wordToFreq in parseResult.Result)
                {
                    if (total.TryGetValue(wordToFreq.Key, out int val))
                    {
                        val += wordToFreq.Value;
                        total[wordToFreq.Key] = val;
                    }
                    else
                    {
                        total.Add(wordToFreq.Key, wordToFreq.Value);
                            
                    }
                }
            }
            
            // Sort results
            IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in total orderby entry.Value descending select entry;
            List<KeyValuePair<string, int>> list = sortedDict.Take(10).ToList();
            
            return list;
        }
        
        private void Worker(Object parameters)
        {
            List<IFileObject> files = ((WorkerParameters) parameters).Files;
            int minimumWordLength = ((WorkerParameters) parameters).MinimumWordLength;
            int threadIndex = ((WorkerParameters) parameters).ThreadIndex;
            
            List<IParseResult> result = new List<IParseResult>(files.Count);
            for (int i = 0; i < files.Count; i++)
            {
                IFileParser parser = new FileParser(files[i], minimumWordLength);

                IParseResult res = parser.Parse();
                    
                result.Add(res);
            }

            Dictionary<string, int> total = new Dictionary<string, int>();
            for (int i = 0; i < result.Count; i++)
            {
                foreach (KeyValuePair<string, int> wordToFreq in result[i].Result)
                {
                    if (total.TryGetValue(wordToFreq.Key, out int val))
                    {
                        val += wordToFreq.Value;
                        total[wordToFreq.Key] = val;
                    }
                    else
                    {
                        total.Add(wordToFreq.Key, wordToFreq.Value);
                    }
                }
                
            }

            this.results.Enqueue(new ParseResult(total));
            this.events[threadIndex].Set();
        }

    }

    public class WorkerParameters
    {
        public List<IFileObject> Files { get; set; }

        public int MinimumWordLength { get; set; }

        public int ThreadIndex { get; set; }

        public WorkerParameters(List<IFileObject> files, int minimumWordLength, int threadIndex)
        {
            this.Files = files;
            this.MinimumWordLength = minimumWordLength;
            this.ThreadIndex = threadIndex;
        }
    }
}