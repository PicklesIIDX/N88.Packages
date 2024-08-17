namespace N88.Worlds.Spec;

using FluentAssertions;

public class Worlds
{
    private World _world = new();
    
    [SetUp]
    public void Setup()
    {
        _world = new World();
    }

    [Test]
    public void Create_entities_in_sequential_id_order_starting_from_one()
    {
        _world.CreateEntity().Should().Be(1);
        _world.CreateEntity().Should().Be(2);
        _world.CreateEntity().Should().Be(3);
        _world.CreateEntity().Should().Be(4);
    }
    
    [Test]
    public void Can_only_bind_one_component_of_the_same_type_to_an_entity()
    {
        var entity = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity, new MockComponent()).Should().Be(true);
        Assert.Throws<ComponentBindException>(() => _world.TryBindComponentToEntity(entity, new MockComponent()));
    }

    [Test]
    public void Cannot_bind_to_null_components()
    {
        var entity = _world.CreateEntity();
        Assert.Throws<NullReferenceException>(() => _world.TryBindComponentToEntity<MockComponent>(entity, null!));
    }
    
    [Test]
    public void Can_return_a_component_bound_to_an_entity()
    {
        var entity = _world.CreateEntity();
        var component = new MockComponent();
        _world.TryBindComponentToEntity(entity, component).Should().Be(true);
        _world.TryGetComponentMappedToEntity(entity, out MockComponent? result).Should().Be(true);
        result.Should().Be(component);
    }

    [Test]
    public void Cannot_output_null_component_when_successful()
    {
        var entity = _world.CreateEntity();
        var nonBoundEntity = _world.CreateEntity();
        var component = new MockComponent();
        _world.TryBindComponentToEntity(entity, component);
        if (_world.TryGetComponentMappedToEntity(entity, out MockComponent? result))
        {
            result.Should().NotBeNull();
        }

        if (!_world.TryGetComponentMappedToEntity(nonBoundEntity, out MockComponent? nullResult))
        {
            nullResult.Should().BeNull();
        }
    }
    
    [Test]
    public void Can_return_a_component_not_not_bound_to_an_entity()
    {
        var entity = _world.CreateEntity();
        var component = new MockComponent();
        _world.TryBindComponentToEntity(entity, component).Should().Be(true);
        _world.TryReleaseComponent<MockComponent>(entity).Should().Be(true);
        _world.GetUnboundComponent<MockComponent>().Should().Be(component);
    }
    
    [Test]
    public void Will_fail_to_unbind_a_component_not_bound_to_an_entity()
    {
        var entity = _world.CreateEntity();
        _world.TryReleaseComponent<MockComponent>(entity).Should().Be(false);
    }
    
    [Test]
    public void Will_fail_to_interact_with_non_existent_entity()
    {
        var entity = _world.CreateEntity();
        var nonExistentEntity = entity+1;
        _world.TryReleaseComponent<MockComponent>(nonExistentEntity).Should().Be(false);
        _world.TryBindComponentToEntity(nonExistentEntity, new MockComponent()).Should().Be(false);
        _world.TryGetComponentMappedToEntity(nonExistentEntity, out MockComponent? _).Should().Be(false);
        _world.TryReleaseEntity(nonExistentEntity).Should().Be(false);
    }
    
    [Test]
    public void Return_all_entities_bound_to_a_given_component_type()
    {
        var entity1 = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity1, new MockComponent());
        var entity2 = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity2, new MockComponent());
        var entity3 = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity3, new MockComponent());

        var entities = _world.GetEntitiesWithComponent<MockComponent>();
        entities.Should().ContainInOrder([entity1, entity2, entity3]);
    }
    
    [Test]
    public void Can_not_return_null_when_no_entities_have_component()
    {
        var result = _world.GetEntitiesWithComponent<MockComponent>();
        result.Should().NotBeNull();
    }
    
    
    [Test]
    public void Can_return_all_components_of_a_type_bound_to_any_entity()
    {
        var component1 = new MockComponent();
        _world.TryBindComponentToEntity(_world.CreateEntity(), component1);
        var component2 = new MockComponent();
        _world.TryBindComponentToEntity(_world.CreateEntity(), component2);
        var component3 = new MockComponent();
        _world.TryBindComponentToEntity(_world.CreateEntity(), component3);

        var components = _world.GetComponentsForAllEntities<MockComponent>();
        components.Should().ContainInOrder([component1, component2, component3]);
    }

    [Test]
    public void Can_not_return_null_when_no_components_bound_to_any_entities()
    {
        var result = _world.GetComponentsForAllEntities<MockComponent>();
        result.Should().NotBeNull();
    }
    
    [Test]
    public void Disposes_of_disposable_components_when_released_from_an_entity()
    {
        var component = new MockComponent();
        component.Disposed.Should().Be(false);

        var entity = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity, component);
        _world.TryReleaseComponent<MockComponent>(entity);

        component.Disposed.Should().Be(true);

        component.Disposed = false;
        _world.TryBindComponentToEntity(entity, component);
        _world.TryReleaseEntity(entity);

        component.Disposed.Should().Be(true);
    }

    [Test]
    public void Can_recycle_unbound_component()
    {
        var entity = _world.CreateEntity();
        var component = new MockComponent();
        _world.TryBindComponentToEntity(entity, component).Should().Be(true);
        _world.TryReleaseComponent<MockComponent>(entity).Should().Be(true);

        _world.GetUnboundComponent<MockComponent>().Should().Be(component);
    }

    [Test]
    public void Will_provide_new_component_if_non_unbound_when_requested()
    {
        var component = new MockComponent();;

        var newComponent = _world.GetUnboundComponent<MockComponent>();
        newComponent.Should().NotBe(component);
        
        newComponent.Should().BeEquivalentTo((MockComponent)default!);

        var entity = _world.CreateEntity();
        _world.TryBindComponentToEntity(entity, component).Should().Be(true);
        _world.TryReleaseEntity(entity).Should().Be(true);
        var pooled = _world.GetUnboundComponent<MockComponent>();
        pooled.Should().BeEquivalentTo(component);
        _world.TryBindComponentToEntity(entity, pooled).Should().Be(true);
        
        _world.GetUnboundComponent<MockComponent>().Should().BeEquivalentTo((MockComponent)default!);
    }
}

public class MockComponent : IDisposable
{
    public bool Disposed = false;
    public int Count = 0;
    public void Dispose()
    {
        Disposed = true;
    }
}