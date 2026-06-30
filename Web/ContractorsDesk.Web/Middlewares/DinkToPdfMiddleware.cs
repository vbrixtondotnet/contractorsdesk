using DinkToPdf.Contracts;
using DinkToPdf;
using System.Runtime.InteropServices;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class DinkToPdfSetup
	{
		public static IServiceCollection AddDinkToPdf(this IServiceCollection services)
		{

			// Set the library path
			var libraryPath = Path.Combine(Directory.GetCurrentDirectory(), "DinkToPDF", "NativeBinaries/libwkhtmltox.dll");

			// Ensure the library can be found
			if (!File.Exists(libraryPath))
			{
				throw new FileNotFoundException("libwkhtmltox.dll not found. Ensure it is in the correct folder.", libraryPath);
			}

			// Load the library manually if needed
			var context = new CustomAssemblyLoadContext();
			context.LoadUnmanagedLibrary(libraryPath);

			services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
			return services;
		}
	}
}
