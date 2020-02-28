![](https://raw.githubusercontent.com/peeweek/net.peeweek.gameplay-ingredients/master/Documentation%7E/Images/site-banner.png)

[![openupm](https://img.shields.io/npm/v/net.peeweek.gameplay-ingredients?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.peeweek.gameplay-ingredients/) [![Documentation Status](https://readthedocs.org/projects/pip/badge/?version=stable)](http://pip.pypa.io/en/stable/?badge=stable)

Gameplay Ingredients for your Unity Games - A collection of scripts that ease simple tasks while making games and prototypes.

<u>You can read Documentation at this address :</u> [https://peeweek.readthedocs.io/en/latest/gameplay-ingredients/](https://peeweek.readthedocs.io/en/latest/gameplay-ingredients/)

## Requirements

* **Unity 2019.3** (Older versions compatible with 2018.3 / 2019.1 / 2019.2)
* Package Manager UI
* (**Optional** : Command-line Git installed on your system, for example [Git For Windows](https://gitforwindows.org/))

## How to install

### Install as Local Package

You can use a manual, local package installation if you need to alter the code locally or update the code base regularly. 

- Clone this repository somewhere of your liking
- In your project, open the `Window/Package Manager` window and use the + button to select the `Add Package from Disk...` option.
- Navigate to your repository folder and select the `package.json` file
- The package shall be added as a **local package**

### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add net.peeweek.gameplay-ingredients
```

### Git Reference Version

- Ensure you have a **[Command Line Git](https://gitforwindows.org/) Installed**
- With Unity 2019.3 closed, edit the `Packages/manifest.json` with a text editor
- Append the line `    "net.peeweek.gameplay-ingredients": "https://github.com/peeweek/net.peeweek.gameplay-ingredients.git#2019.3.3",` under `dependencies`

You can check that the package was imported by looking at the project window, under Packages/ Hierarchy, there should be a `Gameplay Ingredients` hierarchy

# Version / Tag Compatibility

Gameplay Ingredients comes at latest version with the following compatibility:

**Unity 2019.3** : clone and check out the `master` branch at the tag `2019.3.3` 

#### Older Versions

* **Unity 2018.3 / 2018.4 :** choose the tag `2018.3.0`
* **Unity 2019.1 / 2019.2 :** choose the tag `2019.1.2` 
