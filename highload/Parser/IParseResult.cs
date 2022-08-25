using System.Collections.Generic;

namespace highload.Parser
{
    public interface IParseResult
    {
        // Word to frequency
        Dictionary<string, int> Result { get; set; }
    }
}