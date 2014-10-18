using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace TimeTracker.Backend.Models
{
    public class TimeTrackerContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Wokator.Models.WokatorContext>());

        public TimeTrackerContext()
            : base("TimeTrackerContext")
        {
            DefaultSchema = ConfigurationManager.AppSettings["DefaultSchema"];
        }

        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityCategorization> ActivityCategorizations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Goal> Goals { get; set; }

        public string DefaultSchema { get; set; }
        private string GetTableName(Type type)
        {
            var tableAttribute = type.GetCustomAttributes(false).OfType<System.ComponentModel.DataAnnotations.Schema.TableAttribute>().FirstOrDefault();
            return tableAttribute == null ? type.Name : tableAttribute.Name;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<WokatorContext, WokatorConfiguration>());
            //Database.SetInitializer<LockscreenAdsContext>(new DropCreateDatabaseIfModelChanges<LockscreenAdsContext>());

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

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);

        }

    }
}
