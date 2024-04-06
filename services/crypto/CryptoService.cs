using System.Security.Cryptography;
public class CryptoService : ICrypto 
{
    static string ToXmlString(RSAParameters rsaParameters)
    {
        using (var sw = new System.IO.StringWriter())
        {
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, rsaParameters);
            return sw.ToString();
        }
    }

    public void generateKeys(out string publicKey, out string privateKey) 
    {
        // Crear una instancia de la clase RSACryptoServiceProvider
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            // Generar el par de claves pública y privada
            RSAParameters privateKeyRSA = rsa.ExportParameters(true);
            RSAParameters publicKeyRSA = rsa.ExportParameters(false);
            publicKey = ToXmlString(publicKeyRSA);
            privateKey = ToXmlString(privateKeyRSA);
        }
    }

    public void signMessage(string message, string privateKey, out string signature) 
    {
        using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);   
            signature = Convert.ToBase64String(signatureBytes);
        }
    }

    public bool verifySign(string message, string publicKeyXml, string signature) 
    {
        // Crear una instancia de la clase RSACryptoServiceProvider
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            // Cargar la clave pública
            rsa.FromXmlString(publicKeyXml);

            // Convertir el mensaje a bytes
            byte[] mensajeBytes = System.Text.Encoding.UTF8.GetBytes(message);

            // Convertir la firma desde base64 a bytes
            byte[] firmaBytes = Convert.FromBase64String(signature);

            // Verificar la firma
            bool isValid = rsa.VerifyData(mensajeBytes, firmaBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return isValid;
        }
    }
}