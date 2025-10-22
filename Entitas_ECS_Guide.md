# Entitas ECS - Полное руководство по разработке

> Составлено на основе анализа проекта ecs-survivors

## Содержание
1. [Основные концепции](#основные-концепции)
2. [Архитектура проекта](#архитектура-проекта)
3. [Компоненты (Components)](#компоненты-components)
4. [Системы (Systems)](#системы-systems)
5. [Контексты (Contexts)](#контексты-contexts)
6. [Features - модульная организация](#features---модульная-организация)
7. [Создание сущностей (Entity Creation)](#создание-сущностей-entity-creation)
8. [Паттерны Factory](#паттерны-factory)
9. [View Binding - связь с Unity](#view-binding---связь-с-unity)
10. [Жизненный цикл и уничтожение](#жизненный-цикл-и-уничтожение)
11. [Entity Indices - быстрый поиск](#entity-indices---быстрый-поиск)
12. [Системы способностей и эффектов](#системы-способностей-и-эффектов)
13. [Система ввода (Input)](#система-ввода-input)
14. [Dependency Injection](#dependency-injection)
15. [Производительность и оптимизация](#производительность-и-оптимизация)
16. [Лучшие практики](#лучшие-практики)

---

## Основные концепции

### ECS = Entity Component System

```
Entity (Сущность)      - Контейнер для компонентов (ID)
Component (Компонент)  - Чистые данные (без логики)
System (Система)       - Логика обработки компонентов
```

### Философия Entitas
- **Данные отделены от логики** - компоненты = данные, системы = логика
- **Композиция вместо наследования** - сущности строятся из компонентов
- **Реактивные системы** - автоматическая реакция на изменения
- **Кодогенерация** - автоматическое создание удобного API

---

## Архитектура проекта

### Рекомендуемая структура директорий

```
Assets/Code/
├── Common/                     # Общие компоненты и системы
│   ├── Destruct/              # Система уничтожения
│   ├── Entity/                # Вспомогательные методы создания
│   └── Extensions/            # Расширения для удобства
├── Gameplay/                  # Игровая логика
│   ├── BattleFeature.cs      # Корневая Feature
│   ├── Features/              # Модули функциональности
│   │   ├── Hero/             # Герой
│   │   ├── Enemies/          # Враги
│   │   ├── Abilities/        # Способности
│   │   ├── Movement/         # Движение
│   │   ├── Effects/          # Эффекты
│   │   └── ...
│   ├── Input/                 # Обработка ввода
│   └── StaticData/           # Конфигурация
├── Infrastructure/            # Базовая инфраструктура
│   ├── Systems/              # Фабрики систем
│   ├── Services/             # Сервисы
│   └── View/                 # Привязка к Unity View
├── Meta/                      # Мета-игра (меню, магазин)
└── Generated/                 # Автогенерируемый код Entitas
    ├── Game/
    ├── Input/
    └── Meta/
```

---

## Компоненты (Components)

### Типы компонентов

#### 1. Маркерные компоненты (Marker)
```csharp
[Game]
public class Hero : IComponent { }

[Game]
public class Enemy : IComponent { }

[Game]
public class Moving : IComponent { }

[Game]
public class Destructed : IComponent { }
```

#### 2. Компоненты-данные (Data)
```csharp
[Game]
public class WorldPosition : IComponent
{
    public Vector3 Value;
}

[Game]
public class Speed : IComponent
{
    public float Value;
}

[Game]
public class Direction : IComponent
{
    public Vector2 Value;
}

[Game]
public class CurrentHp : IComponent
{
    public float Value;
}
```

#### 3. Компоненты со сложными данными
```csharp
[Game]
public class BaseStats : IComponent
{
    public Dictionary<Stats, float> Value;
}

[Game]
public class ViewPath : IComponent
{
    public string Value;
}
```

#### 4. Компоненты с ссылками на Unity объекты
```csharp
[Game]
public class View : IComponent
{
    public IEntityView Value;
}

[Game]
public class TransformComponent : IComponent
{
    public Transform Value;
}

[Game]
public class HeroAnimatorComponent : IComponent
{
    public HeroAnimator Value;
}
```

### Правила создания компонентов

#### ✅ Правильно
```csharp
// Используйте атрибут контекста
[Game] public class Speed : IComponent { public float Value; }

// Группируйте связанные компоненты в одном файле
// HeroComponents.cs
[Game] public class Hero : IComponent { }
[Game] public class HeroAnimatorComponent : IComponent { public HeroAnimator Value; }

// Используйте простые типы данных
[Game] public class Damage : IComponent { public float Value; }
```

#### ❌ Неправильно
```csharp
// НЕ добавляйте логику в компоненты
public class Speed : IComponent
{
    public float Value;
    public void UpdateSpeed() { } // ❌ Логика должна быть в системах!
}

// НЕ используйте множественное наследование
public class HeroSpeed : Speed { } // ❌

// НЕ создавайте компоненты без атрибута контекста
public class SomeComponent : IComponent { } // ❌ Нет [Game], [Input] и т.д.
```

### Организация компонентов по файлам

```csharp
// CommonComponents.cs - общие компоненты
[Game] public class Id : IComponent { [PrimaryEntityIndex] public int Value; }
[Game] public class Destructed : IComponent { }
[Game] public class WorldPosition : IComponent { public Vector3 Value; }
[Game] public class ViewPath : IComponent { public string Value; }
[Game] public class View : IComponent { public IEntityView Value; }

// MovementComponents.cs - компоненты движения
[Game] public class Speed : IComponent { public float Value; }
[Game] public class Direction : IComponent { public Vector2 Value; }
[Game] public class Moving : IComponent { }
[Game] public class TurnedAlongDirection : IComponent { }

// HeroComponents.cs - компоненты героя
[Game] public class Hero : IComponent { }
[Game] public class HeroAnimatorComponent : IComponent { public HeroAnimator Value; }

// EnemyComponents.cs - компоненты врагов
[Game] public class Enemy : IComponent { }
[Game] public class EnemyTypeId : IComponent { public EnemyTypeId Value; }
[Game] public class EnemyAnimatorComponent : IComponent { public EnemyAnimator Value; }
```

---

## Системы (Systems)

### Типы систем

#### 1. IInitializeSystem - инициализация (один раз при запуске)
```csharp
public class InitializeHeroSystem : IInitializeSystem
{
    private readonly IHeroFactory _heroFactory;

    public InitializeHeroSystem(IHeroFactory heroFactory)
    {
        _heroFactory = heroFactory;
    }

    public void Initialize()
    {
        _heroFactory.CreateHero(Vector3.zero);
    }
}
```

#### 2. IExecuteSystem - выполнение каждый кадр
```csharp
public class SetHeroDirectionByInputSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _heroes;
    private readonly IGroup<InputEntity> _inputs;
    private readonly List<GameEntity> _buffer = new(32);

    public SetHeroDirectionByInputSystem(GameContext game, InputContext input)
    {
        _heroes = game.GetGroup(GameMatcher.Hero);
        _inputs = input.GetGroup(InputMatcher.Input);
    }

    public void Execute()
    {
        foreach (InputEntity input in _inputs)
        foreach (GameEntity hero in _heroes.GetEntities(_buffer))
        {
            hero.isMoving = input.hasAxisInput;

            if (input.hasAxisInput)
                hero.ReplaceDirection(input.AxisInput.normalized);
        }
    }
}
```

#### 3. ICleanupSystem - очистка после Execute
```csharp
public class CleanupGameDestructedSystem : ICleanupSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public CleanupGameDestructedSystem(GameContext game)
    {
        _entities = game.GetGroup(GameMatcher.Destructed);
    }

    public void Cleanup()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.Destroy();
        }
    }
}
```

#### 4. ITearDownSystem - финальная очистка
```csharp
public class TearDownSystem : ITearDownSystem
{
    public void TearDown()
    {
        // Очистка ресурсов при выходе
    }
}
```

### Шаблон типичной системы

```csharp
public class TypicalSystem : IExecuteSystem
{
    // 1. Группы сущностей для обработки
    private readonly IGroup<GameEntity> _entities;

    // 2. Буфер для переиспользования (оптимизация)
    private readonly List<GameEntity> _buffer = new(32);

    // 3. Зависимости (сервисы, фабрики)
    private readonly ITimeService _time;
    private readonly ISomeFactory _factory;

    // 4. Конструктор - настройка групп и DI
    public TypicalSystem(
        GameContext game,
        ITimeService time,
        ISomeFactory factory)
    {
        _time = time;
        _factory = factory;

        // Определяем группу сущностей через Matcher
        _entities = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.Component1,
                GameMatcher.Component2)
            .NoneOf(
                GameMatcher.Component3));
    }

    // 5. Логика обработки
    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            // Обработка каждой сущности
            ProcessEntity(entity);
        }
    }

    private void ProcessEntity(GameEntity entity)
    {
        // Бизнес-логика здесь
    }
}
```

### Matcher - запросы к сущностям

```csharp
// Все сущности с компонентами Hero И WorldPosition
game.GetGroup(GameMatcher
    .AllOf(
        GameMatcher.Hero,
        GameMatcher.WorldPosition));

// Сущности с Moving, но БЕЗ Destructed
game.GetGroup(GameMatcher
    .AllOf(GameMatcher.Moving)
    .NoneOf(GameMatcher.Destructed));

// Сложный запрос
game.GetGroup(GameMatcher
    .AllOf(
        GameMatcher.DamageEffect,
        GameMatcher.EffectValue,
        GameMatcher.TargetId)
    .NoneOf(
        GameMatcher.Processed));

// Любой из компонентов (OR)
game.GetGroup(GameMatcher
    .AnyOf(
        GameMatcher.Hero,
        GameMatcher.Enemy));
```

### Примеры реальных систем

#### Движение по направлению
```csharp
public class DirectionalMoveSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _movers;
    private readonly List<GameEntity> _buffer = new(64);
    private readonly ITimeService _time;

    public DirectionalMoveSystem(GameContext game, ITimeService time)
    {
        _time = time;
        _movers = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.Direction,
                GameMatcher.WorldPosition,
                GameMatcher.Speed,
                GameMatcher.Moving));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _movers.GetEntities(_buffer))
        {
            Vector3 delta = entity.Direction * entity.Speed * _time.DeltaTime;
            entity.ReplaceWorldPosition(entity.WorldPosition + delta);
        }
    }
}
```

#### Система смерти
```csharp
public class MarkDeadSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public MarkDeadSystem(GameContext game)
    {
        _entities = game.GetGroup(GameMatcher
            .AllOf(GameMatcher.CurrentHp)
            .NoneOf(GameMatcher.Dead));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            if (entity.CurrentHp <= 0)
                entity.isDead = true;
        }
    }
}
```

---

## Контексты (Contexts)

### Что такое Context?
Context - это контейнер для сущностей определенного типа. Разделение на контексты помогает организовать логику.

### Типичные контексты

```csharp
// Contexts.cs (автогенерируется)
public partial class Contexts : Entitas.Contexts
{
    public static Contexts sharedInstance;

    public GameContext game { get; set; }
    public InputContext input { get; set; }
    public MetaContext meta { get; set; }
}
```

#### GameContext - игровая логика
```csharp
[Game] public class Hero : IComponent { }
[Game] public class Enemy : IComponent { }
[Game] public class Projectile : IComponent { }
```

#### InputContext - ввод пользователя
```csharp
[Input] public class Input : IComponent { }
[Input] public class AxisInput : IComponent { public Vector2 Value; }
[Input] public class MousePosition : IComponent { public Vector3 Value; }
```

#### MetaContext - мета-игра (UI, меню)
```csharp
[Meta] public class MainMenuScreen : IComponent { }
[Meta] public class ShopItem : IComponent { }
[Meta] public class GameSettings : IComponent { }
```

### Мультиконтекстные компоненты
```csharp
// Компонент доступен в двух контекстах
[Game, Meta]
public class Destructed : IComponent { }
```

### Доступ к контекстам в системах
```csharp
public class CrossContextSystem : IExecuteSystem
{
    private readonly GameContext _game;
    private readonly InputContext _input;
    private readonly MetaContext _meta;

    public CrossContextSystem(GameContext game, InputContext input, MetaContext meta)
    {
        _game = game;
        _input = input;
        _meta = meta;
    }

    public void Execute()
    {
        // Используем несколько контекстов
    }
}
```

---

## Features - модульная организация

### Что такое Feature?
Feature - это контейнер для систем, объединяющий их по функциональности.

### Базовая структура Feature

```csharp
public class HeroFeature : Feature
{
    public HeroFeature(ISystemFactory systems)
    {
        // Порядок важен! Системы выполняются сверху вниз

        // 1. Инициализация
        Add(systems.Create<InitializeHeroSystem>());

        // 2. Обработка ввода
        Add(systems.Create<SetHeroDirectionByInputSystem>());

        // 3. Логика
        Add(systems.Create<HeroAttackSystem>());
        Add(systems.Create<AnimateHeroMovementSystem>());

        // 4. Смерть
        Add(systems.Create<HeroDeathSystem>());

        // 5. Финализация
        Add(systems.Create<FinalizeHeroDeathProcessingSystem>());
    }
}
```

### Иерархия Features - корневая Feature

```csharp
public class BattleFeature : Feature
{
    public BattleFeature(ISystemFactory systems)
    {
        // Input
        Add(new InputFeature(systems));

        // View Binding
        Add(new BindViewFeature(systems));

        // Gameplay Features
        Add(new HeroFeature(systems));
        Add(new EnemyFeature(systems));
        Add(new MovementFeature(systems));
        Add(new AbilityFeature(systems));
        Add(new EffectFeature(systems));
        Add(new LootingFeature(systems));

        // Cleanup
        Add(new ProcessDestructedFeature(systems));
        Add(systems.Create<GameOverOnHeroDeathSystem>());
    }
}
```

### Пример модульной Feature

```csharp
public class MovementFeature : Feature
{
    public MovementFeature(ISystemFactory systems)
    {
        // Расчет дельты движения
        Add(systems.Create<DirectionalDeltaMoveSystem>());
        Add(systems.Create<OrbitalDeltaMoveSystem>());

        // Применение движения
        Add(systems.Create<OrbitCenterFollowSystem>());
        Add(systems.Create<UpdateTransformPositionSystem>());

        // Поворот
        Add(systems.Create<TurnAlongDirectionSystem>());
        Add(systems.Create<RotateAlongDirectionSystem>());
    }
}
```

---

## Создание сущностей (Entity Creation)

### Базовое создание

```csharp
// Простое создание
GameEntity entity = Contexts.sharedInstance.game.CreateEntity();

// Или через вспомогательный класс
public static class CreateEntity
{
    public static GameEntity Empty() =>
        Contexts.sharedInstance.game.CreateEntity();
}
```

### Fluent API - цепочка вызовов

```csharp
GameEntity hero = CreateEntity.Empty()
    .AddId(1)
    .AddWorldPosition(Vector3.zero)
    .AddSpeed(5f)
    .AddDirection(Vector2.up)
    .With(x => x.isHero = true)
    .With(x => x.isMoving = true);
```

### Вспомогательный метод With()

```csharp
// Extension method для удобства
public static class EntityExtensions
{
    public static T With<T>(this T entity, Action<T> action) where T : IEntity
    {
        action(entity);
        return entity;
    }
}

// Использование
entity
    .AddSpeed(5f)
    .With(x => x.isActive = true)
    .With(x => x.isMoving = true);
```

---

## Паттерны Factory

### Базовая структура Factory

```csharp
public interface IHeroFactory
{
    GameEntity CreateHero(Vector3 at);
}

public class HeroFactory : IHeroFactory
{
    private readonly IIdentifierService _identifiers;

    public HeroFactory(IIdentifierService identifiers)
    {
        _identifiers = identifiers;
    }

    public GameEntity CreateHero(Vector3 at)
    {
        Dictionary<Stats, float> baseStats = InitStats.EmptyStatDictionary()
            .With(x => x[Stats.Speed] = 2f)
            .With(x => x[Stats.MaxHp] = 100f)
            .With(x => x[Stats.Damage] = 5f);

        return CreateEntity.Empty()
            .AddId(_identifiers.Next())
            .AddWorldPosition(at)
            .AddBaseStats(baseStats)
            .AddDirection(Vector2.zero)
            .AddSpeed(baseStats[Stats.Speed])
            .AddCurrentHp(baseStats[Stats.MaxHp])
            .AddViewPath("Gameplay/Hero/hero")
            .With(x => x.isHero = true)
            .With(x => x.isTurnedAlongDirection = true)
            .With(x => x.isMovementAvailable = true);
    }
}
```

### Factory для врагов

```csharp
public interface IEnemyFactory
{
    GameEntity CreateEnemy(EnemyTypeId typeId, Vector3 at);
}

public class EnemyFactory : IEnemyFactory
{
    private readonly IIdentifierService _identifiers;
    private readonly IStaticDataService _staticData;

    public EnemyFactory(
        IIdentifierService identifiers,
        IStaticDataService staticData)
    {
        _identifiers = identifiers;
        _staticData = staticData;
    }

    public GameEntity CreateEnemy(EnemyTypeId typeId, Vector3 at)
    {
        switch (typeId)
        {
            case EnemyTypeId.Goblin:
                return CreateGoblin(at);
            case EnemyTypeId.Orc:
                return CreateOrc(at);
            default:
                throw new ArgumentException($"Unknown enemy type: {typeId}");
        }
    }

    private GameEntity CreateGoblin(Vector3 at)
    {
        EnemyStaticData data = _staticData.ForEnemy(EnemyTypeId.Goblin);

        Dictionary<Stats, float> baseStats = InitStats.EmptyStatDictionary()
            .With(x => x[Stats.Speed] = data.Speed)
            .With(x => x[Stats.MaxHp] = data.MaxHp)
            .With(x => x[Stats.Damage] = data.Damage);

        return CreateEntity.Empty()
            .AddId(_identifiers.Next())
            .AddEnemyTypeId(EnemyTypeId.Goblin)
            .AddWorldPosition(at)
            .AddBaseStats(baseStats)
            .AddSpeed(baseStats[Stats.Speed])
            .AddCurrentHp(baseStats[Stats.MaxHp])
            .AddViewPath(data.PrefabPath)
            .With(x => x.isEnemy = true)
            .With(x => x.isMovementAvailable = true)
            .With(x => x.isTurnedAlongDirection = true);
    }

    private GameEntity CreateOrc(Vector3 at)
    {
        // Аналогично
    }
}
```

### Factory для снарядов

```csharp
public interface IArmamentFactory
{
    GameEntity CreateProjectile(Vector3 at, Vector3 direction, float damage);
}

public class ArmamentFactory : IArmamentFactory
{
    private readonly IIdentifierService _identifiers;

    public GameEntity CreateProjectile(Vector3 at, Vector3 direction, float damage)
    {
        return CreateEntity.Empty()
            .AddId(_identifiers.Next())
            .AddWorldPosition(at)
            .AddDirection(direction.normalized)
            .AddSpeed(10f)
            .AddDamage(damage)
            .AddViewPath("Gameplay/Projectiles/Arrow")
            .With(x => x.isProjectile = true)
            .With(x => x.isMoving = true)
            .AddSelfDestructTimer(5f); // Уничтожится через 5 сек
    }
}
```

---

## View Binding - связь с Unity

### Архитектура View Binding

```
GameEntity ←→ EntityBehaviour (MonoBehaviour) ←→ Unity GameObject
```

### EntityBehaviour - базовый класс для View

```csharp
public class EntityBehaviour : MonoBehaviour, IEntityView
{
    private GameEntity _entity;
    private ICollisionRegistry _collisionRegistry;

    public void Construct(ICollisionRegistry collisionRegistry)
    {
        _collisionRegistry = collisionRegistry;
    }

    // Вызывается когда View связывается с Entity
    public void SetEntity(GameEntity entity)
    {
        _entity = entity;
        _entity.AddView(this);
        _entity.Retain(this); // Удержание entity пока view существует

        // Регистрация всех компонент-регистраров
        foreach (IEntityComponentRegistrar registrar in
                 GetComponentsInChildren<IEntityComponentRegistrar>())
        {
            registrar.RegisterComponents();
        }

        // Регистрация коллайдеров
        foreach (Collider2D collider2d in GetComponentsInChildren<Collider2D>())
        {
            _collisionRegistry.Register(collider2d.GetInstanceID(), _entity);
        }
    }

    // Вызывается при уничтожении связи
    public void ReleaseEntity()
    {
        foreach (IEntityComponentRegistrar registrar in
                 GetComponentsInChildren<IEntityComponentRegistrar>())
        {
            registrar.UnregisterComponents();
        }

        foreach (Collider2D collider2d in GetComponentsInChildren<Collider2D>())
        {
            _collisionRegistry.Unregister(collider2d.GetInstanceID());
        }

        _entity.Release(this);
        _entity = null;
    }

    private void OnDestroy()
    {
        if (_entity != null)
            ReleaseEntity();
    }
}
```

### Система привязки View

```csharp
public class BindEntityViewFromPathSystem : IExecuteSystem
{
    private readonly IEntityViewFactory _entityViewFactory;
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public BindEntityViewFromPathSystem(
        GameContext game,
        IEntityViewFactory entityViewFactory)
    {
        _entityViewFactory = entityViewFactory;

        // Только сущности с ViewPath, но без View
        _entities = game.GetGroup(GameMatcher
            .AllOf(GameMatcher.ViewPath)
            .NoneOf(GameMatcher.View));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            _entityViewFactory.CreateViewForEntity(entity);
        }
    }
}
```

### EntityViewFactory

```csharp
public interface IEntityViewFactory
{
    void CreateViewForEntity(GameEntity entity);
}

public class EntityViewFactory : IEntityViewFactory
{
    private readonly IInstantiator _instantiator;
    private readonly ICollisionRegistry _collisionRegistry;

    public void CreateViewForEntity(GameEntity entity)
    {
        GameObject prefab = Resources.Load<GameObject>(entity.ViewPath);
        GameObject instance = _instantiator.Instantiate(prefab);

        EntityBehaviour entityBehaviour = instance.GetComponent<EntityBehaviour>();
        entityBehaviour.Construct(_collisionRegistry);
        entityBehaviour.SetEntity(entity);

        instance.transform.position = entity.WorldPosition;
    }
}
```

### Component Registrars

#### TransformRegistrar
```csharp
public class TransformRegistrar : EntityComponentRegistrar
{
    public override void RegisterComponents()
    {
        Entity.AddTransform(transform);
    }

    public override void UnregisterComponents()
    {
        if (Entity.hasTransform)
            Entity.RemoveTransform();
    }
}
```

#### SpriteRendererRegistrar
```csharp
public class SpriteRendererRegistrar : EntityComponentRegistrar
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public override void RegisterComponents()
    {
        Entity.AddSpriteRenderer(_spriteRenderer);
    }

    public override void UnregisterComponents()
    {
        if (Entity.hasSpriteRenderer)
            Entity.RemoveSpriteRenderer();
    }
}
```

#### AnimatorRegistrar
```csharp
public class HeroAnimatorRegistrar : EntityComponentRegistrar
{
    [SerializeField] private HeroAnimator _animator;

    public override void RegisterComponents()
    {
        Entity.AddHeroAnimator(_animator);
    }

    public override void UnregisterComponents()
    {
        if (Entity.hasHeroAnimator)
            Entity.RemoveHeroAnimator();
    }
}
```

### Система обновления позиции Transform

```csharp
public class UpdateTransformPositionSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(64);

    public UpdateTransformPositionSystem(GameContext game)
    {
        _entities = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.WorldPosition,
                GameMatcher.Transform));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.Transform.position = entity.WorldPosition;
        }
    }
}
```

---

## Жизненный цикл и уничтожение

### Двухфазное уничтожение

#### Фаза 1: Маркировка для уничтожения
```csharp
// Системы помечают сущности для удаления
entity.isDestructed = true;
```

#### Фаза 2: Cleanup
```csharp
public class ProcessDestructedFeature : Feature
{
    public ProcessDestructedFeature(ISystemFactory systems)
    {
        // 1. Таймер самоуничтожения
        Add(systems.Create<SelfDestructTimerSystem>());

        // 2. Очистка Meta контекста
        Add(systems.Create<CleanupMetaDestructedSystem>());

        // 3. Отвязка View от сущности
        Add(systems.Create<CleanupGameDestructedViewSystem>());

        // 4. Уничтожение самой сущности
        Add(systems.Create<CleanupGameDestructedSystem>());
    }
}
```

### SelfDestructTimerSystem

```csharp
public class SelfDestructTimerSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);
    private readonly ITimeService _time;

    public SelfDestructTimerSystem(GameContext game, ITimeService time)
    {
        _time = time;
        _entities = game.GetGroup(GameMatcher
            .AllOf(GameMatcher.SelfDestructTimer)
            .NoneOf(GameMatcher.Destructed));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.ReplaceSelfDestructTimer(
                entity.SelfDestructTimer - _time.DeltaTime);

            if (entity.SelfDestructTimer <= 0)
                entity.isDestructed = true;
        }
    }
}
```

### CleanupGameDestructedViewSystem

```csharp
public class CleanupGameDestructedViewSystem : ICleanupSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public CleanupGameDestructedViewSystem(GameContext game)
    {
        _entities = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.Destructed,
                GameMatcher.View));
    }

    public void Cleanup()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.View.ReleaseEntity();
            Object.Destroy(entity.View.gameObject);
            entity.RemoveView();
        }
    }
}
```

### CleanupGameDestructedSystem

```csharp
public class CleanupGameDestructedSystem : ICleanupSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public CleanupGameDestructedSystem(GameContext game)
    {
        _entities = game.GetGroup(GameMatcher.Destructed);
    }

    public void Cleanup()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.Destroy();
        }
    }
}
```

---

## Entity Indices - быстрый поиск

### Типы индексов

#### Primary Index - уникальный ключ
```csharp
[Game]
public class Id : IComponent
{
    [PrimaryEntityIndex]
    public int Value;
}

// Использование
GameEntity entity = game.GetEntityWithId(123);
```

#### Entity Index - неуникальный ключ
```csharp
[Game]
public class ParentAbility : IComponent
{
    [EntityIndex]
    public AbilityId Value;
}

// Использование - получить все сущности с данным AbilityId
HashSet<GameEntity> abilities = game.GetEntitiesWithParentAbility(AbilityId.FireBolt);
```

### Настройка индексов в Contexts.cs

```csharp
public partial class GameContext
{
    public GameEntity GetEntityWithId(int value)
    {
        return GetGroup(GameMatcher.Id).GetSingleEntity();
    }

    public HashSet<GameEntity> GetEntitiesWithParentAbility(AbilityId value)
    {
        // Автогенерируется Entitas
    }
}
```

### Extension методы для удобства

```csharp
public static class EntityLinkExtensions
{
    public static GameEntity Target(this GameEntity entity)
    {
        return Contexts.sharedInstance.game.GetEntityWithId(entity.TargetId);
    }

    public static GameEntity Producer(this GameEntity entity)
    {
        return Contexts.sharedInstance.game.GetEntityWithId(entity.ProducerId);
    }
}

// Использование
GameEntity target = effect.Target();
GameEntity producer = projectile.Producer();
```

---

## Системы способностей и эффектов

### Структура системы способностей

```csharp
public class VegetableBoltAbilitySystem : IExecuteSystem
{
    private readonly IArmamentFactory _armamentFactory;
    private readonly IAbilityUpgradeService _abilityUpgradeService;
    private readonly IStaticDataService _staticData;

    private readonly IGroup<GameEntity> _abilities;
    private readonly IGroup<GameEntity> _heroes;
    private readonly IGroup<GameEntity> _enemies;

    private readonly List<GameEntity> _buffer = new(32);

    public VegetableBoltAbilitySystem(
        GameContext game,
        IArmamentFactory armamentFactory,
        IAbilityUpgradeService abilityUpgradeService,
        IStaticDataService staticData)
    {
        _armamentFactory = armamentFactory;
        _abilityUpgradeService = abilityUpgradeService;
        _staticData = staticData;

        _abilities = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.VegetableBoltAbility,
                GameMatcher.CooldownIsUp));

        _heroes = game.GetGroup(GameMatcher.Hero);
        _enemies = game.GetGroup(GameMatcher.Enemy);
    }

    public void Execute()
    {
        foreach (GameEntity ability in _abilities.GetEntities(_buffer))
        foreach (GameEntity hero in _heroes)
        {
            if (_enemies.count <= 0)
                continue;

            int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.VegetableBolt);
            AbilityLevel config = _staticData.GetAbilityLevel(AbilityId.VegetableBolt, level);

            GameEntity target = FindClosestEnemy(hero);
            Vector3 direction = (target.WorldPosition - hero.WorldPosition).normalized;

            _armamentFactory
                .CreateVegetableBolt(config.Damage, hero.WorldPosition)
                .AddProducerId(hero.Id)
                .ReplaceDirection(direction)
                .With(x => x.isMoving = true);

            ability.PutOnCooldown(config.Cooldown);
        }
    }

    private GameEntity FindClosestEnemy(GameEntity hero)
    {
        GameEntity closest = null;
        float minDistance = float.MaxValue;

        foreach (GameEntity enemy in _enemies)
        {
            float distance = Vector3.Distance(hero.WorldPosition, enemy.WorldPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }
}
```

### Система кулдаунов

```csharp
// Компоненты
[Game] public class Cooldown : IComponent { public float Value; }
[Game] public class CooldownIsUp : IComponent { }

// Extension
public static class CooldownExtensions
{
    public static void PutOnCooldown(this GameEntity entity, float time)
    {
        entity.ReplaceCooldown(time);
        entity.isCooldownIsUp = false;
    }
}

// Система
public class CooldownSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);
    private readonly ITimeService _time;

    public CooldownSystem(GameContext game, ITimeService time)
    {
        _time = time;
        _entities = game.GetGroup(GameMatcher
            .AllOf(GameMatcher.Cooldown)
            .NoneOf(GameMatcher.CooldownIsUp));
    }

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            entity.ReplaceCooldown(entity.Cooldown - _time.DeltaTime);

            if (entity.Cooldown <= 0)
            {
                entity.isCooldownIsUp = true;
                entity.RemoveCooldown();
            }
        }
    }
}
```

### Система эффектов

```csharp
// Компоненты эффектов
[Game] public class Effect : IComponent { }
[Game] public class DamageEffect : IComponent { }
[Game] public class HealEffect : IComponent { }
[Game] public class EffectValue : IComponent { public float Value; }
[Game] public class TargetId : IComponent { public int Value; }
[Game] public class ProducerId : IComponent { public int Value; }
[Game] public class Processed : IComponent { }

// Система обработки урона
public class ProcessDamageEffectSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _effects;
    private readonly List<GameEntity> _buffer = new(32);

    public ProcessDamageEffectSystem(GameContext game)
    {
        _effects = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.DamageEffect,
                GameMatcher.EffectValue,
                GameMatcher.TargetId)
            .NoneOf(GameMatcher.Processed));
    }

    public void Execute()
    {
        foreach (GameEntity effect in _effects.GetEntities(_buffer))
        {
            GameEntity target = effect.Target();

            effect.isProcessed = true;

            if (target.isDead)
                continue;

            target.ReplaceCurrentHp(target.CurrentHp - effect.EffectValue);

            if (target.hasDamageTakenAnimator)
                target.DamageTakenAnimator.PlayDamageTaken();
        }
    }
}

// Система очистки обработанных эффектов
public class CleanupProcessedEffects : ICleanupSystem
{
    private readonly IGroup<GameEntity> _effects;
    private readonly List<GameEntity> _buffer = new(32);

    public CleanupProcessedEffects(GameContext game)
    {
        _effects = game.GetGroup(GameMatcher
            .AllOf(
                GameMatcher.Effect,
                GameMatcher.Processed));
    }

    public void Cleanup()
    {
        foreach (GameEntity effect in _effects.GetEntities(_buffer))
        {
            effect.isDestructed = true;
        }
    }
}
```

### Создание эффектов

```csharp
public class CreateDamageEffect
{
    public static GameEntity Create(GameEntity target, float damage, GameEntity producer = null)
    {
        GameEntity effect = CreateEntity.Empty()
            .AddTargetId(target.Id)
            .AddEffectValue(damage)
            .With(x => x.isEffect = true)
            .With(x => x.isDamageEffect = true);

        if (producer != null)
            effect.AddProducerId(producer.Id);

        return effect;
    }
}

// Использование
CreateDamageEffect.Create(enemy, 10f, hero);
```

---

## Система ввода (Input)

### Input Feature

```csharp
public class InputFeature : Feature
{
    public InputFeature(ISystemFactory systems)
    {
        Add(systems.Create<InitializeInputSystem>());
        Add(systems.Create<EmitInputSystem>());
    }
}
```

### Инициализация Input Entity

```csharp
public class InitializeInputSystem : IInitializeSystem
{
    private readonly InputContext _input;

    public InitializeInputSystem(InputContext input)
    {
        _input = input;
    }

    public void Initialize()
    {
        _input.CreateEntity().isInput = true;
    }
}
```

### Чтение ввода

```csharp
// Компоненты
[Input] public class Input : IComponent { }
[Input] public class AxisInput : IComponent { public Vector2 Value; }
[Input] public class MousePosition : IComponent { public Vector3 Value; }

// Система
public class EmitInputSystem : IExecuteSystem
{
    private readonly IGroup<InputEntity> _inputs;
    private readonly List<InputEntity> _buffer = new(1);

    public EmitInputSystem(InputContext input)
    {
        _inputs = input.GetGroup(InputMatcher.Input);
    }

    public void Execute()
    {
        foreach (InputEntity input in _inputs.GetEntities(_buffer))
        {
            Vector2 axis = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical"));

            if (axis != Vector2.zero)
                input.ReplaceAxisInput(axis);
            else if (input.hasAxisInput)
                input.RemoveAxisInput();

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            input.ReplaceMousePosition(mousePos);
        }
    }
}
```

### Применение ввода к герою

```csharp
public class SetHeroDirectionByInputSystem : IExecuteSystem
{
    private readonly IGroup<GameEntity> _heroes;
    private readonly IGroup<InputEntity> _inputs;
    private readonly List<GameEntity> _heroBuffer = new(1);
    private readonly List<InputEntity> _inputBuffer = new(1);

    public SetHeroDirectionByInputSystem(GameContext game, InputContext input)
    {
        _heroes = game.GetGroup(GameMatcher.Hero);
        _inputs = input.GetGroup(InputMatcher.Input);
    }

    public void Execute()
    {
        foreach (InputEntity input in _inputs.GetEntities(_inputBuffer))
        foreach (GameEntity hero in _heroes.GetEntities(_heroBuffer))
        {
            hero.isMoving = input.hasAxisInput;

            if (input.hasAxisInput)
                hero.ReplaceDirection(input.AxisInput.normalized);
        }
    }
}
```

---

## Dependency Injection

### SystemFactory с DI контейнером

```csharp
public interface ISystemFactory
{
    T Create<T>() where T : ISystem;
}

public class SystemFactory : ISystemFactory
{
    private readonly DiContainer _container;

    public SystemFactory(DiContainer container)
    {
        _container = container;
    }

    public T Create<T>() where T : ISystem
    {
        return _container.Resolve<T>();
    }
}
```

### Регистрация зависимостей (Zenject/VContainer)

```csharp
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Contexts
        Container.Bind<Contexts>().FromInstance(Contexts.sharedInstance).AsSingle();
        Container.Bind<GameContext>().FromInstance(Contexts.sharedInstance.game).AsSingle();
        Container.Bind<InputContext>().FromInstance(Contexts.sharedInstance.input).AsSingle();

        // Services
        Container.Bind<ITimeService>().To<TimeService>().AsSingle();
        Container.Bind<IIdentifierService>().To<IdentifierService>().AsSingle();
        Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
        Container.Bind<IPhysicsService>().To<PhysicsService>().AsSingle();

        // Factories
        Container.Bind<IHeroFactory>().To<HeroFactory>().AsSingle();
        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
        Container.Bind<IArmamentFactory>().To<ArmamentFactory>().AsSingle();
        Container.Bind<IEntityViewFactory>().To<EntityViewFactory>().AsSingle();

        // System Factory
        Container.Bind<ISystemFactory>().To<SystemFactory>().AsSingle();

        // Systems
        Container.Bind<BattleFeature>().AsSingle();
    }
}
```

---

## Производительность и оптимизация

### 1. Переиспользование буферов

```csharp
// ✅ Правильно - буфер создается один раз
public class SomeSystem : IExecuteSystem
{
    private readonly List<GameEntity> _buffer = new(64); // Предвыделение

    public void Execute()
    {
        foreach (GameEntity entity in _entities.GetEntities(_buffer))
        {
            // Буфер переиспользуется каждый кадр
        }
    }
}

// ❌ Неправильно - аллокация каждый кадр
public class BadSystem : IExecuteSystem
{
    public void Execute()
    {
        foreach (GameEntity entity in _entities) // Создает аллокации
        {
        }
    }
}
```

### 2. Оптимальный размер буфера

```csharp
// Малые группы (1-10 сущностей)
private readonly List<GameEntity> _buffer = new(8);

// Средние группы (10-50 сущностей)
private readonly List<GameEntity> _buffer = new(32);

// Большие группы (50-200 сущностей)
private readonly List<GameEntity> _buffer = new(64);

// Очень большие группы (200+ сущностей)
private readonly List<GameEntity> _buffer = new(128);
```

### 3. Кэширование Matcher

```csharp
// ✅ Правильно - Matcher создается один раз в конструкторе
public SomeSystem(GameContext game)
{
    _entities = game.GetGroup(GameMatcher
        .AllOf(GameMatcher.Hero)
        .NoneOf(GameMatcher.Dead));
}

// ❌ Неправильно - Matcher создается каждый кадр
public void Execute()
{
    var entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero)); // Аллокация!
}
```

### 4. Entity Pooling

Entitas автоматически использует пулинг сущностей - не нужно ничего делать!

```csharp
// Entity создается из пула
GameEntity entity = game.CreateEntity();

// Entity возвращается в пул
entity.Destroy();
```

### 5. Избегайте GetComponent в циклах

```csharp
// ✅ Правильно - компоненты уже есть у сущности
foreach (GameEntity entity in _entities.GetEntities(_buffer))
{
    float speed = entity.Speed; // Быстро
    Vector3 pos = entity.WorldPosition; // Быстро
}

// ❌ Неправильно - Unity GetComponent медленный
foreach (GameEntity entity in _entities.GetEntities(_buffer))
{
    var rb = entity.View.GetComponent<Rigidbody>(); // Медленно!
}
```

### 6. Используйте Entity Indices для поиска

```csharp
// ✅ Правильно - O(1) поиск через индекс
GameEntity target = game.GetEntityWithId(123);

// ❌ Неправильно - O(n) поиск в цикле
GameEntity target = null;
foreach (var entity in game.GetEntities())
{
    if (entity.Id == 123)
    {
        target = entity;
        break;
    }
}
```

### 7. Профилирование

```csharp
// В Generated/Feature.cs автоматически добавляется профилирование
public class Feature : Systems
{
    public Feature(string name) : base(name) { }

    public override void Execute()
    {
        UnityEngine.Profiling.Profiler.BeginSample(name);
        base.Execute();
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
```

---

## Лучшие практики

### 1. Структура проекта

```
✅ Организуйте по Features
Assets/Code/Gameplay/Features/
    Hero/
    Enemies/
    Abilities/
    Movement/

✅ Группируйте компоненты по функциональности
HeroComponents.cs
MovementComponents.cs
CombatComponents.cs

✅ Один Context = одна область ответственности
Game - игровая логика
Input - ввод
Meta - мета-игра
```

### 2. Именование

```csharp
// ✅ Системы - глагол + существительное + System
MoveHeroSystem
SpawnEnemySystem
ProcessDamageEffectSystem

// ✅ Компоненты - существительное (без Component)
Speed
WorldPosition
Hero

// ✅ Features - существительное + Feature
HeroFeature
MovementFeature
AbilityFeature
```

### 3. Принципы разработки

```csharp
// ✅ Single Responsibility - одна система = одна задача
public class MoveSystem : IExecuteSystem  // Только движение
public class RotateSystem : IExecuteSystem  // Только поворот

// ❌ Не делайте
public class MoveAndRotateSystem : IExecuteSystem // Две задачи!

// ✅ Маленькие, простые системы лучше больших сложных
public class MarkDeadSystem : IExecuteSystem  // Проверка HP
public class ProcessDeadSystem : IExecuteSystem  // Обработка смерти

// ✅ Используйте двухфазное уничтожение
entity.isDestructed = true;  // Фаза 1: пометка
// ...
entity.Destroy();  // Фаза 2: cleanup система
```

### 4. Порядок систем в Feature

```csharp
public class SomeFeature : Feature
{
    public SomeFeature(ISystemFactory systems)
    {
        // 1. Инициализация
        Add(systems.Create<InitializeSystem>());

        // 2. Ввод
        Add(systems.Create<ReadInputSystem>());

        // 3. Игровая логика
        Add(systems.Create<GameLogicSystem>());

        // 4. Физика/Движение
        Add(systems.Create<MoveSystem>());

        // 5. Анимация
        Add(systems.Create<AnimateSystem>());

        // 6. Обновление View
        Add(systems.Create<UpdateViewSystem>());

        // 7. Очистка
        Add(systems.Create<CleanupSystem>());
    }
}
```

### 5. Работа с View

```csharp
// ✅ Используйте Registrar для Unity компонентов
public class TransformRegistrar : EntityComponentRegistrar
{
    public override void RegisterComponents()
    {
        Entity.AddTransform(transform);
    }
}

// ✅ Отделяйте View от логики
// Логика в системах, View только отображение

// ❌ НЕ добавляйте логику в MonoBehaviour
public class HeroView : MonoBehaviour
{
    void Update() // ❌ НЕ ДЕЛАЙТЕ ТАК
    {
        // Логика должна быть в системах!
    }
}
```

### 6. Компоненты

```csharp
// ✅ Компоненты - только данные
[Game] public class Speed : IComponent { public float Value; }

// ❌ НЕ добавляйте методы
[Game] public class BadSpeed : IComponent
{
    public float Value;
    public void IncreaseSpeed() { } // ❌
}

// ✅ Используйте флаги для состояний
[Game] public class Moving : IComponent { }
entity.isMoving = true;

// ✅ Используйте Value для одиночных значений
[Game] public class Speed : IComponent { public float Value; }
```

### 7. Фабрики

```csharp
// ✅ Создавайте сущности через фабрики
public class HeroFactory : IHeroFactory
{
    public GameEntity CreateHero(Vector3 at)
    {
        return CreateEntity.Empty()
            .AddId(_identifiers.Next())
            .AddWorldPosition(at)
            .With(x => x.isHero = true);
    }
}

// ❌ НЕ создавайте напрямую в системах
public class BadSystem : IExecuteSystem
{
    public void Execute()
    {
        var hero = game.CreateEntity(); // ❌
        hero.AddId(1);
        // ...
    }
}
```

### 8. Тестирование

```csharp
// Системы легко тестировать
[Test]
public void When_HP_Zero_Then_Entity_Marked_Dead()
{
    // Arrange
    var contexts = new Contexts();
    var system = new MarkDeadSystem(contexts.game);
    var entity = contexts.game.CreateEntity();
    entity.AddCurrentHp(0);

    // Act
    system.Execute();

    // Assert
    Assert.IsTrue(entity.isDead);
}
```

### 9. Советы по производительности

```csharp
// ✅ Переиспользуйте буферы
private readonly List<GameEntity> _buffer = new(32);

// ✅ Кэшируйте результаты
private readonly IGroup<GameEntity> _heroes;

// ✅ Используйте Entity Indices
[PrimaryEntityIndex] public int Value;

// ✅ Избегайте LINQ в Execute
// ❌ var first = entities.First();
// ✅ var first = entities.GetEntities(_buffer)[0];

// ✅ Batch операции
foreach (var entity in _entities.GetEntities(_buffer))
{
    // Обработать все сразу
}
```

### 10. Паттерны расширения функциональности

```csharp
// ✅ Добавление нового типа сущности
1. Создать компоненты в NewEntityComponents.cs
2. Создать NewEntityFactory
3. Создать системы в NewEntityFeature/Systems/
4. Добавить NewEntityFeature в BattleFeature

// ✅ Добавление новой способности
1. Добавить компонент в AbilityComponents.cs
2. Создать NewAbilitySystem
3. Добавить в AbilityFeature

// ✅ Добавление нового контекста
1. Создать [NewContext] атрибут
2. Создать компоненты с [NewContext]
3. Сгенерировать код (CodeGenerator)
4. Создать системы для нового контекста
```

---

## Заключение

### Ключевые принципы Entitas ECS

1. **Разделение данных и логики** - Компоненты содержат только данные, Системы - только логику
2. **Композиция** - Сущности строятся из компонентов, а не через наследование
3. **Модульность** - Features организуют системы в логические группы
4. **Производительность** - Переиспользование буферов, Entity Indices, кэширование
5. **Тестируемость** - Системы легко тестировать изолированно
6. **Кодогенерация** - Entitas генерирует удобный API автоматически

### Типичный workflow

1. Определить компоненты для новой функциональности
2. Создать системы для обработки компонентов
3. Объединить системы в Feature
4. Добавить Feature в корневую BattleFeature
5. (Опционально) Создать Factory для создания сущностей
6. (Опционально) Настроить View Binding через Registrars

### Дополнительные ресурсы

- Документация Entitas: https://github.com/sschmid/Entitas
- ECS Survivors (исходный проект): D:\Загрузки\ecs-survivors-viewers-main\ecs-survivors-viewers-main\src\ecs-survivors
- Визуальный дебаггер: Unity -> Window -> Entitas

### Кодогенерация Entitas

Для генерации кода Entitas необходимо выполнить команду в папке проекта (EntitasAiTest):

```bash
dotnet Jenny\Jenny.Generator.Cli.dll gen Jenny.properties -v
```

Эта команда запускает генератор кода Jenny, который создает:
- API для работы с компонентами (Add/Replace/Remove методы)
- Matchers для запросов к сущностям
- Entity Indices для быстрого поиска
- Контексты и другой вспомогательный код

**Важно:** Запускайте генерацию после каждого изменения компонентов!

---

**Удачи в разработке на Entitas! 🚀**
