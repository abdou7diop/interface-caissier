using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace projet_css;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(_ => CreateMainWindow().Show());

    public static Window CreateMainWindow() // Change to public static
    {
        var window = new Window
        {
            Title = "Produits Alimentaires",
            Width = 800,
            Height = 600,
            Background = Brushes.Gray
        };

        var mainGrid = new Grid();
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        var border = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(2),
            Width = 300,
            Height = 455,
            Margin = new Thickness(0, 20, 750, 0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top
        };
        Grid.SetColumn(border, 1);

        var borderGrid = new Grid();
        borderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        borderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        borderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        borderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        borderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var produitHeader = new TextBlock
        {
            Text = "Produit :",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10)
        };
        Grid.SetRow(produitHeader, 0);
        Grid.SetColumn(produitHeader, 0);

        var prixHeader = new TextBlock
        {
            Text = "Prix :",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10)
        };
        Grid.SetRow(prixHeader, 0);
        Grid.SetColumn(prixHeader, 1);

        var selectedProductsText = new TextBlock
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10)
        };
        Grid.SetRow(selectedProductsText, 1);
        Grid.SetColumn(selectedProductsText, 0);
        Grid.SetColumnSpan(selectedProductsText, 2);

        var selectedProductsScrollViewer = new ScrollViewer
        {
            Content = selectedProductsText,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(10)
        };
        Grid.SetRow(selectedProductsScrollViewer, 1);
        Grid.SetColumn(selectedProductsScrollViewer, 0);
        Grid.SetColumnSpan(selectedProductsScrollViewer, 2);

        var selectedPriceText = new TextBlock
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10)
        };
        Grid.SetRow(selectedPriceText, 1);
        Grid.SetColumn(selectedPriceText, 1);

        var totalText = new TextBlock
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(10, 10, 890, 10)
        };

        borderGrid.Children.Add(produitHeader);
        borderGrid.Children.Add(prixHeader);
        borderGrid.Children.Add(selectedProductsScrollViewer);
        borderGrid.Children.Add(selectedPriceText);

        var borderScrollViewer = new ScrollViewer
        {
            Content = borderGrid,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(10)
        };

        border.Child = borderScrollViewer;

        var grid = new UniformGrid
        {
            Columns = 3,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10),
        };

        var produits = new ObservableCollection<(string Name, double Price)>
        {
            ("Pomme", 1.0), ("Banane", 0.5), ("Carotte", 0.3), ("Beignet", 1.2), ("Oeuf", 0.2),
            ("Fromage", 2.5), ("Pain", 1.5), ("Lait", 1.0), ("Yaourt", 0.8), ("Poulet", 5.0),
            ("Poisson", 4.0), ("Riz", 1.0), ("Pâtes", 1.2), ("Tomate", 0.7), ("Salade", 0.5),
            ("Orange", 0.6), ("Citron", 0.4), ("Fraise", 2.0), ("Raisin", 2.5), ("Poire", 1.0),
            ("Chocolat", 1.5), ("Café", 3.0), ("Thé", 2.0), ("Sucre", 0.8), ("Sel", 0.2),
            ("Poivre", 0.3), ("Huile", 3.5), ("Beurre", 2.0), ("Miel", 4.0), ("Confiture", 3.0)
        };

        var selectedProducts = new ObservableCollection<(string Name, double Price)>();

        foreach (var produit in produits)
        {
            var button = new Button
            {
                Content = produit.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5),
                Width = 100
            };
            button.Click += (sender, e) =>
            {
                AddProduct(selectedProducts, produit);
                UpdateSelectedProductsText(selectedProducts, selectedProductsText, selectedPriceText);
            };
            grid.Children.Add(button);
        }

        var addProductTextBox = new TextBox
        {
            Watermark = "Ajouter un produit",
            Margin = new Thickness(10)
        };
        var addProductPriceTextBox = new TextBox
        {
            Watermark = "Prix",
            Margin = new Thickness(10)
        };

        var addProductButton = new Button
        {
            Content = "Ajouter",
            Margin = new Thickness(10)
        };

        addProductButton.Click += (sender, e) =>
        {
            if (!string.IsNullOrWhiteSpace(addProductTextBox.Text) && double.TryParse(addProductPriceTextBox.Text, out double price))
            {
                var newProduct = (Name: addProductTextBox.Text, Price: price);
                produits.Add(newProduct);
                var newButton = new Button
                {
                    Content = newProduct.Name,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(5)
                };
                newButton.Click += (s, ev) =>
                {
                    AddProduct(selectedProducts, newProduct);
                    UpdateSelectedProductsText(selectedProducts, selectedProductsText, selectedPriceText);
                };
                grid.Children.Add(newButton);
                addProductTextBox.Text = string.Empty;
                addProductPriceTextBox.Text = string.Empty;
            }
        };

        var addProductPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10)
        };
        addProductPanel.Children.Add(addProductTextBox);
        addProductPanel.Children.Add(addProductPriceTextBox);
        addProductPanel.Children.Add(addProductButton);

        var mainStackPanel = new StackPanel();
        mainStackPanel.Children.Add(addProductPanel);
        mainStackPanel.Children.Add(grid);

        var keypadInputText = new TextBlock
        {
            Text = "",
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var removeProductButton = new Button
        {
            Content = "Effacer",
            Margin = new Thickness(4, 90, 10, 10),
            Width = 100,
            Height = 150,
            Background = Brushes.LightGreen
        };

        removeProductButton.Click += (sender, e) =>
        {
            if (selectedProducts.Count > 0)
            {
                selectedProducts.RemoveAt(selectedProducts.Count - 1);
                UpdateSelectedProductsText(selectedProducts, selectedProductsText, selectedPriceText);
            }
        };

        var printTicketButton = new Button
        {
            Content = "Imprimer \nle \nticket",
            Margin = new Thickness(4, 90, 10, 10),
            Width = 100,
            Height = 150,
            Background = Brushes.LightBlue,
            IsEnabled = false // Initially disabled
        };

        printTicketButton.Click += (sender, e) =>
        {
            var dialog = new Window
            {
                Title = "Information",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = "Ticket imprimé !",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            dialog.ShowDialog(window);
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10)
        };
        buttonPanel.Children.Add(removeProductButton);
        buttonPanel.Children.Add(printTicketButton);

        Grid.SetRow(buttonPanel, 1);
        Grid.SetColumn(buttonPanel, 1);

        mainGrid.Children.Add(buttonPanel);

        var carteButton = new Button
        {
            Content = "Carte",
            Margin = new Thickness(5),
            Width = 100,
            Height = 70,
            Background = Brushes.BlueViolet,
            IsEnabled = false // Initially disabled
        };

        carteButton.Click += (sender, e) =>
        {
            printTicketButton.IsEnabled = true; // Enable the print button
            var dialog = new Window
            {
                Title = "Paiement par Carte",
                Width = 400,
                Height = 200
            };

            var dialogContent = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Vous devez payer : {totalText.Text}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5),
                        Width = 100
                    }
                }
            };

            var okButton = (Button)dialogContent.Children[1];
            okButton.Click += (s, ev) => dialog.Close(); // Close the dialog on "OK" click

            dialog.Content = dialogContent;
            dialog.ShowDialog(window);
        };

        var especesButton = new Button
        {
            Content = "Espèces",
            Margin = new Thickness(5),
            Width = 100,
            Height = 70,
            Background = Brushes.Red,
            IsEnabled = false // Initially disabled
        };

        especesButton.Click += (sender, e) =>
        {
            printTicketButton.IsEnabled = true; // Enable the print button
            var dialog = new Window
            {
                Title = "Paiement en Espèces",
                Width = 400,
                Height = 200
            };

            var dialogContent = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Vous devez payer : {totalText.Text}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5),
                        Width = 100
                    }
                }
            };

            var okButton = (Button)dialogContent.Children[1];
            okButton.Click += (s, ev) => dialog.Close(); // Close the dialog on "OK" click

            dialog.Content = dialogContent;
            dialog.ShowDialog(window);
        };

        var paymentButtonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 15, 500, 0) // Adjusted margin to move left
        };

        paymentButtonsPanel.Children.Add(carteButton);
        paymentButtonsPanel.Children.Add(especesButton);

        Grid.SetRow(paymentButtonsPanel, 0);
        Grid.SetColumn(paymentButtonsPanel, 1);
        mainGrid.Children.Add(paymentButtonsPanel);

        var numericKeypad = CreateNumericKeypad(keypadInputText, totalText, selectedProducts, carteButton, especesButton);
        Grid.SetRow(numericKeypad, 1);
        Grid.SetColumn(numericKeypad, 0);

        mainGrid.Children.Add(numericKeypad);

        Grid.SetRow(totalText, 2);
        Grid.SetColumn(totalText, 0);
        Grid.SetColumnSpan(totalText, 2);
        mainGrid.Children.Add(totalText);

        mainGrid.Children.Add(border); // Ensure the border is added to the main grid
        Grid.SetColumn(border, 1);

        mainGrid.Children.Add(mainStackPanel); // Ensure the product grid is added to the main grid
        Grid.SetRow(mainStackPanel, 0);
        Grid.SetColumn(mainStackPanel, 0);

        window.Content = mainGrid;
        return window;
    }

    private static Button? multiplyButton;

    private static Grid CreateNumericKeypad(TextBlock keypadInputText, TextBlock totalText, ObservableCollection<(string Name, double Price)> selectedProducts, Button carteButton, Button especesButton)
    {
        var keypadGrid = new Grid
        {
            Margin = new Thickness(10)
        };

        for (int i = 0; i < 4; i++)
        {
            keypadGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        for (int i = 0; i < 3; i++)
        {
            keypadGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        }

        var buttons = new[]
        {
            "1", "2", "3",
            "4", "5", "6",
            "7", "8", "9",
            "0", "*", "="
        };

        var quantityBuilder = new StringBuilder();
        int index = 0;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (index >= buttons.Length) break;
                var button = new Button
                {
                    Content = buttons[index],
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 50
                };
                if (buttons[index] == "*")
                {
                    multiplyButton = button;
                }
                button.Click += (sender, e) =>
                {
                    if (sender is Button btn && btn.Content != null)
                    {
                        var content = btn.Content.ToString();
                        if (content == "*")
                        {
                            if (int.TryParse(quantityBuilder.ToString(), out int quantity) && quantity > 0)
                            {
                                SelectedQuantity = quantity;
                                btn.Content = $"{quantity} *";
                                quantityBuilder.Clear();
                            }
                        }
                        else if (content == "=")
                        {
                            double totalPrice = 0;
                            int totalItems = 0;
                            foreach (var product in selectedProducts)
                            {
                                totalPrice += product.Price;
                                totalItems++;
                            }
                            totalText.Text = $"Total : {totalPrice:C}, Articles : {totalItems}";
                            carteButton.IsEnabled = true; // Enable the Carte button
                            especesButton.IsEnabled = true; // Enable the Espèces button
                        }
                        else
                        {
                            quantityBuilder.Append(content);
                        }
                    }
                };
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                keypadGrid.Children.Add(button);
                index++;
            }
        }

        return keypadGrid;
    }

    private static int SelectedQuantity = 1;

    private static void UpdateSelectedProductsText(ObservableCollection<(string Name, double Price)> selectedProducts, TextBlock selectedProductsText, TextBlock selectedPriceText)
    {
        var sb = new StringBuilder();
        double totalPrice = 0;
        foreach (var product in selectedProducts)
        {
            sb.AppendLine($"{product.Name} - {product.Price:C}");
            totalPrice += product.Price;
        }
        selectedProductsText.Text = sb.ToString();
        selectedPriceText.Text = $"Total : {totalPrice:C}";
    }

    private static void AddProduct(ObservableCollection<(string Name, double Price)> selectedProducts, (string Name, double Price) product)
    {
        for (int i = 0; i < SelectedQuantity; i++)
        {
            selectedProducts.Add(product);
        }
        SelectedQuantity = 1;
        if (multiplyButton != null)
        {
            multiplyButton.Content = "*";
        }
    }
}
