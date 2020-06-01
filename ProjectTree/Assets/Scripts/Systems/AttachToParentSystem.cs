using Unity.Entities;
using Unity.Transforms;

public class AttachToParentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var translations = GetComponentDataFromEntity<Translation>();
        Entities.ForEach((ref ParentComponent parentComponent, ref Translation translation, ref Entity entity) =>
        {
            var pos = translations[parentComponent.parent].Value;

            pos.y += 1.432f;


            translation.Value = pos;
        }).Run();
    }
}