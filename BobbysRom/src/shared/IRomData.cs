namespace BobbysRom.Shared
{
    public interface IRomData
    {
        int SizeX { get; set; }
        int SizeZ { get; set; }
        
        byte[] Data { get; set; }
    }
}