namespace ReadersConnect.Web.Extensions
{
    public static class AuthorizationPolicies
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("GetUsersPolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanGetUsers");
                });
                options.AddPolicy("GetPermissionsPolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanGetPermissions");
                });
                options.AddPolicy("CreateRolePolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanCreateRole");
                });
                options.AddPolicy("CreatePermissionPolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanCreatePermission");
                });
                options.AddPolicy("EditPermissionPolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanEditPermission");
                });
                options.AddPolicy("DeleteRolePolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanDeleteRole");
                });
                options.AddPolicy("DeletePermissionPolicy", policy =>
                {
                    policy.RequireClaim("Permission", "CanDeletePermission");
                });
            });
        }
    }
}
