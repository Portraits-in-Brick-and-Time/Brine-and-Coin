using MessagePack;

namespace ObjectModel.Models.Quest.Steps;

[MessagePackObject(AllowPrivate = true)]
internal class CharacterDiesStepModel : IQuestStepModel
{
    public CharacterDiesStepModel(string characterName)
    {
        CharacterName = characterName;
    }

    [Key(0)]
    public string CharacterName { get; set; }
}
