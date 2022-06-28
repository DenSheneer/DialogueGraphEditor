using System.IO;

public static class DGE_Utilities
{
    public static void EnsureExistingResourcesFolder()
    {
        if (!Directory.Exists("Assets"))
        {
            Directory.CreateDirectory("Assets");
        }
        if (!Directory.Exists("Assets/Resources"))
        {
            Directory.CreateDirectory("Assets/Resources");
        }
    }
    public static void EnsureExistingActorsFolder()
    {
        EnsureExistingResourcesFolder();

        if (!Directory.Exists("Assets/Resources/Actors"))
        {
            Directory.CreateDirectory("Assets/Resources/Actors");
        }
    }
    public static void EnsureExistingConversationsFolder()
    {
        EnsureExistingResourcesFolder();


        Directory.CreateDirectory("Assets/Resources/Conversations");

    }
}
