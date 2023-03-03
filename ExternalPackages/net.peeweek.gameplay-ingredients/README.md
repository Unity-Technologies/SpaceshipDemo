![](https://raw.githubusercontent.com/peeweek/net.peeweek.gameplay-ingredients/master/Documentation%7E/Images/site-banner.png)

[![openupm](https://img.shields.io/npm/v/net.peeweek.gameplay-ingredients?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.peeweek.gameplay-ingredients/) [![Documentation Status](https://readthedocs.org/projects/pip/badge/?version=stable)](http://pip.pypa.io/en/stable/?badge=stable)

Gameplay Ingredients for your Unity Games - A collection of scripts that ease simple tasks while making games and prototypes.

<u>You can read Documentation at this address :</u> [https://peeweek.readthedocs.io/en/latest/gameplay-ingredients/](https://peeweek.readthedocs.io/en/latest/gameplay-ingredients/)

## Requirements

* **Unity 2020.3** for latest version 
  * (Older releases are still compatible with 2018.3 / 2019.1 / 2019.2 / 2019.3 / 2020.3)

* (**Optional, for older versions or development** : Command-line Git installed on your system, for example [Git For Windows](https://gitforwindows.org/))

## How to install (2020.2 and Newer)

* In Unity, Open **Project Settings** Window (Edit/Project Settings) and navigate to **Package Manager**
* Add a new **Scoped Registry** that references the [openupm registry](https://openupm.com): `https://package.openupm.com`
* Add the following scopes to the OpenUPM Scoped Registry : `com.dbrizov`, `net.peeweek` 

![](https://raw.githubusercontent.com/peeweek/net.peeweek.gameplay-ingredients/master/Documentation%7E/Images/project-settings.png)

* Open the Package Manager window (Window/Package Manager) and Select **Packages : My Registries** in the toolbar.
* Select Gameplay Ingredients in the list, then click the Install Button

## How to install as Local Package (2020.2 and Newer)

* Clone the repository.
* In Unity, Open **Project Settings** Window (Edit/Project Settings) and navigate to **Package Manager**
* Add a new **Scoped Registry** that references the [openupm registry](https://openupm.com): `https://package.openupm.com`
* Add the following scopes to the OpenUPM Scoped Registry : `com.dbrizov`, `net.peeweek` 

* Open the Package Manager window (Window/Package Manager)
* Click the plus button, select "Add Pacakge from disk" and locate the `package.json` file located at the root of the package.

## How to install (2019.3.x and Older)

### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add net.peeweek.gameplay-ingredients
```

### Git Reference Version

- Ensure you have a **[Command Line Git](https://gitforwindows.org/) Installed**
- With Unity 2019.3 closed, edit the `Packages/manifest.json` with a text editor
- Append the line `    "net.peeweek.gameplay-ingredients": "https://github.com/peeweek/net.peeweek.gameplay-ingredients.git#2019.3.7",` under `dependencies`

You can check that the package was imported by looking at the project window, under Packages/ Hierarchy, there should be a `Gameplay Ingredients` hierarchy

# Version / Tag Compatibility

Gameplay Ingredients comes at latest version with the following compatibility:

**Unity 2021.3 +** : choose the tag  : ![openupm](https://img.shields.io/npm/v/net.peeweek.gameplay-ingredients?label=openupm&registry_uri=https://package.openupm.com)

#### Older Versions

* **Unity 2018.3 / 2018.4 :** choose the tag `2018.3.0`
* **Unity 2019.1 / 2019.2 :** choose the tag `2019.1.2` 
* **Unity 2019.3 / 2020.1**  : choose the  the tag `2019.3.7` 
* **Unity 2020.3**  : choose the  the tag `2020.2.11` 
