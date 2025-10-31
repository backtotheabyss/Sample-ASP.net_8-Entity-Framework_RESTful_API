using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Extensions.Configuration;
using ASP.net_8_Entity_Framework_RESTful_API.Classes.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
public class Settings
{
    /*  settings */
    public string settingName1 { get; set; } = string.Empty;
    public string settingName2 { get; set; } = string.Empty;

    //public string sinologyUsername = string.Empty;
    //public string sinologyPassword = string.Empty;
    //public string sinologyDid = string.Empty;
    //public string sinologySid = string.Empty;
    //public string sinologysinoToken = string.Empty;

    // public IList<SettingsConfiguration> settings { get; set; }
    public SettingsConfiguration settings { get; set; } = new SettingsConfiguration();
    /* end settings */

    public Settings(IConfiguration configuration)    
    {
        /* settings */
        //sinologyUsername = "DFAR_Synology";
        //sinologyPassword = "Test.goal.fork.2";
        //sinologyDid = string.Empty;
        //sinologySid = string.Empty;
        //sinologysinoToken = string.Empty;

        settingName1 = configuration["settingName1"] ?? string.Empty;
        settingName2 = configuration["settingName2"] ?? string.Empty;

        settings = new SettingsConfiguration
        {
            PrinterSettings = configuration
               .GetSection("PrinterSettings")
               .Get<List<PrinterItem>>() ?? new List<PrinterItem>()
        };

        //settings = configuration
        //    .GetSection("ProductSettings")
        //    .Get<List<SettingsConfiguration>>() ?? new List<SettingsConfiguration>();

        /* end settings */

        /* variables - initialization */
        //List<PrinterConfiguration> test = new List<PrinterConfiguration>()
        //    { new PrinterConfiguration { IP = String.Empty },
        //        new PrinterConfiguration { IP = String.Empty}
        //};
        //PrinterConfiguration x = new PrinterConfiguration() { Name = "Canon MF240 Serices PCL6 V4", IP = "172.22.1.242", Port = 9100, web_interface_url = "portal_top.html", health_url = "", health_regex = "" };
        //List<string> y = new List<string>() { };
        //string[] z = new string[] { };
    }
    
    public static async Task<bool> PurgePrinterViaPowerShell(string printerName)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Get-PrintJob -PrinterName '{printerName}' | Remove-PrintJob -Confirm:$false\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"PowerShell failed for {printerName}. StdErr: {stderr}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while purging printer '{printerName}': {ex.Message}");
            return false;
        }
    }
}