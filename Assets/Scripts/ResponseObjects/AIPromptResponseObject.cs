
using System.Collections.Generic;

public class AIPromptResponseObject
{
    public struct ChoicesData 
    {
        public string text;
        public int index;
        public object logprobs;
        public string finish_reason;
    }

    public struct UsageData
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    public string id { get; set; }
    public string Object { get; set; }
    public long created { get; set; }
    public string model { get; set; }

    public List<ChoicesData> choices { get; set; }
    public UsageData usage { get; set; }

}
