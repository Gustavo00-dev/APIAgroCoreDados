namespace APIAgroCoreDados.Configuracao
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            #region Services/Repository   

            #endregion

            return services;
        }
    }
}
