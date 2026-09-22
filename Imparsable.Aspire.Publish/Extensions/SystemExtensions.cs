namespace Imparsable.Aspire.Publish.Extensions;

internal static class SystemExtensions
{
    extension(Directory)
    {
        public static void DeleteIfExists(string name)
        {
            if (Directory.Exists(name))
                Directory.Delete(name, recursive: true);
        }
    }
}