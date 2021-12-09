using System;

public class ReadSPIFlash : SwitchJoyConBaseSubcommand
{
    public override byte SubcommandID => (byte)SwitchJoyConSubcommandID.SPIFlashRead;

    public uint Address = 0x0;
    public byte Length = 0x0;

    protected override byte[] GetArguments()
    {
        var addrAsBytes = BitConverter.GetBytes(Address);

        byte[] output = new byte[5];

        Array.Copy(addrAsBytes, output, 4);
        output[4] = Length;
        return output;
    }

    public ReadSPIFlash(uint atAddress = 0x0, byte withLength = 0x1)
    {
        Address = atAddress;
        Length = withLength;
    }
}