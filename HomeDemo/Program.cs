using System;
using System.Linq;
using System.Threading.Tasks;
using HADotNet.Core;
using HADotNet.Core.Clients;

namespace HomeDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //initialize with your Home Assistant instance endpoint and its Access Token
            ClientFactory.Initialize("http://localhost:8123", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIzN2MzN2I5OWRjODk0ODI5ODM1NDU4YzJmMTM1NWFlNCIsImlhdCI6MTU4MjYyNjc4NiwiZXhwIjoxODk3OTg2Nzg2fQ.IXPdutRK_mvBNvhvP7s50SPWcWCdD4kLBunhyeaoBtk");
            var entityClient = ClientFactory.GetClient<EntityClient>();
            var lightEntities = await entityClient.GetEntities("light");
            var statesClient = ClientFactory.GetClient<StatesClient>();
            foreach (var lightEntity in lightEntities)
            {
                var state = await statesClient.GetState(lightEntity);
                Console.WriteLine($"Light {state.Attributes["friendly_name"]} is {state.State}");
            }

            var climateControl = await statesClient.GetState("climate.hvac");
            Console.WriteLine($"Temperature is {climateControl.Attributes["current_temperature"]}");
        }
    }
}
