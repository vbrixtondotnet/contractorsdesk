namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class SessionManagementMiddleware
    {
        public static void ConfigureSession(this IServiceCollection services)
		{
            services.AddDistributedMemoryCache();  // Use in-memory cache for session storage
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout
                options.Cookie.HttpOnly = true;  // Prevent client-side JavaScript from accessing session cookie
                options.Cookie.IsEssential = true;  // Mark session cookie as essential
            });
        }
    }
}
