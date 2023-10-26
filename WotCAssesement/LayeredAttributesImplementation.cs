namespace Wotc.Attributes;

public class AttributeValue<T>
{
    private readonly T _defaultValue;
    public T BaseValue { get; set; }
    public T CurrentValue { get; set; }

    public AttributeValue(T defaultValue)
    {
        _defaultValue = defaultValue;
        ResetValues();
    }
    
    public void ResetValues()
    {
        BaseValue = _defaultValue;
        CurrentValue = _defaultValue;
    }
}
public class LayeredAttributesImplementation: ILayeredAttributes
{
    private const int DefaultAttributeValue = 0;
    
    private readonly Dictionary<AttributeKey, AttributeValue<int>> _attributes = new();
    private readonly Dictionary<AttributeKey, PriorityQueue<LayeredEffectDefinition, int>> _layeredEffects = new();
    
    public void SetBaseAttribute(AttributeKey key, int value)
    {
        // check if the attribute already exists, if not add a new entry with the value, otherwise se the value with 0
        if (_attributes.TryGetValue(key, out var attributeValue))
        {
            attributeValue.BaseValue = value;
        }
        else
        {
            _attributes[key] = new AttributeValue<int>(value);
        }
        
        CalculateCurrentAttribute(key);
    }

    public int GetCurrentAttribute(AttributeKey key)
    {
        return _attributes.TryGetValue(key, out var attributeValue) ? attributeValue.CurrentValue : DefaultAttributeValue;
    }
    
    public void AddLayeredEffect(LayeredEffectDefinition effect)
    {
        // add the layered effect to the list, and recalc the attribute it affected

        if (_layeredEffects.TryGetValue(effect.Attribute, out var attribute))
        {
            attribute.Enqueue(effect, effect.Layer);
        }
        else
        {
            _layeredEffects[effect.Attribute] = new PriorityQueue<LayeredEffectDefinition, int>();
            _layeredEffects[effect.Attribute].Enqueue(effect, effect.Layer);
        }

        CalculateCurrentAttribute(effect.Attribute);
    }

    public void ClearLayeredEffects()
    {
        // clear out the layered effects list
        _layeredEffects.Clear();

        // set current values back to base values
        foreach (KeyValuePair<AttributeKey, AttributeValue<int>> attribute in _attributes)
        {
            attribute.Value.ResetValues();
        }
    }
    
    private void CalculateCurrentAttribute(AttributeKey effectAttribute)
    {
        // see if we have a base value, if not create it
        if (!_attributes.ContainsKey(effectAttribute))
        {
            _attributes[effectAttribute] = new AttributeValue<int>(DefaultAttributeValue);
        }
        
        // grab the base value
        int value = _attributes[effectAttribute].BaseValue;
        
        // get all the layered effects, and order them by the layered attribute to get them in the right order
        PriorityQueue<LayeredEffectDefinition, int> effects = _layeredEffects[effectAttribute];
        var queueCopy = new PriorityQueue<LayeredEffectDefinition, int>();

        while (effects.TryDequeue(out var effect, out var priority))
        {
            switch (effect.Operation)
            {
                case EffectOperation.Set:
                    value = effect.Modification;
                    break;
                case EffectOperation.Add:
                    value += effect.Modification;
                    break;
                case EffectOperation.Subtract:
                    value -= effect.Modification;
                    break;
                case EffectOperation.Multiply:
                    value *= effect.Modification;
                    break;
                case EffectOperation.BitwiseOr:
                    value |= effect.Modification;
                    break;
                case EffectOperation.BitwiseAnd:
                    value &= effect.Modification;
                    break;
                case EffectOperation.BitwiseXor:
                    value ^= effect.Modification;
                    break;
            }
            queueCopy.Enqueue(effect, effect.Layer);
        }
        
        _attributes[effectAttribute].CurrentValue = value;
        _layeredEffects[effectAttribute] = queueCopy;
    }
}