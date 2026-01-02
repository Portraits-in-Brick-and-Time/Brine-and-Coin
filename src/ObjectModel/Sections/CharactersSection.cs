using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets.Characters;
using ObjectModel.Models;

namespace ObjectModel.Sections;

public class CharactersSection(ElfFile file) : CustomSection(file)
{
    public List<Character> Characters { get; } = [];

    public IReadOnlyList<PlayableCharacter> GetPlayers() => [.. Characters.OfType<PlayableCharacter>()];
    public IReadOnlyList<NonPlayableCharacter> GetNPCs() => [.. Characters.OfType<NonPlayableCharacter>()];

    public override string Name => ".characters";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Characters.Count);
        foreach (var character in Characters)
        {
            var start = (ulong)writer.BaseStream.Position;
            var model = CharacterModel.FromCharacter(character);
            writer.Write(MessagePackSerializer.Serialize(model));
            AddSymbol(character.Identifier.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<CharacterModel>(reader.BaseStream);
            var instance = (Character)model.Instanciate();
            model.InstanciateAttributesTo(instance, CustomSections.AttributesSection);
            Characters.Add(instance);
        }
    }
}