using System.Text.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");


var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesSummaryDir = Path.Combine(currentDirectory, "salesSummaryDir");
Directory.CreateDirectory(salesSummaryDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

var overallSalesFiles = FindFiles(storesDirectory);

var overallTotal = GenerateSalesSummary(overallSalesFiles);


File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");
File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"Sales Summary{Environment.NewLine}");
File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"------------------------------------{Environment.NewLine}");


File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"  Total Sales: {overallTotal.ToString("C2")}{Environment.NewLine}");
File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"{Environment.NewLine}");
File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"  Details:{Environment.NewLine}");


IEnumerable<string> files = Directory.EnumerateFiles("stores", "*salestotals.json", SearchOption.AllDirectories);


// Loop through each sales file
foreach (var file in files)
{
    // Read the corresponding sales data
    string salesJson = File.ReadAllText(file);
    OverallSalesData? data = JsonSerializer.Deserialize<OverallSalesData?>(salesJson);


    // Get the total for the specific store from the JSON data
    double storeTotal = data?.OverallTotal ?? 0;
    // Console.WriteLine(storeTotal);

    // Format the store total in the desired currency format ($xxx,xxx.xx)
    string formattedStoreTotal = storeTotal.ToString("C2");

    // Append the formatted result to the sales summary file
    File.AppendAllText(Path.Combine(salesSummaryDir, "salesSummary.txt"), $"     {Path.GetFileName(file)}: {formattedStoreTotal} {Environment.NewLine}");
}



IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);

        }
    }

    return salesFiles;
}

double GenerateSalesSummary(IEnumerable<string> overallSalesFiles) {

    double overallSalesTotal = 0;

    foreach (var file in overallSalesFiles) {

        string overalSalesJson = File.ReadAllText(file);
        OverallSalesData? data = JsonSerializer.Deserialize<OverallSalesData?>(overalSalesJson);
        overallSalesTotal += data?.OverallTotal ?? 0;
    }

    return overallSalesTotal;
}


double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonSerializer.Deserialize<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

record SalesData(double Total);

record OverallSalesData(double OverallTotal);
