# Game Data System

* Game Data Serialization for Unity Games
* Unity minimum version: **6000.1**
* Current version: **0.5.1**
* License: **MIT**
* Dependencies:
	- [com.actioncode.persistence : 4.0.0](https://github.com/HyagoOliveira/Persistence/tree/4.0.0/)

## Summary

System to manager local and cloud-based CRUD (CReate, Update, Delete) operations into a serialized Game Data ScriptableObject object.

The serialized class is based in a slot-index so you can Save, Load and Delete Game Data files, both locally and remotely in the cloud.

To start to persist data, you need first to create a serialized Game Data class and a GameDataManager to manager this class.

## Creating Serialized Assets

### Creating the Game Data

Create a class extending from [AbstractGameData](/Runtime/AbstractGameData.cs):

```csharp
using UnityEngine;
using ActionCode.GameDataSystem;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data", order = 110)]
public sealed class GameData : AbstractGameData
{
    public string HighScore;
}
```

`AbstractGameData` is an abstract `ScriptableObject`, so your class should use the `CreateAssetMenu` attribute.

Create an asset for `GameData`.

![The GameData Asset](/Docs~/GameData.png "The GameData Asset")

Those serialized fields are based in a commom slot based GameData file. No need to set them. The Game Data Manager will fill them.

In large projects, you can easily find the GameData asset by going into the Tools > Find > GameData.

![The Find Game Data](/Docs~/FindGameData.png "Find Game Data")

### Creating the Game Data Manager

Now create a GameDataManager class extending from [AbstractGameDataManager<T>](/Runtime/AbstractGameDataManager.cs):

```csharp
using UnityEngine;
using ActionCode.GameDataSystem;

[CreateAssetMenu(fileName = "GameDataManager", menuName = "Game/Game Data Manager", order = 110)]
public class GameDataManager : AbstractGameDataManager<GameData> // <- Put here your AbstractGameData implementation
{
}
```

`AbstractGameDataManager` is also an abstract `ScriptableObject`, so you should use the `CreateAssetMenu` attribute as well.

Create an asset for `GameDataManager`.

![The GameDataManager Asset](/Docs~/GameDataManager.png "The GameDataManager Asset")

Link your GameData and a [PersistenceSettings](https://github.com/HyagoOliveira/Persistence?tab=readme-ov-file#creating-the-persistence-settings) asset.

In the Editor, use the Game Data Manager buttons to locally Save, Load (also from a file) and Delete the referenced GameData, using the Current Slot to do the operation.

> Cloud Data persistence will not work using those buttons since the game should be running.

## Persist Data in Runtime

When the game is running, you can persist data as follow:

```csharp
using System;
using UnityEngine;

public class GameDataController : MonoBehaviour
{
	[SerializeField] private GameDataManager dataManager;

	private async void OnSaveClicked()
	{

	    try
	    {
	        await dataManager.SaveAsync(); // Saves in the current slot
	    }
	    catch (Exception e)
	    {
	        Debug.LogException(e);
	    }
	}

	private async void OnLoadClicked()
	{
	    try
	    {
	        var wasLoaded = await dataManager.TryLoadFromLastSlotAsync(); // or TryLoadAsync(slotIndex)
	        if (wasLoaded) Debug.Log("Game data was loaded.");
	    }
	    catch (Exception e)
	    {
	        Debug.LogException(e);
	    }
	}

	private async void OnDeleteClicked()
	{
	    try
	    {
	        await dataManager.DeleteAsync(slot: 0); // or DeleteAllAsync
	    }
	    catch (Exception e)
	    {
	        Debug.LogException(e);
	    }
	}
}
```

Always use a `try-catch` block to handling exceptions that may happen when saving, loading or deleting.

Following those [instructions](https://github.com/HyagoOliveira/Persistence?tab=readme-ov-file#checking-the-persisted-data) to know how to checking the Persisted Data.

## Cloud Save

To persist data in the Cloud, just select the Cloud Provider in the GameDataManager asset (only Unity Cloud for now):

![The GameDataManager Asset](/Docs~/CloudProvider.png "The CloudProvider")

No code change is necessary. You can use the current code to save and delete data. Loading is done just locally for now.

### File Upload/Download

You can upload the Game Data from the given slot to the Cloud. The file uses Public Access so it can be downloaded later by other user.

Use the asynchronous functions `UploadAsync` and `DownloadAsync`.

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-Game Data System** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/GameDataSystem.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.game-data-system":"https://github.com/HyagoOliveira/GameDataSystem.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>