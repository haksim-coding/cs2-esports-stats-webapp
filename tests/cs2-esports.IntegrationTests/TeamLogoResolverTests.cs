using cs2_esports.Helpers;

namespace cs2_esports.IntegrationTests;

public class TeamLogoResolverTests
{
    [Theory]
    [InlineData("The MongolZ", "MGLZ", "~/images/teams/the-mongolz.png")]
    [InlineData("Team Liquid", "TL", "~/images/teams/Team_Liquid.svg")]
    [InlineData("Aurora", "AUR", "~/images/teams/aurora.svg")]
    [InlineData("HEROIC", "HERO", "~/images/teams/heroic.png")]
    [InlineData("FURIA", "FUR", "~/images/teams/furia.svg")]
    public void GetLogoPath_MapsNewTeamAssets(string teamName, string teamTag, string expectedPath)
    {
        Assert.Equal(expectedPath, TeamLogoResolver.GetLogoPath(teamName, teamTag));
    }

    [Theory]
    [InlineData("MGLZ", "~/images/teams/the-mongolz.png")]
    [InlineData("Liquid", "~/images/teams/Team_Liquid.svg")]
    [InlineData("AUR", "~/images/teams/aurora.svg")]
    [InlineData("HERO", "~/images/teams/heroic.png")]
    [InlineData("FUR", "~/images/teams/furia.svg")]
    public void GetLogoPath_RecognizesCommonNameAndTagAliases(string alias, string expectedPath)
    {
        Assert.Equal(expectedPath, TeamLogoResolver.GetLogoPath(alias, null));
    }
}
