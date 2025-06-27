
public class Account
{
    public int id;

    public string username;

    public string password;

    public string ipserver;

    public Account(int id, string u, string p, string ipServer)
    {
        this.id = id;
        username = u;
        password = p;
        ipserver = ipServer;
    }
}