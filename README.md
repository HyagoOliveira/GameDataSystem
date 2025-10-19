# Game Data System

* Game Data Serialization for Unity Games
* Unity minimum version: **6000.1**
* Current version: **0.5.1**
* License: **MIT**
* Dependencies:
	- [com.actioncode.persistence : 5.0.0](https://github.com/HyagoOliveira/Persistence/tree/5.0.0/)

## Summary

System to manager local and cloud CRUD (CReate, Update, Delete) operations into a serialized Game Data ScriptableObject object based on slot indexes.

You can Save, Load and Delete the Game Data files, both locally and remotely based on a Cloud Provider implementation.

On Builds, all the files are encrypted and compressed using the [Persistence](https://github.com/HyagoOliveira/Persistence) package. You can choose which serialization method to use (Json, XML or Binary).

To start to persisting data, you need first to create a serialized GameData class and a GameDataManager to manager this class.

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

Create an asset for the `GameData`.

![The GameData Asset](/Docs~/GameData.png "The GameData Asset")

The serialized fields are based in a commom slot based GameData file. No need to set them manually because the Game Data Manager will do that.

In large projects, you can easily find the GameData asset by going into the Tools > Find > GameData.

![The Find Game Data](/Docs~/FindGameData.png "Find Game Data")

You should use the `GameData` asset in every gameplay component that needs to load from it:

```csharp
[SerializeField] private GameData data;
[SerializeField] private Text highScore;

private void ShowHighScore() => highScore.value = data.HighScore;
```

> Be careful when using GameObjects loaded from Addressables! 

Addressables System loads the instance based when the files group were build. If the GameDataManager updates the GameData during runtime, those changes will not be present on the GameData referenciated by your GameObject dynamically loaded from Addressable when the game is running from Builds.

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

Create an asset for the `GameDataManager`.

![The GameDataManager Asset](/Docs~/GameDataManager.png "The GameDataManager Asset")

Link your GameData and a PersistenceSettings asset. Check [Creating the Persistence Settings](https://github.com/HyagoOliveira/Persistence?tab=readme-ov-file#creating-the-persistence-settings) for more details.

In the Editor, use the Game Data Manager buttons to locally Save, Load (also from a file) and Delete the referenced GameData. Save, Load and Delete buttons use the Current Slot to do the operation. The Load File button will load a normal or encrypted file into the Current Slot as well.

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

Following those [instructions](https://github.com/HyagoOliveira/Persistence?tab=readme-ov-file#checking-the-persisted-data) to know how to check the Persisted Data.

## Cloud Save

To persist data in the Cloud, just select the Cloud Provider in the GameDataManager asset (only Unity Cloud for now):

![The GameDataManager Asset](/Docs~/CloudProvider.png "The CloudProvider")

No code change is necessary. You can use the current code to save and delete data. Loading is done just locally for now.

### File Upload/Download

You can upload a Game Data file from a given slot to the Cloud. The file will be uploaded using Public Access so it can be downloaded later by other user.

Use the asynchronous functions `UploadAsync` and `DownloadAsync`:

```csharp
 private async void OnUploadClicked()
 {
     try
     {
         await dataManager.UploadAsync(); // Uploads the current GameData into the Cloud, using your Cloud User Id
     }
     catch (Exception e)
     {
         Debug.LogException(e);
     }
 }

private async void OnDownloadClicked()
{
    try
    {
		var slot = 0;
		var userId = await dataManager.GetUserIdAsync(); // Or use the Cloud Id from other User
        await dataManager.DownloadAsync(slot, userId); // Downloads the CloudData and places it into the slot 0
        var wasLoaded = await dataManager.TryLoadAsync(slot); // Loads from the slot 0
    }
    catch (Exception e)
    {
        Debug.LogException(e);
    }
}
```

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
