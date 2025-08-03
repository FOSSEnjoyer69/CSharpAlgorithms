#pragma warning disable

using System;
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

        isInitialized = true;
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

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(content);
        return document;
    }
}