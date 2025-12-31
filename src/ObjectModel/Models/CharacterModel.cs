using MessagePack;
using NetAF.Assets;
using NetAF.Assets.Characters;

namespace ObjectModel.Models;

[MessagePackObject]
public class CharacterModel : GameObject {
    public CharacterModel(string name, string description, bool isNPC)
    {
        Name = name;
        Description = description;
        IsNPC = isNPC;
    }
    public CharacterModel()
    {
	
    }

    [Key(2)]
    public string Description { get; set; }
    
    [Key(3)]
    public bool IsNPC { get; set; }

    public override IExaminable Instanciate()
    {
        Character c;
        if (IsNPC)
        {
            c = new NonPlayableCharacter(Name, Description);
        }
        else
        {
            c = new PlayableCharacter(Name, Description);
        }
    
        return c;
    }
}