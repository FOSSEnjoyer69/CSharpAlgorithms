using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace CSharpAlgorithms.Data;

public static class HTMLUtils
{
    public static HtmlNodeCollection GetNodesWithClass(HtmlDocument page, string className) => page.DocumentNode.SelectNodes($"//*[@class='{className}']");
    public static HtmlNodeCollection GetNodesWithInnerText(HtmlDocument page, string innerText) => page.DocumentNode.SelectNodes($"//*[contains(text(), '{innerText}')]");

    public static void PrintNodesXPathAndInnerText(HtmlNodeCollection nodes)
    {
        if (nodes is null || nodes.Count == 0)
        {
            Console.WriteLine("[CSharpAlgorithms.Data.HTMLUtils.PrintNodesXPathAndInnerText] No nodes were given.");
            return;
        }

        foreach (HtmlNode node in nodes)
        {
            Console.WriteLine($"XPath: {node.XPath}, InnerText: {node.InnerText}");
        }
    }

    public static HtmlNode[] SearchForNodes(HtmlDocument page, SearchForNodeParams searchParams)
    {
        if (page is null || string.IsNullOrEmpty(searchParams.NodeType))
            return [];

        IEnumerable<HtmlNode> nodes = page.DocumentNode.Descendants(searchParams.NodeType);

        if (!string.IsNullOrWhiteSpace(searchParams.InnerText))
        {
            nodes = nodes.Where(n => n.InnerText.Trim() == searchParams.InnerText.Trim());
        }

        return nodes.ToArray();
    }
}

public struct SearchForNodeParams
{
    public SearchForNodeParams() { }

    public string NodeType { get; set; } = "";
    public string InnerText { get; set; } = "";
}