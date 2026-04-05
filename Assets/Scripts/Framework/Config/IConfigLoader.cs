using Luban;

namespace Framework.Config
{
    public interface IConfigLoader
    {
        ByteBuf LoadOffset(string fileName);
        ByteBuf LoadByteBuf(string fileName, int offset, int length);
    }
}