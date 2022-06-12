# Show If

### A property attribute to make comparisions on editor before displaying a value

The `[ShowIf]` attribute will allow you to hide or show a serialized attribute when needed

It can be used for checking a sibling boolean field

```c#
public bool useReferencedPrefab;
[ShowIf("useReferencedPrefab")]
public GameObject prefab;
```

Also the boolean condition can be inverted to get a "show if not"

```c#
[ShowIf("useReferencedPrefab",invert:true)]
public string resourcesPath;
```

Also by using the comparers

```c#

public WeaponShootingType shootingType;
[ShowIf("shootingType","==",WeaponShootingType.PhysicProjectile)]
public ProjectileConfig projectileConfig;
[ShowIf("shootingType","==",WeaponShootingType.Raycast)]
public RaycastConfig raycastConfig;
```

You can compare with a number too

```c#
public float spreading;
[ShowIf("spreading",">",0)]
public SpreadSettings spreadSettings;
```

NOTE: allowed comparers are:

```c#
"==", "!=", ">", ">=", "<", "<="
```
