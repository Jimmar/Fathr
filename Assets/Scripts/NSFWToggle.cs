using UnityEngine;

/// <summary>
/// A toggle on the main menu for turning on/off the NSFW images.
/// </summary>
public class NSFWToggle : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Toggle nsfwToggle;

    public void Start()
    {
        this.nsfwToggle.isOn = Game.IsNSFWOn;
    }

    public void OnToggle()
    {
        Game.IsNSFWOn = this.nsfwToggle.isOn;
    }
}
