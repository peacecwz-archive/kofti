using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kofti.Models;

namespace Kofti.Services
{
    public interface IConfigService
    {
        T GetValue<T>(string key, T defaultValue = default);
        Task PublishAsync(string applicationName, Dictionary<string, object> configurations);
        void InitClient();
        void Load();
        void InitServer(Action<KoftiInitMessage> action);
    }
}