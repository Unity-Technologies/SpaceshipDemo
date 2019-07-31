![](https://raw.githubusercontent.com/peeweek/net.peeweek.gameplay-ingredients/master/Documentation%7E/Images/site-banner.png)

Gameplay Ingredients for your Unity Games - A collection of scripts that ease simple tasks while making games and prototypes.

## Requirements

* Unity 2018.3
* Package Manager UI

## How to install

You can use a manual, local package installation if you need to alter the code locally or automate the fetch of the repository by using a git address directly. The latter option shall download and manage automatically the repository, with the drawback of being read-only.

### Manual Version

- Clone repository somewhere of your liking
- In your project, open the `Window/Package Manager` window and use the + button to select the `Add Package from Disk...` option.
- Navigate to your repository folder and select the `package.json` file
- The repository shall be added

### Git reference version

- With unity closed, edit the `Packages/manifest.json` with a text editor
- append the line `    "net.peeweek.gameplay-ingredients": "https://github.com/peeweek/net.peeweek.gameplay-ingredients.git#2018.3.0",` under `dependencies`

You can check that the package was imported by looking at the project window, under Packages/ Hierarchy, there should be a `Gameplay Ingredients` hierarchy
