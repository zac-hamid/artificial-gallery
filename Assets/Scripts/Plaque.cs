using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plaque : MonoBehaviour
{
    public Painting painting;
    public TextMeshPro tmPro;

    public void OnEnable()
    {
        tmPro.SetText(painting.prompt);
    }
}
