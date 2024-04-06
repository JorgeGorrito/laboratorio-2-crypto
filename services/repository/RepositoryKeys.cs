using System.Collections.Generic;
using System.Data.SQLite;

public class RepositoryKeys : IRepositoryKeys
{
    private readonly string _connectionString;

    public RepositoryKeys(string connectionString)
    {
        _connectionString = connectionString;
        
    }

    private void CreateDatabase()
    {
        // Leer el contenido del archivo db.sql
        string script;
        using (StreamReader reader = new StreamReader("./data/db.sql"))
        {
            script = reader.ReadToEnd();
        }

        // Conectar a la base de datos
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            
            // Ejecutar el script SQL para crear las tablas y otros objetos
            using (SQLiteCommand command = new SQLiteCommand(script, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public IEnumerable<KeyModel> GetAllKeys()
    {
        List<KeyModel> keys = new List<KeyModel>();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM keys", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        keys.Add(new KeyModel
                        {
                            name = reader.GetString(0),
                            publicKey = reader.GetString(1),
                            privateKey = reader.GetString(2)
                        });
                    }
                }
            }
        }
        return keys;
    }

    public KeyModel GetKeyByName(string name)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM keys WHERE name = @Name", connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new KeyModel
                        {
                            name = reader.GetString(0),
                            publicKey = reader.GetString(1),
                            privateKey = reader.GetString(2)
                        };
                    }
                }
            }
        }
        return null;
    }

    public void AddKey(KeyModel key)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("INSERT INTO keys (name, publicKey, privateKey) VALUES (@Name, @PublicKey, @PrivateKey)", connection))
            {
                command.Parameters.AddWithValue("@Name", key.name);
                command.Parameters.AddWithValue("@PublicKey", key.publicKey);
                command.Parameters.AddWithValue("@PrivateKey", key.privateKey);
                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateKey(string name, KeyModel updatedKey)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("UPDATE keys SET publicKey = @PublicKey, privateKey = @PrivateKey WHERE name = @Name", connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@PublicKey", updatedKey.publicKey);
                command.Parameters.AddWithValue("@PrivateKey", updatedKey.privateKey);
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteKey(string name)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("DELETE FROM keys WHERE name = @Name", connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }
    }
}