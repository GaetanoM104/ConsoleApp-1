using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class AppData
{
    public decimal Balance {get; set;}
    public List<Deposit> Deposits {get;set;} = new List<Deposit>();
    public List<Expense> Expenses {get;set;} = new List<Expense>();

    public AppData(decimal balance, List<Deposit> deposits, List<Expense> expenses)
    {
        Balance = balance;
        Deposits = deposits;
        Expenses = expenses;
    }
    
    public void SaveFile(string fileName)
    {
        string toJson = JsonSerializer.Serialize(this);
        File.WriteAllText(fileName, toJson);
    }
}

class InputHelper
{
    public static int intInput()
    {
        string? input = Console.ReadLine();
        int number;
        while(!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid number, try again.");
                input = Console.ReadLine();
            }

        return number;
    }
    
    public static decimal decimalInput()
    {
        string? input = Console.ReadLine();
        decimal number;
        while(!decimal.TryParse(input, out number) || number <= 0)
            {
                Console.WriteLine("Invalid number, try again.");
                input = Console.ReadLine();
            }

        return number;
    }

    public static string stringInput()
    {
        string? word = Console.ReadLine();
        decimal a;
        while(string.IsNullOrEmpty(word) || decimal.TryParse(word, out a))
        {
            Console.WriteLine("Input can't be null or a number, try again.");
            word = Console.ReadLine();
        }

        return word;
    }
}

class Transaction
{
    public DateTime date {get;set;}
    public decimal amount {get;set;}
    public string? type {get;set;}
}

class Deposit : Transaction
{
    public decimal addBalance(decimal balance, decimal number, string kind)
    {
        amount = number;
        type = kind;
        date = DateTime.Now;
        
        balance += amount;
        return balance;
    }

    public void printDetails()
    {
        Console.Write(date.ToString("dd/MM/yyyy HH:mm:ss | "));
        Console.WriteLine($"{amount} of {type} gained.");
    }
}


class Expense : Transaction
{
    public decimal addExpense(decimal balance, decimal number, string kind)
    {
        amount = number;
        type = kind;
        date = DateTime.Now;
        
        balance -= amount;
        return balance;
    }

    public void printDetails()
    {
        Console.Write(date.ToString("dd/MM/yyyy HH:mm:ss | "));
        Console.WriteLine($"{amount} of {type} spent.");
    }
}



class Program
{
    static void Main(string[] args)
    {
        List<Deposit> deposits = new List<Deposit>();
        List<Expense> expenses = new List<Expense>();
        decimal balance = 0;
        int option;
        string fileName = "data.json";

        if (File.Exists(fileName))
        {
            string jsonString = File.ReadAllText(fileName);
            AppData? data = JsonSerializer.Deserialize<AppData>(jsonString);

            if(data != null)
            {
                balance = data.Balance;
                deposits = data.Deposits;
                expenses = data.Expenses;
            }
        }

        do
        {
            Console.WriteLine("------");
            Console.WriteLine($"Current balance: {balance}");
            Console.WriteLine("Select which operation you want to perform:");
            Console.WriteLine("-1 . Delete save and exit");
            Console.WriteLine("0 . Exit");
            Console.WriteLine("1 . Deposit");
            Console.WriteLine("2 . Add expenses");
            Console.WriteLine("3 . Show expenses and total spent");
            Console.WriteLine("4 . Show deposit and total deposited");
            Console.WriteLine("5 . Search for a specific deposit");
            Console.WriteLine("6 . Search for a specific expense");
            Console.WriteLine("7 . Order deposits by amount");
            Console.WriteLine("8 . Order expenses by amount");
            Console.WriteLine("9 . Delete a deposit");
            Console.WriteLine("10 . Delete an expense");
            Console.WriteLine("------");

            string? input = Console.ReadLine();
            while(!int.TryParse(input, out option))
            {
                Console.WriteLine("Invalid input. Try again.");
                input = Console.ReadLine();
            }

            switch (option)
            {
                case -1:
                    if(File.Exists(fileName)) File.Delete(fileName);
                    Console.WriteLine("Save deleted. Exiting the program...");
                    break;
                
                case 0:
                    Console.WriteLine("Exiting the program...");
                    SaveData(fileName, balance, deposits, expenses);
                    break;

                case 1:
                    AddDeposit(deposits, ref balance);
                    SaveData(fileName, balance, deposits, expenses);
                    break;

                case 2:
                    AddExpense(expenses, ref balance);
                    SaveData(fileName, balance, deposits, expenses);
                    break;

                case 3:
                    ShowExpenses(expenses);
                    break;

                case 4:
                    ShowDeposits(deposits);
                    break;

                case 5:
                    SearchDeposit(deposits);
                    break;

                case 6:
                    SearchExpense(expenses);
                    break;

                case 7:
                    OrderDeposits(deposits);
                    break;

                case 8:
                    OrderExpenses(expenses);
                    break;

                case 9:
                    DeleteDeposit(deposits, ref balance);
                    SaveData(fileName, balance, deposits, expenses);
                    break;

                case 10:
                    DeleteExpense(expenses, ref balance);
                    SaveData(fileName, balance, deposits, expenses);
                    break;

                default:
                    Console.WriteLine("Invalid option. Try again");
                    break;
            }
        } while(option != 0 && option != -1);
    }

    static void SaveData(string fileName, decimal balance, List<Deposit> deposits, List<Expense> expenses)
    {
        AppData newData = new AppData(balance, deposits, expenses);
        newData.SaveFile(fileName);
    }

    static void ShowExpenses(List<Expense> expenses)
    {
        Console.WriteLine("Expenses:");
        decimal totalSpent = 0;
        foreach(var expense in expenses)
        {
            expense.printDetails();
            totalSpent += expense.amount;
        }
        Console.WriteLine($"Total spent: {totalSpent}");
    }

    static void ShowDeposits(List<Deposit> deposits)
    {
        Console.WriteLine("Deposits:");
        decimal totalDeposited = 0;
        foreach(var deposit in deposits)
        {
        deposit.printDetails();
        totalDeposited += deposit.amount;
        }
        Console.WriteLine($"Total deposited: {totalDeposited}");
    }

    static void AddDeposit(List<Deposit> deposits, ref decimal balance)
    {
        deposits.Add(new Deposit());
        Console.WriteLine("Enter the amount to deposit:");
        decimal amount = InputHelper.decimalInput();
        Console.WriteLine("Enter the type of deposit:");
        string type = InputHelper.stringInput();
        balance = deposits[^1].addBalance(balance, amount, type);
        Console.WriteLine($"{amount} of {type} added to your balance.");
        Console.WriteLine($"Balance: {balance}€.");
    }

    static void AddExpense(List<Expense> expenses, ref decimal balance)
    {
        Console.WriteLine("Enter the amount of the expence:");
        decimal amount = InputHelper.decimalInput();
        Console.WriteLine("Enter the type of the expence:");
        string type = InputHelper.stringInput();
        expenses.Add(new Expense());
        balance = expenses[^1].addExpense(balance, amount, type);
        Console.WriteLine($"{amount} of {type} removed from your balance.");
        Console.WriteLine($"Balance: {balance}.");
    }

    static void SearchDeposit(List<Deposit> deposits)
    {
        Console.WriteLine("Type the type of the deposit you want to see:");
        string? typeD = InputHelper.stringInput();
        decimal typeDeposited = 0;
        int nD = 0;
        foreach(var deposit in deposits)
        {
            if(deposit.type?.ToLower() == typeD.ToLower())
            {
                typeDeposited += deposit.amount;
                nD++;
            }
        }
        if(nD == 0) Console.WriteLine($"There are no transactions of {typeD}");
        else Console.WriteLine($"You have deposited {typeDeposited} of {typeD}");
    }

    static void SearchExpense(List<Expense> expenses)
    {
        Console.WriteLine("Type the type of the expense you want to see:");
        string? typeE = InputHelper.stringInput();
        decimal typeExpense = 0;
        int nE = 0;
        foreach(var expense in expenses)
        {
            if(expense.type?.ToLower() == typeE.ToLower())
            {
                typeExpense += expense.amount;
                nE++;
            }
        }
        if(nE == 0) Console.WriteLine($"There are no transactions of {typeE}");
        else Console.WriteLine($"You have spent {typeExpense} of {typeE}");
    }

    static void OrderDeposits(List<Deposit> deposits)
    {
        if(deposits.Count != 0)
        {
            int i = 1;
            var orderDeposits = deposits.OrderByDescending(x => x.amount).ToList();
            foreach(var deposit in orderDeposits)
            {
                Console.Write($"{i} . ");
                deposit.printDetails();
                i++;
            }
        } else Console.WriteLine("There arent any deposits");
    }

    static void OrderExpenses(List<Expense> expenses)
    {
        if(expenses.Count != 0)
        {
            int i = 1;
            var orderExpenses = expenses.OrderByDescending(x => x.amount).ToList();
            foreach(var expense in orderExpenses)
            {
                Console.Write($"{i} . ");
                expense.printDetails();
                i++;
            }
        } else Console.WriteLine("There arent any expenses");
    }

    static void DeleteDeposit(List<Deposit> deposits, ref decimal balance)
    {
        if(deposits.Count != 0){
            Console.WriteLine("Deposits:");
            int i = 1;
            foreach(var deposit in deposits)
            {
                Console.Write($"{i} . ");
                deposit.printDetails();
                i++;
            }
            Console.WriteLine("Choose which deposit to delete");
            int option = InputHelper.intInput()-1;
            if(option >= deposits.Count || option < 0) Console.WriteLine("Number of the deposit not valid.");
            while(option < 0 || option >= deposits.Count)
            {
                option = InputHelper.intInput()-1;
                if(option >= deposits.Count || option < 0) Console.WriteLine("Number of the deposit not valid.");
            } 
            balance -= deposits[option].amount;
            deposits.RemoveAt(option);
            Console.WriteLine($"Deposit number {option+1} removed.");
        } else Console.WriteLine("There arent any deposits");
    }

    static void DeleteExpense(List<Expense> expenses, ref decimal balance)
    {
        if(expenses.Count != 0){
            Console.WriteLine("Expense:");
            int i = 1;
            foreach(var expense in expenses)
            {
                Console.Write($"{i} . ");
                expense.printDetails();
                i++;
            }
            Console.WriteLine("Choose which expense to delete");
            int option = InputHelper.intInput()-1;
            if(option >= expenses.Count || option < 0) Console.WriteLine("Number of the expense not valid.");
            while(option < 0 || option >= expenses.Count)
            {
                option = InputHelper.intInput()-1;
                if(option >= expenses.Count || option < 0) Console.WriteLine("Number of the expense not valid.");
            } 
            balance += expenses[option].amount;
            expenses.RemoveAt(option);
            Console.WriteLine($"Expense number {option+1} removed.");
        } else Console.WriteLine("There arent any expenses");
    }
}