using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using Assets.Scripts;

public class AIGenerator : MonoBehaviour
{

    private static List<string> promptTemplates = new List<string>()
    {
        //"Describe an image of <THEME> in two sentences or less.",
        "Write an image prompt to do with <THEME> in two sentences or less."
    };

    [SerializeField] private TextAsset nounListFile;
    [SerializeField] private Texture2D invalidTexture;

    private static Texture2D staticInvalidTex;
    private static List<string> nounList = new List<string>();
    private static readonly HttpClient client = new HttpClient();
    private static string dalleApiKey = "<YOUR_DALLE2_API_KEY_HERE>";
    private static string orgId = "<YOUR_OPENAPI_ORG_ID_HERE>";
    private static string imageGenUri = "https://api.openai.com/v1/images/generations";
    private static string promptGenUri = "https://api.openai.com/v1/completions";
    private static string promptGeneratorModel = "text-davinci-002";
    private static int numImagesToGenerate = 1;
    private static string sizeToGenerate = "256x256";
    private static System.Random random = new System.Random(); 
    static JsonSerializerSettings serializerSettings = new JsonSerializerSettings();

    public void Awake()
    {
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dalleApiKey);
        client.DefaultRequestHeaders.Add("OpenAI-Organization", orgId);

        staticInvalidTex = invalidTexture;
        string fs = nounListFile.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        foreach (string line in fLines)
        {
            nounList.Add(line);
        }
    }

    public static string GetRandomPromptTheme()
    {
        return nounList.ElementAt(random.Next(0, nounList.Count));
    }

    public static string GetRandomPromptTemplate()
    {
        return promptTemplates.ElementAt(random.Next(0, promptTemplates.Count));
    }

    public static async Task<string> GeneratePrompt(string theme)
    {
        Debug.Log("Generating prompt with theme: " + theme);
        string prompt = GetRandomPromptTemplate().Replace("<THEME>", theme);

        AIPromptRequestObject requestObject = new AIPromptRequestObject
        {
            model = promptGeneratorModel,
            prompt = prompt,
            temperature = 0.8f,
            max_tokens = 60
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestObject, serializerSettings), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(promptGenUri, content);

        var responseString = await response.Content.ReadAsStringAsync();

        Debug.Log("Got response: " + responseString);

        AIPromptResponseObject responseObj = JsonConvert.DeserializeObject<AIPromptResponseObject>(responseString);

        string promptText = responseObj.choices[0].text;

        string generatedPrompt = "";

        var lines = Regex.Split(promptText, "\n\n");

        // We want to ignore the first line.
        foreach(var line in lines.Skip(1).ToArray())
        {
            generatedPrompt += line;
        }

        generatedPrompt = generatedPrompt.Trim();
        generatedPrompt = generatedPrompt.Trim('.');

        generatedPrompt += ", digital art";

        return generatedPrompt;
    }

    // Returns a Texture2D generated by an AI, along with a boolean to determine if the generation was successful.
    public static async Task<(Texture2D, bool)> GenerateImage(string prompt)
    {
        AIImageRequestObject requestObject = new AIImageRequestObject
        {
            prompt = prompt,
            n = numImagesToGenerate,
            size = sizeToGenerate
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestObject, serializerSettings), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(imageGenUri, content);

        var responseString = await response.Content.ReadAsStringAsync();

        Debug.Log("Got response: " + responseString);

        AIImageResponseObject responseObj = JsonConvert.DeserializeObject<AIImageResponseObject>(responseString);

        try
        {
            var url = responseObj.data.First().url;

            return (await RemoteImageFetcher.GetRemoteTexture(url), true);
        }
        catch
        {
            return (staticInvalidTex, false);
        }
    }
}