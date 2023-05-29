using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    public string theme;
    public string prompt;
    [SerializeField]
    GameObject plaqueTextObject;

    // Start is called before the first frame update
    async void Start()
    {
        if (theme == null || theme == string.Empty)
        {
            theme = AIGenerator.GetRandomPromptTheme();
        }

        prompt = await AIGenerator.GeneratePrompt(theme);
        var (texture, wasSuccessful) = await AIGenerator.GenerateImage(prompt);
        
        if (!wasSuccessful)
        {
            prompt = "Unfortunately, this image was unable to be generated.";
        }

        plaqueTextObject.SetActive(true);

        _renderer.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
