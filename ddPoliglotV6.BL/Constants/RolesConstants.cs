using System;
using System.Collections.Generic;
using System.Text;

namespace ddPoliglotV6.BL.Constants
{
    public static class RolesConstants
    {
        public static string SuperAdminRoleName { get; set; } = "SuperAdmin";
        public static string AdminRoleName { get; set; } = "Admin";
        public static string LessonsMakerRoleName { get; set; } = "LessonsMaker";
        public static string UserRoleName { get; set; } = "User";
        public static string RoleNamesForSuperAdmin { get; set; } = "SuperAdmin";
        public static string RoleNamesForAdmin { get; set; } = "Admin, SuperAdmin";
        public static string RoleNamesForLessonsMaker = "LessonsMaker, Admin, SuperAdmin";
        public static string RoleNamesForUser { get; set; } = "User, LessonsMaker, Admin, SuperAdmin";
    }
}
