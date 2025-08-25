using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StoryLoader : MonoBehaviour
{
    [Header("UI")]
    public Image sceneImage;
    public Text sceneText; // ou TMP_Text

    [Header("Parte atual (1..3)")]
    [Range(1,3)] public int part = 1;

    void Start() { StartCoroutine(LoadPart(part)); }

    public void GoTo(int p)
    {
        part = Mathf.Clamp(p,1,3);
        StartCoroutine(LoadPart(part));
    }

    IEnumerator LoadPart(int p)
    {
        string baseUrl = StoryConfig.GetBaseUrl();
        if (string.IsNullOrEmpty(baseUrl))
        {
            sceneText.text = "Defina a URL base e clique em Salvar.";
            yield break;
        }

        string txtUrl = $"{baseUrl}/part{p}.txt";
        string imgUrl = $"{baseUrl}/part{p}.jpg"; // use .png se preferir

        // Texto
        using (UnityWebRequest req = UnityWebRequest.Get(txtUrl))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
                sceneText.text = $"Falha ao carregar texto (parte {p}): {req.error}";
            else
                sceneText.text = req.downloadHandler.text;
        }

        // Imagem
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(imgUrl))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                sceneImage.sprite = null;
                Debug.LogWarning($"Falha img parte {p}: {req.error}");
            }
            else
            {
                var tex = DownloadHandlerTexture.GetContent(req);
                var spr = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f), 100f);
                sceneImage.sprite = spr;
                sceneImage.preserveAspect = true;
            }
        }
    }
}
