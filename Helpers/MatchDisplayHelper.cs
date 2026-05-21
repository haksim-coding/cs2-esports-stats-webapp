using cs2_esports.Models;

namespace cs2_esports.Helpers;

public static class MatchDisplayHelper
{
    public static string GetFormatLabel(MatchFormat format)
    {
        return format switch
        {
            MatchFormat.BestOf1 => "Best Of 1",
            MatchFormat.BestOf3 => "Best Of 3",
            MatchFormat.BestOf5 => "Best Of 5",
            _ => format.ToString()
        };
    }

    public static string GetFormatBadgeLabel(MatchFormat format)
    {
        return format switch
        {
            MatchFormat.BestOf1 => "BO1",
            MatchFormat.BestOf3 => "BO3",
            MatchFormat.BestOf5 => "BO5",
            _ => format.ToString()
        };
    }

    public static string GetMapLabel(MapPool map)
    {
        return map switch
        {
            MapPool.Ancient => "Ancient",
            MapPool.Mirage => "Mirage",
            MapPool.Inferno => "Inferno",
            MapPool.Anubis => "Anubis",
            MapPool.Nuke => "Nuke",
            MapPool.Dust2 => "Dust 2",
            MapPool.Cache => "Cache",
            _ => map.ToString()
        };
    }
}