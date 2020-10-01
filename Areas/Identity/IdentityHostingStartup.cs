using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Models;

[assembly: HostingStartup(typeof(SistemaDeControleDeTCCs.Areas.Identity.IdentityHostingStartup))]
namespace SistemaDeControleDeTCCs.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                
            });
        }
    }
}