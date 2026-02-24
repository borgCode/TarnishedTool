using System;

namespace TarnishedTool.Utilities
{
    /// <summary>
    /// Utility methods for constructing and patching x86-64 assembly code bytes.
    /// </summary>
    public static class AsmHelper
    {
        /// <summary>
        /// Calculates the relative offset from an instruction to a target address.
        /// </summary>
        /// <param name="instructionAddress">The address of the instruction containing the offset.</param>
        /// <param name="targetAddress">The target address the offset should point to.</param>
        /// <param name="instructionLength">The total length of the instruction (offset is calculated from instruction end).</param>
        /// <returns>The signed 32-bit relative offset.</returns>
        public static int GetRelOffset(IntPtr instructionAddress, IntPtr targetAddress, int instructionLength = 0)
            => (int)(targetAddress.ToInt64() - (instructionAddress.ToInt64() + instructionLength));

        /// <summary>
        /// Calculates the relative offset and returns it as a little-endian byte array.
        /// </summary>
        /// <param name="instructionAddress">The address of the instruction containing the offset.</param>
        /// <param name="targetAddress">The target address the offset should point to.</param>
        /// <param name="instructionLength">The total length of the instruction (offset is calculated from instruction end).</param>
        /// <returns>A 4-byte little-endian representation of the relative offset.</returns>
        public static byte[] GetRelOffsetBytes(IntPtr instructionAddress, IntPtr targetAddress, int instructionLength = 0)
            => BitConverter.GetBytes(GetRelOffset(instructionAddress, targetAddress, instructionLength));

        /// <inheritdoc cref="GetRelOffset(IntPtr, IntPtr, int)"/>
        public static int GetRelOffset(long instructionAddress, long targetAddress, int instructionLength = 0)
            => (int)(targetAddress - (instructionAddress + instructionLength));

        /// <inheritdoc cref="GetRelOffsetBytes(IntPtr, IntPtr, int)"/>
        public static byte[] GetRelOffsetBytes(long instructionAddress, long targetAddress, int instructionLength = 0)
            => BitConverter.GetBytes(GetRelOffset(instructionAddress, targetAddress, instructionLength));

        /// <summary>
        /// Writes multiple relative offsets into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="patches">
        /// Array of patches, each containing:
        /// <list type="bullet">
        ///   <item><description>instructionAddress: Address of the instruction in memory.</description></item>
        ///   <item><description>targetAddress: The target address the offset should point to.</description></item>
        ///   <item><description>instructionLength: Total length of the instruction.</description></item>
        ///   <item><description>writeOffset: Index in the buffer to write the 4-byte offset.</description></item>
        /// </list>
        /// </param>
        public static void WriteRelativeOffsets(byte[] buffer,
            (long instructionAddress, long targetAddress, int instructionLength, int writeOffset)[] patches)
        {
            foreach (var (instructionAddress, targetAddress, instructionLength, writeOffset) in patches)
            {
                var relativeBytes = GetRelOffsetBytes(instructionAddress, targetAddress, instructionLength);
                Array.Copy(relativeBytes, 0, buffer, writeOffset, 4);
            }
        }
        
        /// <summary>
        /// Writes a single relative offset into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="instructionAddress">Address of the instruction in memory.</param>
        /// <param name="targetAddress">The target address the offset should point to.</param>
        /// <param name="instructionLength">Total length of the instruction.</param>
        /// <param name="writeOffset">Index in the buffer to write the 4-byte offset.</param>
        public static void WriteRelativeOffset(byte[] buffer, nint instructionAddress, nint targetAddress, int instructionLength, int writeOffset)
        {
            var relativeBytes = GetRelOffsetBytes(instructionAddress, targetAddress, instructionLength);
            Array.Copy(relativeBytes, 0, buffer, writeOffset, 4);
        }

        /// <summary>
        /// Calculates the relative offset for a jump back to the original code after a hook.
        /// </summary>
        /// <param name="hookAddress">The address where the hook was installed.</param>
        /// <param name="originalInstructionLength">The length of the original instructions that were overwritten.</param>
        /// <param name="jmpInstructionEnd">The address immediately after the jmp instruction (where RIP will be).</param>
        /// <returns>A 4-byte little-endian representation of the relative offset.</returns>
        public static byte[] GetJmpOriginOffsetBytes(long hookAddress, int originalInstructionLength, IntPtr jmpInstructionEnd)
            => BitConverter.GetBytes((int)(hookAddress + originalInstructionLength - jmpInstructionEnd.ToInt64()));

        /// <summary>
        /// Writes multiple jump-back offsets into a byte buffer for hook trampolines.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="patches">
        /// Array of patches, each containing:
        /// <list type="bullet">
        ///   <item><description>hookAddress: Address where the hook was installed.</description></item>
        ///   <item><description>originalInstructionLength: Length of the original instructions that were overwritten.</description></item>
        ///   <item><description>jmpInstructionAddress: Address of the jmp instruction in the buffer.</description></item>
        ///   <item><description>writeOffset: Index in the buffer to write the 4-byte offset.</description></item>
        /// </list>
        /// </param>
        /// <remarks>
        /// Assumes a 5-byte near jmp (E9 xx xx xx xx). The offset is calculated from the end of the jmp instruction.
        /// </remarks>
        public static void WriteJumpOffsets(byte[] buffer,
            (long hookAddress, int originalInstructionLength, IntPtr jmpInstructionAddress, int writeOffset)[] patches)
        {
            foreach (var (hookAddress, originalInstructionLength, jmpInstructionAddress, writeOffset) in patches)
            {
                var originOffsetBytes = GetJmpOriginOffsetBytes(hookAddress, originalInstructionLength, jmpInstructionAddress + 5);
                Array.Copy(originOffsetBytes, 0, buffer, writeOffset, 4);
            }
        }

        /// <summary>
        /// Converts a 64-bit address to a little-endian byte array.
        /// </summary>
        /// <param name="address">The absolute address.</param>
        /// <returns>An 8-byte little-endian representation of the address.</returns>
        public static byte[] GetAbsAddressBytes(long address)
            => BitConverter.GetBytes(address);

        /// <summary>
        /// Writes multiple 64-bit absolute addresses into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="patches">Array of (address, writeOffset) tuples.</param>
        public static void WriteAbsoluteAddresses(byte[] buffer, (long address, int writeOffset)[] patches)
        {
            foreach (var (address, writeOffset) in patches)
            {
                var addressBytes = GetAbsAddressBytes(address);
                Array.Copy(addressBytes, 0, buffer, writeOffset, 8);
            }
        }

        /// <summary>
        /// Writes a single 64-bit absolute address into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="address">The absolute address to write.</param>
        /// <param name="writeOffset">Index in the buffer to write the 8-byte address.</param>
        public static void WriteAbsoluteAddress(byte[] buffer, long address, int writeOffset)
        {
            var addressBytes = GetAbsAddressBytes(address);
            Array.Copy(addressBytes, 0, buffer, writeOffset, 8);
        }

        /// <summary>
        /// Writes multiple 32-bit immediate values into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="patches">Array of (value, writeOffset) tuples.</param>
        public static void WriteImmediateDwords(byte[] buffer, (int value, int writeOffset)[] patches)
        {
            foreach (var (value, writeOffset) in patches)
            {
                var valueBytes = BitConverter.GetBytes(value);
                Array.Copy(valueBytes, 0, buffer, writeOffset, 4);
            }
        }
    }
}