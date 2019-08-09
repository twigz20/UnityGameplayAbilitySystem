﻿using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[UpdateAfter(typeof(ResetAttributesDeltaSystem))]
[UpdateBefore(typeof(ApplyAttributesDeltaSystem))]
public class __ATTRIBUTE__ModificationSystem : GameplayEffectAttributeModificationSystem<P__ATTRIBUTE__ModifierJob, T__ATTRIBUTE__ModifierJob> { }

[RequireComponentTag(typeof(AttributeModifyComponent), typeof(__ATTRIBUTE__Modifier), typeof(PermanentAttributeModification))]
public struct P__ATTRIBUTE__ModifierJob : AttributeModifierJob<PermanentAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<PermanentAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Health;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.BaseValue += attrMod.Change;
            attrs.Health = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
        Ecb.DestroyEntity(index, entity);
    }
}

[RequireComponentTag(typeof(AttributeModifyComponent), typeof(__ATTRIBUTE__Modifier), typeof(TemporaryAttributeModification))]
public struct T__ATTRIBUTE__ModifierJob : AttributeModifierJob<TemporaryAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<TemporaryAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Health;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.TempDelta += attrMod.Change;
            attrs.Health = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}



public struct __ATTRIBUTE__Modifier : AttributeModifier, IComponentData {
    public void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        throw new System.NotImplementedException();
    }

    public void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Health;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.TempDelta += attrMod.Change;
        attrs.Health = attr;
    }
}
public interface AttributeModifier  {
    void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs);
    void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs);
}
