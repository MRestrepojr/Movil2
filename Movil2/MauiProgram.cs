using Movil2.Data;
using System.IO;

namespace Movil2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "productos.db3");
        builder.Services.AddSingleton(new ProductoDatabase(dbPath));

        return builder.Build();
    }
}
