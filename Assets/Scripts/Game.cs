using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game stat object. Use this to track anything that would otherwise be a global.
/// It's a MonoBehaviour so that it can initialize things on Start.
/// </summary>
public class Game : MonoBehaviour
{
    public database.Database.Image currentImage = null;
    [HideInInspector]
    public List<string> outstandingNotUnderstoodWords = new List<string>();
    
    public database.Scorer myScorer { get; private set; }

    public void Start()
    {
        database.Database.Initialize();
        this.myScorer = new database.Scorer();
        database.DadResponder.Initialize();
    }

    #region Singleton management
    private static Game instance;
    public static Game Instance {
        get { return instance; }
    }
    #endregion
}
