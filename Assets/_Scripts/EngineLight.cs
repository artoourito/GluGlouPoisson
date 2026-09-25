using UnityEngine;
using UnityEngine.UI;

public class EngineLight : MonoBehaviour
{
    [Header("Références")]
    public Image lightImage;

    [Header("Couleurs")]
    public Color engineOffColor = Color.red;
    public Color engineOnColor = Color.green;

    private bool _isEngineOn;

    void Start()
    {
        if (lightImage == null)
            lightImage = GetComponent<Image>();

        if (lightImage == null)
        {
            Debug.LogError("EngineLight : pas d'Image trouvée !");
            return;
        }

        // 🔹 Utilise le singleton au lieu du champ Inspector
        if (InputManager.Instance != null)
        {
            InputManager.Instance.powerButtonAction += ToggleEngine;
            Debug.Log("✅ EngineLight abonné à InputManager.Instance");
        }
        else
        {
            Debug.LogError("❌ InputManager.Instance est null !");
        }

        UpdateLight();
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.powerButtonAction -= ToggleEngine;
    }

    public void ToggleEngine()
    {
        Debug.Log("🔴 ToggleEngine appelée");
        SetEngineState(!_isEngineOn);
    }

    public void SetEngineState(bool isOn)
    {
        _isEngineOn = isOn;
        UpdateLight();
    }

    private void UpdateLight()
    {
        lightImage.color = _isEngineOn ? engineOnColor : engineOffColor;
    }
}