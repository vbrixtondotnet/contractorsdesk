using System.Runtime.Loader;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public class CustomAssemblyLoadContext : AssemblyLoadContext
	{
		public IntPtr LoadUnmanagedLibrary(string absolutePath)
		{
			return LoadUnmanagedDll(absolutePath);
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			return LoadUnmanagedDllFromPath(unmanagedDllName);
		}

		protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
		{
			return null; // We only handle unmanaged libraries here.
		}
	}
}
