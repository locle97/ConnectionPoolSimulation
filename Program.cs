using Dumpify;

Parallel.For(0, 10, i =>
{
    using (var connection = new DatabaseConnection("Server=.;Database=MyDatabase;User Id=sa"))
    {
        // Open connection
        connection.Connect();

        // Execute query
        connection.Execute($"SELECT {i} FROM MyTable");

        // Close connection
        connection.Disconnect();
    }

    "------------------------------".Dump();
});


public class DatabaseConnection : IDisposable
{
    private string _connectionString;
    private string _connectionId = String.Empty;
    private static Dictionary<string, Queue<string>> _connections = new Dictionary<string, Queue<string>>();

    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect()
    {
        "Connecting to database...".Dump();
        if (_connections.TryGetValue(_connectionString, out var queue) && queue.Count > 0)
        {
            _connectionId = queue.Dequeue();
            $"Reusing connection {_connectionId}".Dump();
            return;
        }

        _connectionId = CreateConnection();
    }

    private string CreateConnection()
    {
        return Guid.NewGuid().ToString();
    }

    public void Execute(string query)
    {
        $"Executing query {query}...".Dump();
    }

    public void Disconnect()
    {
        $"Disconnecting {_connectionId} from database...".Dump();
        if (_connections.TryGetValue(_connectionString, out var queue))
        {
            queue.Enqueue(_connectionId);
        }
        else
        {
            _connections.Add(_connectionString, new Queue<string>(new[] { _connectionId }));
        }
    }

    public void Dispose()
    {
    }
}
