public class MenuConsoleService {
    private string folderFiles = "./files/"; 
    private ICrypto iCrypto;
    private IRepositoryKeys iRepositoryKeys;

    private IFileManager iFileManager;
    public MenuConsoleService(ICrypto iCrypto, IRepositoryKeys iRepositoryKeys, IFileManager iFileManager)
    {
        this.iCrypto = iCrypto;
        this.iRepositoryKeys = iRepositoryKeys;
        this.iFileManager = iFileManager;
        this.run();
    }
    
    private void run() 
    {
        string opcion;
        while (true) 
        {
            Console.WriteLine("Menú:");
            Console.WriteLine("1. Crear nuevo par de llaves");
            Console.WriteLine("2. Listar llaves creadas");
            Console.WriteLine("3. Ver llave creada");
            Console.WriteLine("4. Eliminar llave creada");
            Console.WriteLine("5. Firmar mensaje");
            Console.WriteLine("6. Verificar firma");
            Console.WriteLine("0. Salir");
            Console.Write("Ingrese su opción: ");
            
            opcion = Console.ReadLine();
            
            switch (opcion) 
            {
                case "1":
                    generateNewKeys();
                    break;
                case "2":
                    listAllCreatedKeys();
                    break;

                case "3":
                    getCreatedKey();
                    break;  
                case "4":
                    deleteCreatedKey();
                    break ;

                case "5":
                    signMessage();
                    break;
                case "6":
                    validateSign();
                    break;
                case "0":
                    Console.WriteLine("PowerBy, Jorge Abella - 160004300");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Por favor, ingrese una opción válida.");
                    break;
            }
        }
    }

     private void validateSign()
    {
        string nameZip;
        Console.WriteLine($"Informacion: \nLos archivos se buscaran en {iFileManager.GetFolderFiles()}\nLos archivos deben estar en formato .zip\nEl archivo .zip debe contener:\n\t-signature.txt, \n\t-message.txt, \n\t-publicKey.txt");
        Console.Write("Digite el nombre del .zip:\n> ");
        nameZip = Console.ReadLine();

        try
        {
            string extractedFolderPath = Path.Combine(iFileManager.GetFolderFiles(), "extracted");
            iFileManager.ExtractZipFile(nameZip, extractedFolderPath);

            string messageFilePath = Path.Combine(extractedFolderPath, "message.txt");
            string signatureFilePath = Path.Combine(extractedFolderPath, "signature.txt");
            string publicKeyFilePath = Path.Combine(extractedFolderPath, "publicKey.txt");

            string message = File.ReadAllText(messageFilePath);
            string signature = File.ReadAllText(signatureFilePath);
            string publicKey = File.ReadAllText(publicKeyFilePath);

            bool isValid = iCrypto.verifySign(message, publicKey, signature);
            if (isValid)
            {
                Console.WriteLine("La firma es válida.");
            }
            else
            {
                Console.WriteLine("La firma no es válida.");
            }

            Directory.Delete(extractedFolderPath, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al validar la firma: " + ex.Message);
        }
    }

    private void signMessage()
    {
        string signature;
        string message;
        string nameKey;
        KeyModel key;
        Console.Write("Digite el mensaje que desea firmar:\n> ");
        message = Console.ReadLine();
        Console.Write("Digite el nombre de la llave que desea utilizar para firmar:\n> ");
        nameKey = Console.ReadLine();
        if (string.IsNullOrEmpty(nameKey))
        {
            Console.WriteLine("Error al digitar el nombre de la llave");
            return;
        }
        try
        {
            key = iRepositoryKeys.GetKeyByName(nameKey);
            iCrypto.signMessage(message, key.privateKey, out signature);

            // Crear los archivos message.txt, signature.txt y publicKey.txt
            iFileManager.CreateTextFile("message.txt", message);
            iFileManager.CreateTextFile("signature.txt", signature);
            iFileManager.CreateTextFile("publicKey.txt", key.publicKey);

            string[] fileNames = { "message.txt", "signature.txt", "publicKey.txt" };
            string zipFilePath = iFileManager.CreateZipFile(fileNames);

            iFileManager.DeleteFile(fileNames);
            Console.WriteLine($"Se han guardado los archivos firmados en {zipFilePath}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al firmar el mensaje: " + ex.Message);
        }
    }


    private void deleteCreatedKey()
    {
        Console.Write("Digite nombre de la llave creada a eliminar:\n> ");
        string nameKey = Console.ReadLine();

        if (string.IsNullOrEmpty(nameKey))
        {
            Console.WriteLine("Error: El nombre de la llave no puede estar vacío.");
            return;
        }

        try
        {
            // Intentar eliminar la llave del repositorio
            iRepositoryKeys.DeleteKey(nameKey);
            Console.WriteLine($"Llave '{nameKey}' eliminada correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar la llave '{nameKey}': {ex.Message}");
        }
    }

    private void getCreatedKey()
    {
        string nameKey;
        KeyModel key;
        Console.Write("Digite nombre de la llave creada a visualizar:\n> ");
        nameKey = Console.ReadLine();
        if (string.IsNullOrEmpty(nameKey))
        {
            Console.WriteLine("Error al digitar el nombre de la key");
            return;
        }
        try
        {
            key = iRepositoryKeys.GetKeyByName(nameKey);

            Console.WriteLine("INFORMACION DE LA LLAVE: ");
            Console.WriteLine($"nombre: {key.name}\nclave publica:\n{key.publicKey}\nclave privada:\n{key.privateKey}\n");
        }
        catch (Exception ex)
        {   
            Console.WriteLine("Error al obtener informacion de la llave: " + ex.Message);
        }

    }

    private void listAllCreatedKeys()
    {
        var keys = iRepositoryKeys.GetAllKeys();
        if (keys.Any())
        {
            Console.WriteLine("Llaves creadas:");
            foreach (var key in keys)
            {
                Console.WriteLine($" > {key.name}");
            }
        }
        else
        {
            Console.WriteLine("No se han creado llaves todavía.");
        }
    }

    private void generateNewKeys() 
    {
        try 
        {
            // Generar un nuevo par de llaves
            string publicKey, privateKey;
            string name;

            Console.Write("Digite nombre del nuevo par de llaves:\n> ");
            name = Console.ReadLine();
            if (string.IsNullOrEmpty(name)) 
            {
                Console.Write("Error al digitar el nombre");
                return;
            }

            iCrypto.generateKeys(out publicKey, out privateKey);
            
            // Guardar las llaves en el repositorio
            iRepositoryKeys.AddKey(new KeyModel { name = name, publicKey = publicKey, privateKey = privateKey });
            
            Console.WriteLine("Se ha creado un nuevo par de llaves y se ha guardado en el repositorio.");
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Error al generar el par de llaves: " + ex.Message);
        }
    }
}