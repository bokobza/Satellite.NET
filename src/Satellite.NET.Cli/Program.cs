using System;
using System.Threading.Tasks;
using Satellite.NET.Models;

namespace Satellite.NET.Cli
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                SatelliteApi api = new SatelliteApi(true);
                var pendingOrders = await api.GetPendingOrders();

                Console.WriteLine($"The last 20 pending orders are: ");
                foreach (var order in pendingOrders)
                {
                    Console.WriteLine($"{order.Uuid}");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
