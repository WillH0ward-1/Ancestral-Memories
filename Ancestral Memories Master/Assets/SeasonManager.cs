using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class SeasonManager : MonoBehaviour
{
    public enum Season { Spring, Summer, Autumn, Winter }

    [SerializeField]
    private int _dayOfYear;

    public Season initSeason = Season.Summer;
    [SerializeField, Range(0, 3)]
    private int currentSeasonIndex;
    [SerializeField, Range(0, 24)]
    private float timeOfDay;

    [SerializeField, Range(1, 10)]
    private int seasonSpeedMultiplier = 1; // Serialized setting for controlling the speed of season transitions

    public SeasonEvent OnSeasonChanged = new SeasonEvent();

    [System.Serializable]
    public class SeasonEvent : UnityEvent<Season> { }

    public Season _currentSeason; // Serialized field to display the current season in the Inspector

    private MapObjGen mapObjGen;

    public Season CurrentSeason
    {
        get { return _currentSeason; }
        set
        {
            if (_currentSeason != value)
            {
                _currentSeason = value;
                currentSeasonIndex = (int)_currentSeason;
                _dayOfYear = currentSeasonIndex * timeCycle.daysPerSeason + (_dayOfYear % timeCycle.daysPerSeason);
                if (timeCycle != null)
                {
                    timeCycle.DayOfYear = _dayOfYear;
                }
                OnSeasonChanged.Invoke(_currentSeason);
            }
        }
    }

    public int DayOfYear
    {
        get { return _dayOfYear; }
        set
        {
            int totalDaysInYear = GetTotalDaysInYear();
            if (totalDaysInYear > 0)
            {
                _dayOfYear = value % totalDaysInYear;
                currentSeasonIndex = _dayOfYear / timeCycle.daysPerSeason; // Access through TimeCycleManager
                CurrentSeason = (Season)currentSeasonIndex;
                if (timeCycle != null)
                {
                    timeCycle.DayOfYear = _dayOfYear;
                }
            }
        }
    }

    public void InitTime()
    {
        timeCycle = GetComponent<TimeCycleManager>();
        SetInitialSeason();
        UpdateTotalDaysInYear();
    }

    public int GetTotalDaysInYear()
    {
        return GetNumberOfSeasons() * timeCycle.daysPerSeason; // Access through TimeCycleManager
    }

    public TimeCycleManager timeCycle;

    private int GetNumberOfSeasons()
    {
        return System.Enum.GetValues(typeof(Season)).Length;
    }

    // Serialized field to display the total number of days in the inspector
    [SerializeField]
    private int _totalDaysInYear;

    public Season GetCurrentSeason(int dayOfYear)
    {
        int seasonIndex = dayOfYear / timeCycle.daysPerSeason;
        return (Season)(seasonIndex % GetNumberOfSeasons());
    }

    public Color[] leafColors; // Define in the inspector, one color for each season
    private Dictionary<string, Color> seasonToColor;

    private void OnEnable()
    {
        timeCycle = GetComponent<TimeCycleManager>();
        mapObjGen = FindObjectOfType<MapObjGen>();

        SetInitialSeason();
        UpdateTotalDaysInYear();

        // Initialize the dictionary with the colors for each season
        seasonToColor = new Dictionary<string, Color>();
        var seasons = System.Enum.GetNames(typeof(Season));

        // Resize the leafColors array to match the number of seasons
        System.Array.Resize(ref leafColors, seasons.Length);
        for (int i = 0; i < seasons.Length; i++)
        {
            // Use a default color if one wasn't set in the inspector
            if (leafColors[i] == null)
                leafColors[i] = Color.white; // Default color

            seasonToColor[seasons[i]] = leafColors[i];
        }

        // Subscribe to the OnSeasonChanged event
        OnSeasonChanged.AddListener(ChangeLeafColorBasedOnSeason);
    }

    private void ChangeLeafColorBasedOnSeason(Season season)
    {
        string seasonName = season.ToString();
        if (seasonToColor.ContainsKey(seasonName))
        {
            Color newLeafColor = seasonToColor[seasonName];
            mapObjGen.LerpLeafColour(newLeafColor);
        }
    }


    private void SetInitialSeason()
    {
        CurrentSeason = initSeason;
        DayOfYear = (int)initSeason * timeCycle.daysPerSeason;
    }


    void Update()
    {
        if (timeCycle != null)
        {
            timeOfDay = timeCycle.TimeOfDay;
            DayOfYear = timeCycle.DayOfYear;
        }
    }

    // Method to update the total number of days based on the current settings
    private void UpdateTotalDaysInYear()
    {
        int numberOfSeasons = GetNumberOfSeasons();
        _totalDaysInYear = numberOfSeasons * timeCycle.daysPerSeason;

        // Update the maximum range of the _dayOfYear field based on the total number of days
        _dayOfYear = Mathf.Clamp(_dayOfYear, 0, _totalDaysInYear - 1);
    }
}
