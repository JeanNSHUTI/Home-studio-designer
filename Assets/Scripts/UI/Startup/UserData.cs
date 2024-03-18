using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserData 
{
    private string userTypePref = string.Empty;
    private string userPriceRangePref = string.Empty;
    private List<string> userStylePrefs;
    private List<string> userPlacementPrefs;
    private bool userPopularityInterest = true;

    public UserData(int sylesSize, int placementSize)
    {
        userStylePrefs = new List<string>();
        userPlacementPrefs = new List<string>();
    }

    public string UserTypePref { get => userTypePref; set => userTypePref = value; }
    public string UserPriceRangePref { get => userPriceRangePref; set => userPriceRangePref = value; }
    public List<string> UserStylePrefs { get => userStylePrefs; set => userStylePrefs = value; }
    public List<string> UserPlacementPrefs { get => userPlacementPrefs; set => userPlacementPrefs = value; }
    public bool UserPopularityInterest { get => userPopularityInterest; set => userPopularityInterest = value; }

    public override string ToString()
    {
        return "User data \nType: " + UserTypePref + " PriceRange: " + UserPriceRangePref + " Styles: " + string.Join("; ", UserStylePrefs) +
            "Placements: " + string.Join("; ", UserPlacementPrefs) + " Popularity: " + UserPopularityInterest.ToString();
    }
}
