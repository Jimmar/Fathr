using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Word = database.Database.Word;

/// <summary>
/// Main game stat object. Use this to track anything that would otherwise be a global.
/// It's a MonoBehaviour so that it can initialize things on Start.
/// </summary>
public class Game : MonoBehaviour
{
    public database.Database.Image currentImage = null;
    [HideInInspector]
    public List<Word> outstandingNotUnderstoodWords { get; set; }
    public List<string> aggregatedEmotions { get; set; } // By occurrence.

    [SerializeField]
    private MusicManager musicManager;

    public bool isGameOver { get { return this.confusion + this.understanding >= this.confunderstansionMaxValue; } }
    
    [SerializeField]
    private float understanding = 4;
    [SerializeField]
    private float confusion = 4;
    public float Understanding { // NOTE: Since we're clamping to 0 on every call, some data will be lost when these are near 0.
        get { return this.understanding; }
        set { this.understanding = Mathf.Clamp(value, 0f, this.confunderstansionMaxValue); this.barUnderstanding.fillAmount = this.understanding / this.confunderstansionMaxValue; }
    }
    public float Confusion {
        get { return this.confusion; }
        set { this.confusion = Mathf.Clamp(value, 0f, this.confunderstansionMaxValue); this.barConfusion.fillAmount = this.confusion / this.confunderstansionMaxValue; }
    }
    [SerializeField]
    [Tooltip("The amount by which confusion should increase by default every second.")]
    private float confusionRateOfIncrease = 0.05f;
    [SerializeField]
    [Tooltip("The maximum numeric amount for both confusion and understanding. This is the amount that would fill up either bar on its own.")]
    private float confunderstansionMaxValue = 100f;
    [SerializeField]
    [Tooltip("Filled image for understanding bar.")]
    private UnityEngine.UI.Image barUnderstanding;
    [SerializeField]
    [Tooltip("Filled image for confusion bar.")]
    private UnityEngine.UI.Image barConfusion;
    
    public database.Scorer myScorer { get; private set; }

    public void Awake() // Awake so it can be used in Start elsewhere.
    {
        instance = this;
        this.outstandingNotUnderstoodWords = new List<Word>();
        this.aggregatedEmotions = new List<string>();
        database.Database.Initialize();
        this.myScorer = new database.Scorer();
        database.DadResponder.Initialize();
        this.Understanding = this.understanding; // Initialize bars.
        this.Confusion = this.confusion;
    }

    public void Update()
    {
        if (!this.isGameOver) {
            this.Confusion += Time.deltaTime * this.confusionRateOfIncrease;
        } else {
            this.EndGame();
        }
        // Dumb check for music.
        if (this.understanding > this.confusion * 2 && this.understanding > 20) {
            this.musicManager.Understanding();
        } else if (this.understanding < this.confusion * 0.5 && this.confusion > 20) {
            this.musicManager.Confused();
        } else {
            this.musicManager.Natural();
        }
    }

    private void EndGame()
    {
        string resultString = "";
        string prevalentEmotion = this.aggregatedEmotions.Count > 0 ? 
            this.aggregatedEmotions[random.Random.Range(0, this.aggregatedEmotions.Count)] :
            "weird";
        bool didWin = this.understanding > this.confusion;
        resultString =
            this.understanding > this.confusion * 2 ? string.Format("Thanks, I really get it! Tumblr sure is {0}, huh?", prevalentEmotion) :
            this.understanding > this.confusion * 1.5 ? string.Format("Hmm, I think I get it. Golly, Tumblr sure is {0}!", prevalentEmotion) :
            this.understanding > this.confusion ? string.Format("Hmm, Tumblr seems very...{0}? Thanks for...this.", prevalentEmotion) :
            this.understanding > this.confusion * 0.5 ? string.Format("I guess I just don't get any of this. But thanks for showing me.") :
            this.understanding > this.confusion * 0.1 ? string.Format("All righty, well, I'm going to get back to the garage. Have fun tumbling or whatever you call it.") :
            "Why do millenials like any of this? Kids these days, I tell ya.";

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.DadSays(resultString);
    }

    #region Singleton management
    private static Game instance;
    public static Game Instance {
        get { return instance; }
    }
    #endregion
}
