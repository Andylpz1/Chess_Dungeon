using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintManager : MonoBehaviour
{
    private GameObject effectTextObject;
    private Coroutine showEffectTextCoroutine;

    public void ShowHint(string text, Vector3 position)
    {
        if (showEffectTextCoroutine == null)
        {
            showEffectTextCoroutine = StartCoroutine(ShowEffectTextAfterDelay(1f, text, position)); // 悬停1秒后显示
        }
    }

    public void HideHint()
    {
        if (showEffectTextCoroutine != null)
        {
            StopCoroutine(showEffectTextCoroutine);
            showEffectTextCoroutine = null;
        }

        if (effectTextObject != null)
        {
            Destroy(effectTextObject);
        }
    }

    private IEnumerator ShowEffectTextAfterDelay(float delay, string text, Vector3 position)
    {
        yield return new WaitForSeconds(delay);

        if (effectTextObject == null)
        {
            effectTextObject = new GameObject("EffectText");
            effectTextObject.transform.SetParent(GameObject.Find("Canvas").transform, false); // 将effectTextObject设置为Canvas的子对象

            Text effectText = effectTextObject.AddComponent<Text>();
            effectText.text = text;
            effectText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            effectText.fontSize = 34;
            effectText.color = Color.white;
        }
    }
}
