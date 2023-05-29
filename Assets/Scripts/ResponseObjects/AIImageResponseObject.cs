using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIImageResponseObject
{
    public struct UrlData
    {
        public string url;
    };

    public long created { get; set; }
    public List<UrlData> data { get; set; }
}
