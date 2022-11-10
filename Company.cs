
using System.Text;
using System;
using ConsoleWEBD;

public static class EditCompany
{
    private static Company company = new Company();
    public static string AddNameCom(string name, long TID)
    {
        company.Hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(10);
        company.Name = name;
        company.Tasks = "0";
        company.Partners = TID.ToString() + ";";
        company.Ended = 0;
        string buf = Requests.Return($"SELECT Company FROM Users WHERE TID = '{TID}'");
        if (buf == "0")
        {
            buf = "";
            buf = buf + company.Hash;
        }
        else
        {
            buf = buf + company.Hash;
        }
        Requests.Update($"UPDATE Users SET Company = '{buf};'");
        Requests.AddCompany(company.Hash, name, company.Tasks, company.Partners, company.Ended);
        return company.Hash;
    }
}

public class Company
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public string Tasks { get; set; }
    public string Partners { get; set; }
    public int Ended { get; set; }
    public string Date { get; set; }
}

