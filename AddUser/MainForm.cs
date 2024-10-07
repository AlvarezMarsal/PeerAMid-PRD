using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Security.Principal;
using Microsoft.Data.SqlClient;

namespace AddUser;

public partial class MainForm : Form
{
    //public const string emailDomain = "@alvarezandmarsal.com";
    public const string AlvarezMarsal = "ALVAREZMARSAL";
    public const string WindowsDomain = AlvarezMarsal + @"\";
    private static readonly char[] _nameSeperators = [',', ';', '\r', '\n', ' '];

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainFormOnLoad(object sender, EventArgs e)
    {
        var settings = ConfigurationManager.AppSettings;
        var servers = settings["Servers"]!;
        _comboBoxServer.Items.AddRange(servers.Split(';'));
        _comboBoxServer.SelectedIndex = 0;
    }

    private void OkClicked(object sender, EventArgs e)
    {
        var users = SplitUserNames(_textBoxUsers.Text);
        if (users.Length == 0)
        {
            MessageBox.Show(this, "You must enter the user name or email address of at least one user.", "Add User", MessageBoxButtons.OK);
            return;
        }

        var server = _comboBoxServer.Text;
        var connectionString = ConfigurationManager.AppSettings[$"{server}-ConnectionString"]!;

        var remainingUsers = AddUsers(connectionString, users);
        _textBoxUsers.Text = string.Join(Environment.NewLine, remainingUsers);
    }

    private void CloseClicked(object sender, EventArgs e)
    {
        var users = SplitUserNames(_textBoxUsers.Text);
        if (users.Length != 0)
            if (MessageBox.Show(this, "Do you want to close without adding these users?", "Add User", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
        Close();
    }

    private static string[] SplitUserNames(string users)
    {
        return users.Split(_nameSeperators, StringSplitOptions.RemoveEmptyEntries);
    }

    private string[] AddUsers(string connectionString, string[] users)
    {
        var remainingUsers = new List<string>();
        var index = 0;
        SearchResult? results = null;
        try
        {
            using var db = new SqlConnection(connectionString);
            db.Open();
            using var root = new DirectoryEntry($"LDAP://{AlvarezMarsal}");
            for (/**/; index < users.Length; ++index)
            {
                var user = users[index];
                using var searcher = new DirectorySearcher(root);
                //searcher.PropertiesToLoad.Add("adspath");
                var isEmail = user.Contains('@');
                var ldapError = false;
                if (isEmail)
                {
                    var email = user;
                    try
                    {
                        searcher.Filter = $"(&(objectCategory=person)(objectClass=user)(mail={email}))";
                        searcher.PropertiesToLoad.Add("sAMAccountName");
                        results = searcher.FindOne();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("LDAP Error: " + ex.Message);
                        ldapError = true;
                    }

                    if (results == null)
                    {
                        if (!ldapError)
                        {
                            MessageBox.Show(this, $"No user was found with email address {email}.", "Error", MessageBoxButtons.OK);
                            remainingUsers.Add(users[index]);
                            continue;
                        }
                        else
                        {
                            user = email[..email.IndexOf('@')];
                        }
                    }
                    else
                    {
                        var properties = results.Properties;
                        if (properties == null)
                        {
                            MessageBox.Show(this, $"The user with email address {email} has no visible properties", "Error", MessageBoxButtons.OK);
                            remainingUsers.Add(users[index]);
                            continue;
                        }

                        if (!properties.Contains("sAMAccountName"))
                        {
                            MessageBox.Show(this, $"The user with email address {email} has no login", "Error", MessageBoxButtons.OK);
                            remainingUsers.Add(users[index]);
                            continue;

                        }

                        user = properties["sAMAccountName"][0]?.ToString();
                        Debug.WriteLine($"Email address {email} belongs to {user}");
                    }
                }
                else
                {
                    if (user.StartsWith(WindowsDomain, StringComparison.OrdinalIgnoreCase))
                        user = user[WindowsDomain.Length..];

                    searcher.Filter = $"(&(objectCategory=person)(objectClass=user)(sAMAccountName={user}))";
                    //searcher.PropertiesToLoad.Add("mail");
                    results = searcher.FindOne();

                    if (results == null)
                    {
                        MessageBox.Show(this, $"User {user} was not found.", "Error", MessageBoxButtons.OK);
                        remainingUsers.Add(users[index]);
                        continue;
                    }

                    var properties = results.Properties;
                    if (properties == null)
                    {
                        MessageBox.Show(this, $"User {user} has no visible properties", "Error", MessageBoxButtons.OK);
                        remainingUsers.Add(users[index]);
                        continue;
                    }

                    //if (!properties.Contains("mail"))
                    //{
                    //    Console.WriteLine($"User {user} has no email address");
                    //    continue;
                    //}

                    user = properties["sAMAccountName"][0]?.ToString();

                    if (Debugger.IsAttached)
                    {
                        foreach (var name in properties.PropertyNames)
                        {
                            var values = properties[name?.ToString() ?? ""];
                            var v = values[0].ToString();
                            for (var j = 1; j < values.Count; ++j)
                                v += "|" + values[j].ToString();
                            Debug.WriteLine($"{user}.{name} = {v}");
                        }
                    }
                }

                if (!AddUserSql(db, user!))
                {
                    remainingUsers.Add(users[index]);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "LDAP Error: " + ex.Message, "Add User", MessageBoxButtons.OK);
            if (index < users.Length)
                remainingUsers.AddRange(users.Skip(index));
        }

        return [.. remainingUsers];
    }

    private bool AddUserSql(SqlConnection connection, string user)
    {
        // var fullName = $"{WindowsDomain}{user}".ToUpper();
        var userId = GetUserId(connection, $"{WindowsDomain}{user}");
        if (userId != -1)
        {
            MessageBox.Show(this, $"User {user} is already in the database.", "Add User", MessageBoxButtons.OK);
            return true; // remove it from the list of users to be added
        }

        try
        { 
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"[YS].[AddUser]";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User", user);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Add User", MessageBoxButtons.OK);
            return false;
        }

        return true;
    }

    private static int GetUserId(SqlConnection connection, string fullUserName)
    {
        var fullName = fullUserName.ToUpper();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT [UserId] FROM [YS].[RegisteredUser] WHERE UPPER([Username]) = '{fullName}'";
        cmd.CommandType = System.Data.CommandType.Text;
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return -1;
        return reader.GetInt32(0);
    }
}
