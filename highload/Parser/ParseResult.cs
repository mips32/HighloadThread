using System.Collections.Generic;

namespace highload.Parser
{
    public class ParseResult : IParseResult
    {
        public Dictionary<string, int> Result { get; set; }

        private ParseResult()
        {
        }

        public ParseResult(Dictionary<string, int> parseResult) : this()
        {
            this.Result = parseResult;
        }

        public ParseResult(int capacity) : this()
        {
            this.Result = new Dictionary<string, int>(capacity);
        }
    }
}
