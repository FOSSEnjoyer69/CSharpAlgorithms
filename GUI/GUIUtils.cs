#pragma warning disable

using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using CSharpAlgorithms.Collections;

namespace CSharpAlgorithms.GUI;

public static class GUIUutils
{
    public static ComboBox CreateDropdownFromTemplate(DataTemplate template, string[] options, string selected, StackPanel parent, Action<string> handler)
    {
        ComboBox dropdown = (ComboBox)template.Build(null);
        Setup(dropdown, options, selected, parent, handler);

        return dropdown;
    }
    public static void Setup(ComboBox dropdown, string[] options, string selected, StackPanel parent, Action<string> handler)
    {
        int selectedIndex = 0;

        if (CollectionUtils.TryGetIndex(options, selected, out int index))
            selectedIndex = index;

        dropdown.ItemsSource = options;
        dropdown.SelectedIndex = selectedIndex;
        dropdown.SelectionChanged += (sender, e) =>
        {
            if (dropdown.SelectedItem is string selectedOption)
                handler(selectedOption);
        };

        if (parent is not null)
            parent.Children.Add(dropdown);
    }

    /// <summary>
    /// Sets up a WrapPanel with buttons for each option.
    /// </summary>
    /// <param name="panel">The StackPanel or WrapPanel to fill with buttons.</param>
    /// <param name="options">Array of strings to use as button labels / args.</param>
    /// <param name="onClick">Function to call when a button is clicked, receives the option string.</param>
    public static void SetupButtonWrapPanel(Panel panel, string[] options, Action<string> onClick)
    {
        // Clear existing children
        panel.Children.Clear();

        foreach (var option in options)
        {
            var button = new Button
            {
                Content = option,
                ContextMenu = new ContextMenu(),
                Margin = new Avalonia.Thickness(5),
                Padding = new Avalonia.Thickness(10, 5),
                Tag = option           // store argument in Tag
            };

            button.Click += (sender, e) =>
            {
                if (sender is Button btn && btn.Tag is string arg)
                    onClick(arg);
            };

            panel.Children.Add(button);
        }
    }

    public static ComboBox GetComboBox(StackPanel panel, string name)
    {
        foreach (var child in panel.Children)
        {
            if (child is ComboBox dropdown && dropdown.Name == name)
                return dropdown;
        }

        throw new Exception($"No ComboBox with name {name} found in StackPanel");
    }
}