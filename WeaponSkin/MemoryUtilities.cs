using Iced.Intel;

namespace WeaponSkin;

internal class MemoryUtilities
{
    public static unsafe (bool IsRead, bool IsWrite) AnalyzeInstructionAccess(nint instructionAddress)
    {
        var memSpan   = new ReadOnlySpan<byte>((void*) instructionAddress, 15);
        var codeBytes = memSpan.ToArray();

        var codeReader = new ByteArrayCodeReader(codeBytes);
        var decoder    = Decoder.Create(64, codeReader);
        decoder.IP = (ulong) instructionAddress;

        var instruction = decoder.Decode();

        if (instruction.IsInvalid)
        {
            return (false, false);
        }

        var infoFactory = new InstructionInfoFactory();
        var info        = infoFactory.GetInfo(instruction);

        var isRead  = false;
        var isWrite = false;

        foreach (var memUse in info.GetUsedMemory())
        {
            switch (memUse.Access)
            {
                case OpAccess.Read:
                case OpAccess.CondRead:
                    isRead = true;

                    break;
                case OpAccess.Write:
                case OpAccess.CondWrite:
                    isWrite = true;

                    break;
                case OpAccess.ReadWrite:
                    isRead  = true;
                    isWrite = true;

                    break;
            }
        }

        return (isRead, isWrite);
    }
}