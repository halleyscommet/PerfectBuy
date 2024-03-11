using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PerfectBuy;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void CalculateButton_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        Console.WriteLine(button.ClickMode);

        int.TryParse(InputField.Text, out var money);
        int.TryParse(CrewField.Text, out var crewmates);

        var flashlights = FlashCheck.IsChecked == true;
        var minimizeSpending = MinimizeSpendingCheck.IsChecked == true;
        var jetpacks = JetpacksCheck.IsChecked == true;
        var walkie = WalkiesCheck.IsChecked == true;

        var itemChooser = new ItemChooser();

        // Load the values from the JSON file
        var values = itemChooser.LoadJson();

        var (itemsToBuy, remainingMoney) =
            itemChooser.ChooseItems(values, crewmates, money, flashlights, minimizeSpending, jetpacks, walkie);

        var output = new StringBuilder();
        output.AppendLine("Items to buy:");
        foreach (var (item, count) in itemsToBuy) output.AppendLine($"- {item}: {count}");
        output.AppendLine($"Remaining money: {remainingMoney}");

        Result.Text = output.ToString();
    }
}