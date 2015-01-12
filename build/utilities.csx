public int GetBuildNumber()
{
    var context = GetContext();
    var val = context.Environment.GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");
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
    var context = GetContext();
    if(context.Environment.GetEnvironmentVariable("APPVEYOR") != null)
    {
        return "AppVeyor";
    }
    return "Local";
}

public bool IsLocalBuild()
{
    return GetBuildSystemName() == "Local";
}

public bool IsPullRequest()
{
    var context = GetContext();
    var value = context.Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");
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
