using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Linq;
using System.Reflection;

namespace TimeTracker.Backend.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("UsersContext")
        {
            DefaultSchema = ConfigurationManager.AppSettings["UsersSchema"];
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public string DefaultSchema { get; set; }
        private string GetTableName(Type type)
        {
            var tableAttribute = type.GetCustomAttributes(false).OfType<System.ComponentModel.DataAnnotations.Schema.TableAttribute>().FirstOrDefault();
            return tableAttribute == null ? type.Name : tableAttribute.Name;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Default schema for the database
            if (!String.IsNullOrEmpty(DefaultSchema))
            {
                var entityMethod = modelBuilder.GetType().GetMethod("Entity");
                foreach (PropertyInfo dbSet in GetType().GetProperties().Where(t => t.PropertyType.IsGenericType && t.PropertyType.GetGenericTypeDefinition().Equals(typeof(DbSet<>))))
                {
                    var entityType = dbSet.PropertyType.GetGenericArguments();
                    var entityMethodGeneric = entityMethod.MakeGenericMethod(entityType);
                    var entityConfig = entityMethodGeneric.Invoke(modelBuilder, null);
                    var toTableMethod = entityConfig.GetType().GetMethod("ToTable", new Type[] { typeof(string), typeof(string) });
                    var tableName = GetTableName(entityType.FirstOrDefault());
                    toTableMethod.Invoke(entityConfig, new object[] { tableName, DefaultSchema });
                }
            }

            base.OnModelCreating(modelBuilder);

        }
    }

    public class UsersContextInit : DropCreateDatabaseIfModelChanges<UsersContext>
    {
        protected override void Seed(UsersContext context)
        {
            WebSecurity.InitializeDatabaseConnection("UsersContext",
                "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AuthToken { get; set; }
        public Guid? ConsumerId { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Extened user registration model for admins
    /// </summary>
    public class RegisterModelExt
    {
        [Required]
        [Display(Name = "RoleName")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterModelCustomer
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int Sex { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string BirthDate { get; set; }
        public int? Age { get; set; }
        public string Occupation { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
