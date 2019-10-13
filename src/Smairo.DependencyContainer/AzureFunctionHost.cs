﻿using System;
using System.IO;
namespace Smairo.DependencyContainer
{
    /// <summary>
    /// Convenient way to get information about where function is running and what is the path where the functions runs
    /// </summary>
    public class AzureFunctionHost : IAzureFunctionHost
    {
        private readonly bool _isRunningInAzure;

        /// <summary>
        /// Default empty ctor that initializes everything that is needed
        /// </summary>
        public AzureFunctionHost()
        {
            // https://stackoverflow.com/questions/45026215/how-to-check-azure-function-is-running-on-local-environment-roleenvironment-i
            var azureSetting = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            _isRunningInAzure = !string.IsNullOrWhiteSpace(azureSetting);
        }

        /// <summary>
        /// Get root of application files
        /// </summary>
        /// <returns></returns>
        public string GetHostRootPath()
        {
            return _isRunningInAzure
                ? @"D:\home\site\wwwroot\"
                : Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Check if we are running now in the Azure service
        /// </summary>
        /// <returns></returns>
        public bool RunsCurrentlyInAzure()
        {
            return _isRunningInAzure;
        }

        /// <summary>
        /// Check if we are running now in the local computer
        /// </summary>
        /// <returns></returns>
        public bool RunsCurrentlyInLocal()
        {
            return !_isRunningInAzure;
        }
    }
}