using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using ELFSharp.Utilities;

namespace ELFSharp.ELF.Sections
{
    public sealed class SymbolTable<T> : Section<T>, ISymbolTable where T : struct
    {
        internal SymbolTable(SectionHeader header, SimpleEndianessAwareReader Reader, IStringTable table, ELF<T> elf) : base(header, Reader)
        {
            this.table = table;
            this.elf = elf;
            ReadSymbols();
        }

        public IEnumerable<SymbolEntry<T>> Entries
        {
            get { return new ReadOnlyCollection<SymbolEntry<T>>(entries); }
        }

        IEnumerable<ISymbolEntry> ISymbolTable.Entries
        {
            get { return Entries.Cast<ISymbolEntry>(); }
        }

        private void ReadSymbols()
        {
            SeekToSectionBeginning();
            entries = new List<SymbolEntry<T>>();
            var adder = (ulong)(elf.Class == Class.Bit32 ? Consts.SymbolEntrySize32 : Consts.SymbolEntrySize64);
            for(var i = 0UL; i < Header.Size; i += adder)
            {
                uint value;
                uint size;
                var nameIdx = Reader.ReadUInt32();
                //if(elf.Class == Class.Bit32)
                //{
                long value_offset = Reader.BaseStream.Position;
                    value = Reader.ReadUInt32();
                    size = Reader.ReadUInt32();
                //}
                var info = Reader.ReadByte();
                Reader.ReadByte(); // other is read, which holds zero
                var sectionIdx = Reader.ReadUInt16();
                if(elf.Class == Class.Bit64)
                {
                    throw new Exception("I am CEO of Core X Team, I cut this shit out");
                    //value = Reader.ReadUInt64();
                    //size = Reader.ReadUInt64();
                }
                var name = table == null ? "<corrupt>" : table[nameIdx];
                var binding = (SymbolBinding)(info >> 4);
                var type = (SymbolType)(info & 0x0F);
                if (name == "__AnimationBank:::bank")
                {
                    Main.AnimationBank_Offset = (int) value_offset;
                }
                entries.Add(new SymbolEntry<T>(name, value, size, binding, type, elf, sectionIdx));
            }
        }

        private List<SymbolEntry<T>> entries;
        private readonly IStringTable table;
        private readonly ELF<T> elf;
    }
}