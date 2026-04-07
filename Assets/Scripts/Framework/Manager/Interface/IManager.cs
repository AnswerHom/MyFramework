using System;

namespace Framework.Manager
{
    public interface IManager:IDisposable
    {
        public string Name { get; }
    }
}