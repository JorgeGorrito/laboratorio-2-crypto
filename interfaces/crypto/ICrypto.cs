public interface ICrypto {
    void generateKeys( out string publicKey, out string privateKey );
    void signMessage(string message, string privateKey, out string signature );
    bool verifySign( string message, string signature, string publicKey);
}