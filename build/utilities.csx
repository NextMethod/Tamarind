public int GetBuildNumber()
{
    var val = EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
    if (val != null)
    {
        int version = 0;
        if (int.TryParse(val, out version))
        {
            return version;
        }
    }
    return 0;
}

public string GetBuildSystemName()
{
    return IsLocalBuild()
        ? "Local"
        : "AppVeyor";
}

public bool IsLocalBuild()
{
    return !HasEnvironmentVariable("APPVEYOR");
}

public bool IsPullRequest()
{
    var value = EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");
    if(value != null)
    {
        int number = 0;
        if(int.TryParse(value, out number))
        {
            return number > 0;
        }
    }
    return false;
}

public bool IsReleaseBuild()
{
    return HasEnvironmentVariable("APPVEYOR_REPO_TAG");
}
