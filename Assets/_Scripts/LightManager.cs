using UnityEngine;
using UnityEngine.UI;

public class EngineLight : MonoBehaviour
{
    [Header("Références")]
    public Image lightImage;
    public InputManager inputManager;

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

        if (inputManager != null)
            inputManager.powerButtonAction += ToggleEngine; // ✅ ici c'est OK
        else
            Debug.LogWarning("EngineLight : inputManager non assigné !");

        UpdateLight();
    }

    void OnDestroy()
    {
        if (inputManager != null)
            inputManager.powerButtonAction -= ToggleEngine;
    }

    // ✅ Cette méthode appartient à EngineLight, donc accessible ici
    public void ToggleEngine()
    {
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