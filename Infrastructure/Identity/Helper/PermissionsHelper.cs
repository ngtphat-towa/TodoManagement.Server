﻿using Domain.Enums;

using Newtonsoft.Json;

namespace Identity.Models
{
    public static class UserPermissionHelper
    {
        public static List<string> GetPermissions(string permissionsJson)
        {
            return JsonConvert.DeserializeObject<List<string>>(permissionsJson) ?? [];
        }

        public static void AddPermission(this ApplicationUser user, ControllerPermission controller, ActionPermission action)
        {
            var permission = $"{controller}_{action}";
            var permissionsList = GetPermissions(user.Permissions);
            if (!permissionsList.Contains(permission))
            {
                permissionsList.Add(permission);
                user.Permissions = JsonConvert.SerializeObject(permissionsList);
            }
        }

        public static void RemovePermission(this ApplicationUser user, ControllerPermission controller, ActionPermission action)
        {
            var permission = $"{controller}_{action}";
            var permissionsList = GetPermissions(user.Permissions);
            permissionsList.Remove(permission);
            user.Permissions = JsonConvert.SerializeObject(permissionsList);
        }

        public static bool HasPermission(this ApplicationUser user, ControllerPermission controller, ActionPermission action)
        {
            var permission = $"{controller}_{action}";
            var permissionsList = GetPermissions(user.Permissions);
            return permissionsList.Contains(permission);
        }
        public static List<string> GetPermissionsForRole(Roles role)
        {
            var permissions = new List<string>();

            switch (role)
            {
                case Roles.SuperAdmin:
                    permissions.AddRange(new[]
                    {
                        $"{ControllerPermission.TODO}_{ActionPermission.CREATE}",
                        $"{ControllerPermission.TODO}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.TODO}_{ActionPermission.EDIT}",
                        $"{ControllerPermission.TODO}_{ActionPermission.DELETE}",
                        $"{ControllerPermission.USER}_{ActionPermission.CREATE}",
                        $"{ControllerPermission.USER}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.USER}_{ActionPermission.EDIT}",
                        $"{ControllerPermission.USER}_{ActionPermission.DELETE}"
                        // Add more permissions as needed
                    });
                    break;
                case Roles.Admin:
                    permissions.AddRange(new[]
                    {
                        $"{ControllerPermission.TODO}_{ActionPermission.CREATE}",
                        $"{ControllerPermission.TODO}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.USER}_{ActionPermission.CREATE}",
                        $"{ControllerPermission.USER}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.USER}_{ActionPermission.EDIT}",
                        $"{ControllerPermission.USER}_{ActionPermission.DELETE}"
                        // Add more permissions as needed
                    });
                    break;
                case Roles.Moderator:
                    permissions.AddRange(new[]
                    {
                        $"{ControllerPermission.TODO}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.USER}_{ActionPermission.VIEW}"
                        // Add more permissions as needed
                    });
                    break;
                case Roles.Basic:
                    permissions.AddRange(new[]
                    {
                        $"{ControllerPermission.TODO}_{ActionPermission.VIEW}",
                        $"{ControllerPermission.USER}_{ActionPermission.VIEW}"
                        // Add more permissions as needed
                    });
                    break;
            }

            return permissions;
        }
    }
}