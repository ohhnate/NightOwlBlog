﻿namespace SimpleBlogMVC.Services.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan duration);
        void Remove(string key);
        bool TryGet<T>(string key, out T value);
    }
}