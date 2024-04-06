public interface IRepositoryKeys
{
    IEnumerable<KeyModel> GetAllKeys();
    KeyModel GetKeyByName(string name);
    void AddKey(KeyModel key);
    void UpdateKey(string name, KeyModel updatedKey);
    void DeleteKey(string name);
}