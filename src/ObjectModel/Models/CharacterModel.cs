using MessagePack;
using NetAF.Assets.Characters;

namespace ObjectModel.Models;

#nullable disable

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

    [Key(1)]
    public string Description { get; set; }
    
    [Key(2)]
    public bool IsNPC { get; set; }

    public override object Instanciate()
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
        
        //c.Attributes.Add(new NetAF.Assets.Attributes.Attribute())
        
        return c;
    }
}