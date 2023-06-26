using DrugLordManager.Data;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace DrugLordManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
			app.MapRazorPages();


            Acc�sDonn�es.SqlConnectionString = "Server=druglordpg.postgres.database.azure.com;Database=druglordmanager;Port=5432;User Id=drugadmin;Password=<password>;Ssl Mode=Require;Trust Server Certificate=true";


			app.Run();
        }
    }
}