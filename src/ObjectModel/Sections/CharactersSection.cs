using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class CharactersSection(ElfFile file) : CustomSection(file)
{
    public List<CharacterModel> Characters { get; } = [];

    public override string Name => ".characters";
    protected override void Write(BinaryWriter writer)
    {
        foreach (var character in Characters)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(character));

            AddSymbol(character.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<CharacterModel>(reader.BaseStream);

            Characters.Add(model);
        }
    }
}