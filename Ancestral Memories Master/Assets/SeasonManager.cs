using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class SeasonManager : MonoBehaviour
{
    public enum Season { Spring, Summer, Autumn, Winter }

    [SerializeField]
    private int _dayOfYear;
    public int daysPerSeason = 90;

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

    public Season CurrentSeason
    {
        get { return _currentSeason; }
        set
        {
            if (_currentSeason != value)
            {
                _currentSeason = value;
                currentSeasonIndex = (int)_currentSeason;
                _dayOfYear = currentSeasonIndex * daysPerSeason + (_dayOfYear % daysPerSeason);
                if (timeCycle != null)
                {
                    timeCycle.dayOfYear = _dayOfYear;
                }
                OnSeasonChanged.Invoke(_currentSeason);
            }
        }
    }

    public int dayOfYear
    {
        get { return _dayOfYear; }
        set
        {
            int totalDaysInYear = TotalDaysInYear;
            if (totalDaysInYear > 0)
            {
                _dayOfYear = value % totalDaysInYear; // Use the property to calculate the total number of days
                currentSeasonIndex = _dayOfYear / daysPerSeason;
                CurrentSeason = (Season)currentSeasonIndex;
                if (timeCycle != null)
                {
                    timeCycle.dayOfYear = _dayOfYear;
                }
            }
        }
    }

    private TimeCycleManager timeCycle;

    private int GetNumberOfSeasons()
    {
        return System.Enum.GetValues(typeof(Season)).Length;
    }

    // Serialized field to display the total number of days in the inspector
    [SerializeField]
    private int _totalDaysInYear;
    public int TotalDaysInYear
    {
        get { return _totalDaysInYear; }
        set { _totalDaysInYear = value; }
    }

    private void OnEnable()
    {
        timeCycle = GetComponent<TimeCycleManager>();
        SetInitialSeason();
        UpdateTotalDaysInYear();
    }

    private void Awake()
    {
        timeCycle = GetComponent<TimeCycleManager>();
        SetInitialSeason();
        UpdateTotalDaysInYear();
    }

    private void SetInitialSeason()
    {
        CurrentSeason = initSeason;
        dayOfYear = (int)initSeason * daysPerSeason;
    }

    void Update()
    {
        if (timeCycle != null)
        {
            timeOfDay = timeCycle.timeOfDay;
            dayOfYear = timeCycle.dayOfYear;
        }
    }

    // Method to update the total number of days based on the current settings
    private void UpdateTotalDaysInYear()
    {
        int numberOfSeasons = GetNumberOfSeasons();
        _totalDaysInYear = numberOfSeasons * daysPerSeason;

        // Update the maximum range of the _dayOfYear field based on the total number of days
        _dayOfYear = Mathf.Clamp(_dayOfYear, 0, _totalDaysInYear - 1);
    }
}
