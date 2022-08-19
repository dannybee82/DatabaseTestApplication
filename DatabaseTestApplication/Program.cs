using DatabaseTestApplicatie.DatabaseMethods;
using DatabaseTestApplicatie.OutputMethods;
using System;
using System.Data.SqlClient;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

/*
 * Demo Application Database Test in Console.
 * 
 * Using Simple Queries: SELECT, INSERT INTO, UPDATE, DELETE
 * using System.Data.SqlClient;
 * 
 * See: sql_queries.txt to insert the Demo Database.
 * 
 */

/*
 * Connection to Database.
 * 
 */

const string yourUsername = "";
const string yourPassword = "";

string connectionString = "Server=localhost;Database=eternal_pizza;Trusted_Connection=True;User ID=" + yourUsername + ";Password=" + yourPassword + ";";

SqlConnection cnn;

/*
 * Other classes in use.
 * 
 */ 

DatabaseMethods databaseMethods = new();
OutputMethodsConsole outputMethodsConsole = new();
RepeatingMenus repeatingMenus = new();

/*
 * Other fields, etc. 
 * 
 */

const string mainTableName = "Pizzas";

string[] mainMenuOptions = new string[] { "Select Column", "Insert Pizza", "Update Pizza", "Delete Pizza", "Exit" };

string[] availableColumns = new string[] { "Id", "Name", "Price", "Spiciness", "Description" };

string[] availableColumnsChoices = new string[] { "All", "Id", "Name", "Price", "Spiciness", "Description" };

string[] otherMenuOptions = new string[] { "Back to Main Menu", "Exit" };

string[] insertNewItemQuestionts = new string[] { "Enter name of Pizza:", "Enter Price: (xx,xx)", "Enter Spiciness", "Enter Description" };
byte[] insertNewItemTypes = new byte[]          { 0,                        2,                      1,                     0 };

string[] allowedColumnsToUpdate = new string[] { "Name", "Price", "Spiciness", "Description" };
byte[] updateTypes = new byte[] { 0, 2, 1, 0 };

/*
 * Showing menu's, etc.
 * 
 */

bool applicationRunning = true;

bool showMainMenu = true;
bool showSelectMenu = false;
bool showInsertMenu = false;

bool showUpdateMenu = false;
bool chooseColumn = false;
bool chooseNewValue = false;

bool showDeleteMenu = false;

bool showAllResults = true;
bool showSingleColumnResults = false;
bool showBackToMainMenuOrExit = false;

string[,]? allRecords;
string[]? singleColumnRecords = null;

int updateThisId = -1;
string updateThisColumn = "";
int updateChosen = -1;
string updateValue = "";

/*
 * Set console title.
 * 
 */

Console.Title = "Database Test";

/*
 * Run the application in a try ... catch block.
 * 
 */

try
{
    //Open connection.
    cnn = new SqlConnection(connectionString);
    cnn.Open();

    Console.WriteLine( "Connection Open" );
    Console.WriteLine("Total Records:" + databaseMethods.CountRecords(cnn, mainTableName));
    Console.WriteLine("All Records:");

    //Get all records.
    allRecords = databaseMethods.SelectAllRecords(cnn, mainTableName, availableColumns.Length);

    while (applicationRunning)
    {
        //Show main menu.
        if (showMainMenu)
        {
            ShowRecords();

            int choice = repeatingMenus.RepeatMenu("Menu Items:", mainMenuOptions, 1, 5);

            if (choice != 3) {
                Console.Clear();
            }

            GotoNextMenu(choice);
        }

        //Show menu: back to main menu, or exit.
        if (showBackToMainMenuOrExit)
        {
            ShowRecords();

            int choice = repeatingMenus.RepeatMenu("Menu Items:", otherMenuOptions, 1, 2);

            Console.Clear();
            BackToMainMenuOrExit(choice);
        }

        //Show Options for SELECT.
        if (showSelectMenu)
        {
            ShowRecords();

            int choice = repeatingMenus.RepeatMenu("Menu Items:", availableColumnsChoices, 1, 6);

            Console.Clear();
            SelectItems(choice);
        }

        //Show Options for INSERT.
        if (showInsertMenu)
        {
            ShowRecords();

            string[] values = new string[availableColumns.Length];

            for(int i = 0; i < insertNewItemQuestionts.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine(insertNewItemQuestionts[i]);

                object? input = repeatingMenus.RepeatInput(insertNewItemTypes[i]);
                string derivedValue = repeatingMenus.DeriveValue(input, insertNewItemTypes[i]);

                //Note: skip first index == id.
                values[i + 1] = derivedValue;               
            }

            Console.Clear();
            PrepareInsert(values);            
        }

        //Show Options for UPDATE.
        if (showUpdateMenu)
        {            
            int maxId = databaseMethods.GetMaxId(cnn, mainTableName);
            int choice = repeatingMenus.RepeatMenu("Choose Pizza to Update by Id:", null, 1, maxId);

            updateThisId = choice;
            chooseColumn = true;
            showUpdateMenu = false;
        }

        //Other menu for UPDATE.
        if (chooseColumn)
        {
            int choice = repeatingMenus.RepeatMenu("Which Column do you want to update?", allowedColumnsToUpdate, 1, 4);
            updateThisColumn = allowedColumnsToUpdate[choice - 1];
            updateChosen = choice - 1;

            chooseColumn = false;
            chooseNewValue = true;
        }

        //Other menu for UPDATE.
        if (chooseNewValue)
        {
            Console.WriteLine("Enter new Value:");
            object? input = repeatingMenus.RepeatInput(updateTypes[updateChosen]);
            updateValue = repeatingMenus.DeriveValue(input, updateTypes[updateChosen]);

            chooseNewValue = false;
            PrepareUpdate();
            Console.Clear();
        }

        //Show Options for Delete.
        if (showDeleteMenu)
        {
            ShowRecords();

            int maxId = databaseMethods.GetMaxId(cnn, mainTableName);
            int choice = repeatingMenus.RepeatMenu("Choose Pizza-Id to Delete:", null, 1, maxId);

            Console.Clear();
            PrepareDelete(choice);            
        }

    }        
    
    //Below: close connection.
    cnn.Close();

    //Exit Application at this point.
    Environment.Exit(0);
}
catch(Exception ex)
{
    Console.WriteLine( ex.Message );
}

/*
 * Below: private methods.
 * 
 */

void GotoNextMenu(int menu)
{
    allMenusToFalse();

    switch (menu)
    {
        case 0: showMainMenu = true; break;
        case 1: showSelectMenu = true; break;
        case 2: showInsertMenu = true; break;
        case 3: showUpdateMenu = true; break;
        case 4: showDeleteMenu = true; break;
    }
    
    if(showUpdateMenu || showDeleteMenu)
    {
        showAllResults = true;
    }

    if(menu == 5)
    {
        ExitApplication();
    }
}

void BackToMainMenuOrExit(int menu)
{
    allMenusToFalse();

    if (menu == 1)
    {
        showBackToMainMenuOrExit = false;
        showMainMenu = true;
        showAllResults = true;
    }
    
    if(menu == 2)
    {
        ExitApplication();
    }
}

void ShowRecords()
{
    //Show all results in arr[,]
    if (showAllResults)
    {
        if (allRecords != null)
        {
            outputMethodsConsole.ShowColumnNames(availableColumns);
            outputMethodsConsole.ShowData2D(allRecords);
        }
    }

    //Show specific results in arr[]
    if (showSingleColumnResults)
    {
        if (singleColumnRecords != null)
        {
            outputMethodsConsole.ShowData(singleColumnRecords);
        }
    }
}

void allMenusToFalse()
{
    showMainMenu = false;
    showSelectMenu = false;
    showInsertMenu = false;
    showUpdateMenu = false;
    showAllResults = false;
    showSingleColumnResults = false;
    showBackToMainMenuOrExit = false;
    showDeleteMenu = false;
}

void SelectItems(int choice)
{
    allMenusToFalse();

    showBackToMainMenuOrExit = true;

    if(choice == 1)
    {
        showAllResults = true;
        allRecords = databaseMethods.SelectAllRecords(cnn, mainTableName, availableColumns.Length);
    }
    else
    {
        showSingleColumnResults = true;
        string select = availableColumnsChoices[(choice - 1)];
        singleColumnRecords = databaseMethods.SelectSpecificRecords(cnn, mainTableName, select);

        Console.WriteLine("Results for: " + select);
    }    
}

void PrepareInsert(string[] values)
{
    allMenusToFalse();
    showAllResults = true;
    showBackToMainMenuOrExit = true;

    databaseMethods.InsertRecord(cnn, mainTableName, availableColumns, values);

    allRecords = databaseMethods.SelectAllRecords(cnn, mainTableName, availableColumns.Length);
}

void PrepareUpdate()
{
    allMenusToFalse();
    showAllResults = true;
    showBackToMainMenuOrExit = true;

    databaseMethods.UpdateRecord(cnn, mainTableName, updateThisColumn, updateValue, updateThisId);

    allRecords = databaseMethods.SelectAllRecords(cnn, mainTableName, availableColumns.Length);    
}

void PrepareDelete(int id)
{
    allMenusToFalse();
    showAllResults = true;
    showBackToMainMenuOrExit = true;

    Console.WriteLine("Deleting Pizza with Id: " + id);

    databaseMethods.DeleteRecordById(cnn, mainTableName, id);

    allRecords = databaseMethods.SelectAllRecords(cnn, mainTableName, availableColumns.Length);
}

void ExitApplication()
{
    applicationRunning = false;
}