using UnityEngine;

public class PlayerRatingManager : MonoBehaviour
{
    private const string RatingKey = "PlayerRating";

    public int PlayerRating
    {
        get { return PlayerPrefs.GetInt(RatingKey, 1000); }
        set { PlayerPrefs.SetInt(RatingKey, value); }
    }

    public void IncreaseRating(int amount)
    {
        PlayerRating += amount;
    }

    public void DecreaseRating(int amount)
    {
        PlayerRating -= amount;
    }
}
