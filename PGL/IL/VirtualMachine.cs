using System.Text;
using PGL.Core;

namespace PGL.IL;

public class VirtualMachine
{
    private Memory<byte>[] _registers;
    private Memory<byte> _mainMemory;

    public VirtualMachine(Configuration configuration, uint memorySize)
    {
        var registers = Enum.GetValues<EILRegister>();
        _registers = new Memory<byte>[registers.Length];
        _mainMemory = new Memory<byte>(new byte[memorySize]);
        for (var i = 0; i < registers.Length; i++)
            _registers[i] = new Memory<byte>(new byte[configuration.TargetPlatformInstructionSizeBytes]);
    }

    public string PrintRegisters()
    {
        var sb = new StringBuilder();
        
        for (var i = 0; i < _registers.Length; i++)
        {
            var reg = (EILRegister)i;
            sb.AppendLine($"{reg,-12} {Convert.ToHexString(_registers[i].Span)}");
        }

        return sb.ToString();
    }

    public void ExecuteProgram(List<ILInstruction> instructions)
    {
        for (var i = 0; i < instructions.Count; i++)
        {
            var rip = GetU64From(EILRegister.RIP);
            ExecuteInstruction(instructions[i]);
            Store(EILRegister.RIP, rip + 1);
        }
    }

    public void ExecuteInstruction(ILInstruction instruction)
    {
        switch (instruction.Instruction)
        {
            case EILInstruction.Function:
                break;
            
            case EILInstruction.Return:
                break;
            
            case EILInstruction.Mov:
                ExecuteMov(instruction);
                break;
#region ALU
            case EILInstruction.Addi:
                ExecuteAddi(instruction);
                break;
            
            case EILInstruction.Addu:
                ExecuteAddu(instruction);
                break;
            
            case EILInstruction.Addf:
                ExecuteAddf(instruction);
                break;
            
            case EILInstruction.Subi:
                ExecuteSubi(instruction);
                break;
            
            case EILInstruction.Subu:
                ExecuteSubu(instruction);
                break;
            
            case EILInstruction.Subf:
                ExecuteSubf(instruction);
                break;
            
            case EILInstruction.Muli:
                ExecuteMuli(instruction);
                break;
            
            case EILInstruction.Mulu:
                ExecuteMulu(instruction);
                break;
            
            case EILInstruction.Mulf:
                ExecuteMulf(instruction);
                break;
            
            case EILInstruction.Divi:
                ExecuteDivi(instruction);
                break;
            
            case EILInstruction.Divu:
                ExecuteDivu(instruction);
                break;
            
            case EILInstruction.Divf:
                ExecuteDivf(instruction);
                break;
#endregion ALU
            
            
            case EILInstruction.Nop:
                break;
            
            default:
                throw new Exception($"Virtual Machine failed: unable to handle unexpected instruction: {instruction.Instruction}");
        }
    }

    public void ExecuteMov(ILInstruction instruction)
    {
        Span<byte> source = stackalloc byte[8];
        GetBytesFrom(source, instruction.RightOperand);

        switch (instruction.LeftOperand)
        {
            case ILRelativeAddressOperand relativeAddressOperand:
                var regValue = GetU64From(relativeAddressOperand.Register);
                var offset = relativeAddressOperand.Offset;
                var address = offset > 0 ? regValue + (ulong)offset : regValue - (ulong)(~offset);
                Store(_mainMemory.Span.Slice((int)address, 8), source);
                break;
            
            case ILRegisterOperand register:
                Store(_registers[(int)register.Register].Span, source);
                break;
            
            default:
                throw new Exception($"Virtual Machine failed: unexpected move operand: {instruction.RightOperand}");
        }
    }
    
    #region Addition

    public void ExecuteAddi(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetI8From(instruction.LeftOperand);
                var right = GetI8From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetI16From(instruction.LeftOperand);
                var right = GetI16From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetI32From(instruction.LeftOperand);
                var right = GetI32From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetI64From(instruction.LeftOperand);
                var right = GetI64From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute addi instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteAddu(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetU8From(instruction.LeftOperand);
                var right = GetU8From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetU16From(instruction.LeftOperand);
                var right = GetU16From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetU32From(instruction.LeftOperand);
                var right = GetU32From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetU64From(instruction.LeftOperand);
                var right = GetU64From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute addu instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteAddf(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 4:
            {
                var left = GetF32From(instruction.LeftOperand);
                var right = GetF32From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetF64From(instruction.LeftOperand);
                var right = GetF64From(instruction.RightOperand);

                var result = left + right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute addf instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    #endregion Addition

    #region Subtraction

    public void ExecuteSubi(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetI8From(instruction.LeftOperand);
                var right = GetI8From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetI16From(instruction.LeftOperand);
                var right = GetI16From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetI32From(instruction.LeftOperand);
                var right = GetI32From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetI64From(instruction.LeftOperand);
                var right = GetI64From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute subi instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteSubu(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetU8From(instruction.LeftOperand);
                var right = GetU8From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetU16From(instruction.LeftOperand);
                var right = GetU16From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetU32From(instruction.LeftOperand);
                var right = GetU32From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetU64From(instruction.LeftOperand);
                var right = GetU64From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute subu instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteSubf(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 4:
            {
                var left = GetF32From(instruction.LeftOperand);
                var right = GetF32From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetF64From(instruction.LeftOperand);
                var right = GetF64From(instruction.RightOperand);

                var result = left - right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute subf instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    #endregion Subtaction
    
    #region Multiplication

    public void ExecuteMuli(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetI8From(instruction.LeftOperand);
                var right = GetI8From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetI16From(instruction.LeftOperand);
                var right = GetI16From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetI32From(instruction.LeftOperand);
                var right = GetI32From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetI64From(instruction.LeftOperand);
                var right = GetI64From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute muli instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }

    public void ExecuteMulu(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetU8From(instruction.LeftOperand);
                var right = GetU8From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetU16From(instruction.LeftOperand);
                var right = GetU16From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetU32From(instruction.LeftOperand);
                var right = GetU32From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetU64From(instruction.LeftOperand);
                var right = GetU64From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute mulu instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteMulf(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 4:
            {
                var left = GetF32From(instruction.LeftOperand);
                var right = GetF32From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetF64From(instruction.LeftOperand);
                var right = GetF64From(instruction.RightOperand);

                var result = left * right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute mulf instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    #endregion Multiplication
    
    #region Division

    public void ExecuteDivi(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetI8From(instruction.LeftOperand);
                var right = GetI8From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetI16From(instruction.LeftOperand);
                var right = GetI16From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetI32From(instruction.LeftOperand);
                var right = GetI32From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetI64From(instruction.LeftOperand);
                var right = GetI64From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute divi instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteDivu(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 1:
            {
                var left = GetU8From(instruction.LeftOperand);
                var right = GetU8From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);
                
                break;
            }

            case 2:
            {
                var left = GetU16From(instruction.LeftOperand);
                var right = GetU16From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 4:
            {
                var left = GetU32From(instruction.LeftOperand);
                var right = GetU32From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetU64From(instruction.LeftOperand);
                var right = GetU64From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute divu instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    public void ExecuteDivf(ILInstruction instruction)
    {
        switch (instruction.ByteSize)
        {
            case 4:
            {
                var left = GetF32From(instruction.LeftOperand);
                var right = GetF32From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            case 8:
            {
                var left = GetF64From(instruction.LeftOperand);
                var right = GetF64From(instruction.RightOperand);

                var result = left / right;

                Store(instruction.DestinationRegister, result);

                break;
            }
            
            default:
                throw new Exception($"Cannot execute divf instruction: unsupported byte size: {instruction.ByteSize}");
        }
    }
    
    #endregion Division

    #region MemoryRetrieval

    public void GetBytesFrom(Span<byte> target, ILOperand operand)
    {
        switch (operand)
        {
            case ILRelativeAddressOperand relativeAddressOperand:
                GetBytesFrom(target, relativeAddressOperand);
                break;
            
            case ILRegisterOperand register:
                GetBytesFrom(target, register);
                break;
            
            case ILImmediateIntegerValueOperand integer:
                GetBytesFrom(target, integer);
                break;
            
            case ILImmediateFloatValueOperand floater:
                GetBytesFrom(target, floater);
                break;
            
            default:
                throw new Exception("Virtual machine failure: unexpected operand");
        }
    }

    public void GetBytesFrom(Span<byte> target, ILImmediateIntegerValueOperand operand)
    {
        switch (operand.ByteSize)
        {
            case 1:
                if (operand.Signed)
                    target[0] = byte.Parse(operand.Literal);
                else
                    target[0] = byte.Parse(operand.Literal);
                break;
            
            case 2:
                if (operand.Signed)
                    BitConverter.TryWriteBytes(target, short.Parse(operand.Literal));
                else
                    BitConverter.TryWriteBytes(target, ushort.Parse(operand.Literal));
                break;
                
            case 4:
                if (operand.Signed)
                    BitConverter.TryWriteBytes(target, int.Parse(operand.Literal));
                else
                    BitConverter.TryWriteBytes(target, uint.Parse(operand.Literal));
                break;
                
            case 8:
                if (operand.Signed)
                    BitConverter.TryWriteBytes(target, long.Parse(operand.Literal));
                else
                    BitConverter.TryWriteBytes(target, ulong.Parse(operand.Literal));
                break;
                
            default:
                throw new Exception($"Virtual machine failed: unexpected byte size for operand: {operand.ByteSize}");
        }
    }

    public void GetBytesFrom(Span<byte> target, ILImmediateFloatValueOperand operand)
    {
        switch (operand.ByteSize)
        {
            case 4:
                BitConverter.TryWriteBytes(target, float.Parse(operand.Literal));
                break;
                
            case 8:
                BitConverter.TryWriteBytes(target, double.Parse(operand.Literal));
                break;
                
            default:
                throw new Exception($"Virtual machine failed: unexpected byte size for operand: {operand.ByteSize}");
        }
    }

    public void GetBytesFrom(Span<byte> target, ILRegisterOperand operand) => _registers[(int)operand.Register].Span.CopyTo(target);

    public void GetBytesFrom(Span<byte> target, ILRelativeAddressOperand operand)
    {
        var regValue = GetU64From(operand.Register);
        var offset = operand.Offset;
        var address = offset > 0 ? regValue + (ulong)offset : regValue - (ulong)(~offset);
        _mainMemory.Span.Slice((int)address, target.Length).CopyTo(target);
    }

    public byte GetI8From(EILRegister register) => _registers[(int)register].Span[0];
    public byte GetI8From(ILOperand operand)
    {
        /* todo ahodder@praethos.com 10/16/23: Not signed atm */
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return byte.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return _mainMemory.Span[offset + immediate.Offset];
            
            case ILRegisterOperand register:
                return _registers[(int)register.Register].Span[0];
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }

    public short GetI16From(EILRegister register) => BitConverter.ToInt16(_registers[(int)register].Span.Slice(0, 2));
    public short GetI16From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return short.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToInt16(_mainMemory.Span.Slice(offset + immediate.Offset, 2));
            
            case ILRegisterOperand register:
                return BitConverter.ToInt16(_registers[(int)register.Register].Span.Slice(0, 2));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public int GetI32From(EILRegister register) => BitConverter.ToInt32(_registers[(int)register].Span.Slice(0, 4));
    public int GetI32From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return int.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToInt32(_mainMemory.Span.Slice(offset + immediate.Offset, 4));
            
            case ILRegisterOperand register:
                return BitConverter.ToInt32(_registers[(int)register.Register].Span.Slice(0, 4));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public long GetI64From(EILRegister register) => BitConverter.ToInt64(_registers[(int)register].Span.Slice(0, 8));
    public long GetI64From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return long.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToInt64(_mainMemory.Span.Slice(offset + immediate.Offset, 8));
            
            case ILRegisterOperand register:
                return BitConverter.ToInt64(_registers[(int)register.Register].Span.Slice(0, 8));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }

    public byte GetU8From(EILRegister register) => _registers[(int)register].Span[0];
    public byte GetU8From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return byte.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return _mainMemory.Span[offset + immediate.Offset];
            
            case ILRegisterOperand register:
                return _registers[(int)register.Register].Span[0];
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public ushort GetU16From(EILRegister register) => BitConverter.ToUInt16(_registers[(int)register].Span.Slice(0, 2));
    public ushort GetU16From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return ushort.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToUInt16(_mainMemory.Span.Slice(offset + immediate.Offset, 2));
            
            case ILRegisterOperand register:
                return BitConverter.ToUInt16(_registers[(int)register.Register].Span.Slice(0, 2));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public uint GetU32From(EILRegister register) => BitConverter.ToUInt32(_registers[(int)register].Span.Slice(0, 4));
    public uint GetU32From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return uint.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToUInt32(_mainMemory.Span.Slice(offset + immediate.Offset, 4));
            
            case ILRegisterOperand register:
                return BitConverter.ToUInt32(_registers[(int)register.Register].Span.Slice(0, 4));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public ulong GetU64From(EILRegister register) => BitConverter.ToUInt64(_registers[(int)register].Span.Slice(0, 8));
    public ulong GetU64From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateIntegerValueOperand integer:
                return ulong.Parse(integer.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToUInt64(_mainMemory.Span.Slice(offset + immediate.Offset, 8));
            
            case ILRegisterOperand register:
                return BitConverter.ToUInt64(_registers[(int)register.Register].Span.Slice(0, 8));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public float GetF32From(EILRegister register) => BitConverter.ToSingle(_registers[(int)register].Span.Slice(0, 4));
    public float GetF32From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateFloatValueOperand floater:
                return float.Parse(floater.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToSingle(_mainMemory.Span.Slice(offset + immediate.Offset, 4));
            
            case ILRegisterOperand register:
                return BitConverter.ToSingle(_registers[(int)register.Register].Span.Slice(0, 4));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    public double GetF64From(EILRegister register) => BitConverter.ToDouble(_registers[(int)register].Span.Slice(0, 8));
    public double GetF64From(ILOperand operand)
    {
        switch (operand)
        {
            case ILImmediateFloatValueOperand floater:
                return float.Parse(floater.Literal);
            
            case ILRelativeAddressOperand immediate:
                var reg = _registers[(int)immediate.Register].Span;
                var offset = BitConverter.ToInt32(reg.Slice(0, 4));
                return BitConverter.ToDouble(_mainMemory.Span.Slice(offset + immediate.Offset, 8));
            
            case ILRegisterOperand register:
                return BitConverter.ToDouble(_registers[(int)register.Register].Span.Slice(0, 8));
            
            default:
                throw new Exception($"Cannot get byte value from operand: unexpected operand: {operand}");
        }
    }
    
    #endregion

    #region MemoryStorage

    public void Store(Span<byte> destination, Span<byte> source) => source.CopyTo(destination);
    
    public void Store(ulong memoryAddress, byte data) => _mainMemory.Span[(int)memoryAddress] = data;

    public void Store(EILRegister destinationRegister, byte data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        registerData[0] = data;
    }
    
    public void Store(ulong memoryAddress,short data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 2), data);
    public void Store(EILRegister destinationRegister, short data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,int data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 4), data);
    public void Store(EILRegister destinationRegister, int data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,long data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 8), data);
    public void Store(EILRegister destinationRegister, long data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,ushort data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 2), data);
    public void Store(EILRegister destinationRegister, ushort data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,uint data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 4), data);
    public void Store(EILRegister destinationRegister, uint data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,ulong data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 8), data);
    public void Store(EILRegister destinationRegister, ulong data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,float data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 4), data);
    public void Store(EILRegister destinationRegister, float data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }
    
    public void Store(ulong memoryAddress,double data) => BitConverter.TryWriteBytes(_mainMemory.Span.Slice((int)memoryAddress, 8), data);
    public void Store(EILRegister destinationRegister, double data)
    {
        var registerData = _registers[(int)destinationRegister].Span;
        BitConverter.TryWriteBytes(registerData, data);
    }

    #endregion
}