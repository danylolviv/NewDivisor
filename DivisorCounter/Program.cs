using System.Diagnostics;
using RestSharp;

public class Program
{
    private const string BaseUrl = "http://cache-service/";
    private static RestClient _restClient = new RestClient(BaseUrl);

    public static async Task Main()
    {
        var first = 1_000_000_000;
        var last = 1_000_000_020;

        var numberWithMostDivisors = first;
        var result = 0;

        var watch = Stopwatch.StartNew();
        for (var i = first; i <= last; i++)
        {
            var innerWatch = Stopwatch.StartNew();

            var divisorCounter = await _restClient.GetAsync<int>(new RestRequest("cache?number=" + i));

            if (divisorCounter == 0)
            {
                for (var divisor = 1; divisor <= i; divisor++)
                {
                    if (i % divisor == 0)
                    {
                        divisorCounter++;
                    }
                }
                _restClient.PostAsync(new RestRequest($"cache?number={i}&divisorCounter={divisorCounter}"));
            }
            
            innerWatch.Stop();
            Console.WriteLine("Counted " + divisorCounter + " divisors for " + i + " in " + innerWatch.ElapsedMilliseconds + "ms");

            if (divisorCounter > result)
            {
                numberWithMostDivisors = i;
                result = divisorCounter;
            }
        }
        watch.Stop();
        
        Console.WriteLine("The number with most divisors inside range is: " + numberWithMostDivisors + " with " + result + " divisors.");
        Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds + "ms");
        Console.ReadLine();
    }
}