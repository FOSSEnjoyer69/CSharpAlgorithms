#pragma warning disable

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace CSharpAlgorithms;

public static class Internet
{
    private static IBrowser browser;

    private static bool isInitialized = false;

    public static async Task Init()
    {
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            DefaultViewport = new ViewPortOptions { Width = 1280, Height = 800 }
        });

        
        // Ensure browser closes on program exit or Ctrl+C
        AppDomain.CurrentDomain.ProcessExit += async (sender, e) => await CloseBrowser();
        Console.CancelKeyPress += async (sender, e) =>
        {
            await CloseBrowser();
            e.Cancel = false; // allow the program to exit
        };

        isInitialized = true;
    }

    private static async Task CloseBrowser()
    {
        if (browser != null)
        {
            try
            {
                await browser.CloseAsync();
                browser = null;
                Console.WriteLine("Browser closed.");
            }
            catch
            {
                // ignore errors during shutdown
            }
        }
    }

    /// <summary>
    /// Fetches the HTML content of a given URL and returns it as an HtmlDocument.
    /// </summary>
    /// <param name="url"></param>
    /// <returns>[HtmlAgilityPack.HtmlDocument] page</returns>
    public static async Task<HtmlDocument> GetPage(string url)
    {
        HttpClient client = new HttpClient();
        string html = await client.GetStringAsync(url);
        File.WriteAllText("page.html", html); // Save the HTML to a file for debugging purposes
        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(html);
        return document;
    }

    public static async Task<HtmlDocument> GetPageWithHeadlessBrowser(string url)
    {
        if (!isInitialized)
        {
            await Init();
            isInitialized = true;
        }

        using var page = await browser.NewPageAsync();
        await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

        await page.GoToAsync(url, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Networkidle2]
        });
        string content = await page.GetContentAsync();
        await page.CloseAsync();

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(content);
        return document;
    }
}