namespace ObjectModel;

using System.IO;
using Hocon;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

public class GameObjectWriter : IDisposable
{
    private readonly MemoryStream _codeStream = new();
    private readonly ElfFile _file = new(ElfArch.ARM);
    private readonly Stream _outputStream;
    private readonly ElfStringTable _strTable = new();
    private readonly ElfSymbolTable _symbolTable = new();
    private ElfSection _objectsSection;

    public GameObjectWriter(Stream outputStream)
    {
        _outputStream = outputStream;

        _file.Add(_strTable);
    }

    public bool IsClosed { get; set; }

    public void WriteObject(GameObject obj)
    {
        if (IsClosed)
        {
            return;
        }

        var start = (ulong)_codeStream.Position;
        MessagePackSerializer.Serialize(_codeStream, obj);
        AddSymbol(obj.Name, start, (ulong)_codeStream.Position - start);
    }
    
    public void WriteObjects(string defintiionFile) {
        var config = HoconParser.Parse(File.ReadAllText(defintiionFile));
     
        foreach(var (name, def) in config.AsEnumerable()) {
            var obj = def.GetObject();
            var type = obj.GetField("type").GetString();

            if (type is "character") {
                var description = obj.GetField("description").GetString(); 
                var isNPC = obj.GetField("isNPC").GetString() == "true";
                
                WriteObject(new CharacterModel(name, description, isNPC));
            }
        }
    }

    private void AddSymbol(string name, ulong index, ulong size)
    {
        foreach (var symbol in _symbolTable.Entries)
        {
            if (symbol.Name == name)
            {
                return;
            }
        }

        _symbolTable.Entries.Add(
            new ElfSymbol()
            {
                Bind = ElfSymbolBind.Global,
                Name = name,
                Value = index,
                Type = ElfSymbolType.Object,
                Size = size,
                SectionLink = _objectsSection
            });
    }

    public void Close()
    {
        if (IsClosed)
        {
            return;
        }

        _objectsSection = new ElfStreamSection(ElfSectionSpecialType.Text, _codeStream)
        {
            Name = ".objects",
            Flags = ElfSectionFlags.None
        };

        _symbolTable.Link = _strTable;

        _file.Add(new ElfSectionHeaderStringTable());
        _file.Add(new ElfSectionHeaderTable());

        _file.Add(_symbolTable);
        _file.Add(_objectsSection);

        _file.Write(_outputStream);
        _outputStream.Flush();

        _outputStream.Close();
        _codeStream.Close();

        IsClosed = true;
    }
    
    public void Dispose()
    {
        Close();
    }
}
