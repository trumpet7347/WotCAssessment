// See https://aka.ms/new-console-template for more information

using Wotc.Attributes;

var monster = new LayeredAttributesImplementation();

var effect1 = new LayeredEffectDefinition
{
    Attribute = AttributeKey.Power,
    Operation = EffectOperation.Add,
    Layer = 1,
    Modification = 3
};

var effect2 = new LayeredEffectDefinition
{
    Attribute = AttributeKey.Power,
    Operation = EffectOperation.Multiply,
    Layer = 1,
    Modification = 2
};

monster.AddLayeredEffect(effect1);
Console.WriteLine(monster.GetCurrentAttribute(AttributeKey.Power));

monster.AddLayeredEffect(effect2);
Console.WriteLine(monster.GetCurrentAttribute(AttributeKey.Power));

monster.SetBaseAttribute(AttributeKey.Power, 10);
Console.WriteLine(monster.GetCurrentAttribute(AttributeKey.Power));

Console.WriteLine(monster.GetCurrentAttribute(AttributeKey.Supertypes));