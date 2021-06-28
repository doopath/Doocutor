namespace Doocutor.Core.Caches
{
    internal interface ICache<T>
    {
        int Limit { get; set; }
        int Size { get;  }
        void Cache(string key, T value);
        bool HasKey(string key);
        T GetValue(string key);
        void Clean();
    }
}