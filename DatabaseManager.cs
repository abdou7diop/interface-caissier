using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace projet_css;

public class DatabaseManager
{
    private MySqlConnection? dbConnection;

    public void InitializeDatabase()
    {
        string connectionString = "Server=localhost;Database=walid_css;User ID=root;Password=;"; // Mettez à jour les détails de connexion
        dbConnection = new MySqlConnection(connectionString);

        try
        {
            dbConnection.Open();
            Console.WriteLine("Database connection established.");
            CreateTables();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to the database: {ex.Message}");
        }
    }

    public void CloseDatabase()
    {
        if (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open)
        {
            dbConnection.Close();
            Console.WriteLine("Database connection closed.");
        }
    }

    private void CreateTables()
    {
        if (dbConnection == null) return;

        try
        {
            string createCashiersTable = @"
                CREATE TABLE IF NOT EXISTS cashiers (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            string createProductsTable = @"
                CREATE TABLE IF NOT EXISTS products (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    price DOUBLE NOT NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

            string createTicketsTable = @"
                CREATE TABLE IF NOT EXISTS tickets (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    details TEXT NOT NULL,
                    total_price DOUBLE NOT NULL,
                    total_items INT NOT NULL,
                    cashier_id INT,
                    is_printed BOOLEAN DEFAULT FALSE, -- Nouvelle colonne pour indiquer si le ticket est imprimé
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (cashier_id) REFERENCES cashiers(id)
                );";

            using var command = new MySqlCommand(createCashiersTable, dbConnection);
            command.ExecuteNonQuery();

            command.CommandText = createProductsTable;
            command.ExecuteNonQuery();

            command.CommandText = createTicketsTable;
            command.ExecuteNonQuery();

            Console.WriteLine("Tables created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating tables: {ex.Message}");
        }
    }

    public List<(string Name, double Price)> GetProducts()
    {
        var products = new List<(string Name, double Price)>();

        if (dbConnection == null) return products;

        try
        {
            Console.WriteLine("Retrieving products from the database...");
            string query = "SELECT DISTINCT name, price FROM products"; // Ajout de DISTINCT
            using var command = new MySqlCommand(query, dbConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString("name");
                double price = reader.GetDouble("price");
                products.Add((name, price));
                Console.WriteLine($"Product retrieved: {name} - {price}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving products: {ex.Message}");
        }

        return products;
    }

    public void AddCashier(string name)
    {
        if (dbConnection == null) return;

        try
        {
            string query = "INSERT INTO cashiers (name) VALUES (@name)";
            using var command = new MySqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@name", name);
            command.ExecuteNonQuery();
            Console.WriteLine("Cashier added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding cashier: {ex.Message}");
        }
    }

    public void AddProduct(string name, double price)
    {
        if (dbConnection == null) return;

        try
        {
            string query = "INSERT INTO products (name, price) VALUES (@name, @price)";
            using var command = new MySqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@price", price);
            command.ExecuteNonQuery();
            Console.WriteLine("Product added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product: {ex.Message}");
        }
    }

    public void AddTicket(string details, double totalPrice, int totalItems, int cashierId)
    {
        if (dbConnection == null) return;

        try
        {
            string query = "INSERT INTO tickets (details, total_price, total_items, cashier_id) VALUES (@details, @totalPrice, @totalItems, @cashierId)";
            using var command = new MySqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@details", details);
            command.Parameters.AddWithValue("@totalPrice", totalPrice);
            command.Parameters.AddWithValue("@totalItems", totalItems);
            command.Parameters.AddWithValue("@cashierId", cashierId);
            command.ExecuteNonQuery();
            Console.WriteLine("Ticket added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding ticket: {ex.Message}");
        }
    }

    public void MarkTicketAsPrinted(int ticketId)
    {
        if (dbConnection == null) return;

        try
        {
            string query = "UPDATE tickets SET is_printed = TRUE WHERE id = @ticketId";
            using var command = new MySqlCommand(query, dbConnection);
            command.Parameters.AddWithValue("@ticketId", ticketId);
            command.ExecuteNonQuery();
            Console.WriteLine($"Ticket with ID {ticketId} marked as printed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error marking ticket as printed: {ex.Message}");
        }
    }

    public List<(int Id, string Details, double TotalPrice, int TotalItems, int CashierId, bool IsPrinted)> GetTickets(bool onlyUnprinted = false)
    {
        var tickets = new List<(int Id, string Details, double TotalPrice, int TotalItems, int CashierId, bool IsPrinted)>();

        if (dbConnection == null) return tickets;

        try
        {
            Console.WriteLine("Retrieving tickets from the database...");
            string query = "SELECT id, details, total_price, total_items, cashier_id, is_printed FROM tickets";
            if (onlyUnprinted)
            {
                query += " WHERE is_printed = FALSE";
            }

            using var command = new MySqlCommand(query, dbConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string details = reader.GetString("details");
                double totalPrice = reader.GetDouble("total_price");
                int totalItems = reader.GetInt32("total_items");
                int cashierId = reader.GetInt32("cashier_id");
                bool isPrinted = reader.GetBoolean("is_printed");

                tickets.Add((id, details, totalPrice, totalItems, cashierId, isPrinted));
                Console.WriteLine($"Ticket retrieved: ID {id}, Printed: {isPrinted}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving tickets: {ex.Message}");
        }

        return tickets;
    }
}
