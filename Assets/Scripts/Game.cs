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

    public bool isGameOver { get { return this.confusion >= this.understanding; } }

    [SerializeField]
    private float understanding = 0;
    [SerializeField]
    private float confusion = 0;
    public float Understanding { get { return this.understanding; } set { this.understanding = value; } }
    public float Confusion { get { return this.confusion; } set { this.confusion = value; } }
    [SerializeField]
    [Tooltip("The amount by which confusion should increase by default every second.")]
    private float confusionRateOfIncrease;
    [SerializeField]
    [Tooltip("The maximum numeric amount for both confusion and understanding. This is the amount that would fill up either bar on its own.")]
    private float confunderstansionMaxValue;
    
    public database.Scorer myScorer { get; private set; }

    public void Awake() // Awake so it can be used in Start elsewhere.
    {
		instance = this;
        database.Database.Initialize();
        this.myScorer = new database.Scorer();
        database.DadResponder.Initialize();
    }

    public void Update()
    {
        this.confusion += Time.deltaTime * this.confusionRateOfIncrease;
        if (this.isGameOver) {
            this.EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Hey! The game is over now!");
    }

    #region Singleton management
    private static Game instance;
    public static Game Instance {
        get { return instance; }
    }
    #endregion
}
