using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // singleton - mame jednu instanciu, ku ktorej pristupuje zkadekolvek 
    public static MusicManager Instance; 

    [SerializeField] public AudioSource musicSource;
    [SerializeField] private MusicLibrary musicLibrary;

    // volume z options
    private float userVolume = 1f;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // not destoyed ked sa menia scenes
        }
        else
        {
            // ak instancia existuje tak detroy duplicitnu 
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    public void SetMusicVolume(float value)
    {
        userVolume = value;
        // actually tu ma byt logaritmicky vztah kvoli tomu ako nase ucho vnima zvuk 
        float logVolume = Mathf.Pow(value, 2f); 

        musicSource.volume = userVolume;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicSource.mute = !enabled;
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
    }

    // fading efekt pri zmene sound tracku
    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        // stisujeme koniec tracku, Mathf.Lerp(a, b, t) v podstate vrati interpolovanu (t) hodnotu medzi a a b 
        // cize ak t = 0.5 tak finckia vrati strednu hodnotu medzi a a b
        float percent = 0;

        while (percent < 1) 
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent) * userVolume;
            yield return null;
        }

        // prepneme na dalsi song z listu
        musicSource.clip = nextTrack;
        musicSource.Play();

        // a teraz presne naopak zvysujeme hlasitost
        percent = 0;
        while (percent < 1) 
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent) * userVolume;
            yield return null;
        }
    }

    void Start()
    {
        // Load saved settings
       // userVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        //int enabled = PlayerPrefs.GetInt("MusicEnabled", 1);

        userVolume = 1f;

        musicSource.volume = userVolume;
        //musicSource.mute = enabled == 0;
    }
}
