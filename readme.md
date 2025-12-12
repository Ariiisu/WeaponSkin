# WeaponSkin
A module/plugin for ModSharp that allows players to apply econ items, including but not limited to weapon paintkit, stickers, musickits, agents, and more.

# Installation

1. Download the latest release from the [releases](https://github.com/Ariiisu/WeaponSkin/releases) page.
2. Extract the contents of the zip file to the `{your_server_dir}/game/` directory, which should contain a sharp directory.
3. Edit the `sharp/configs/weaponskin.jsonc` file to configure the database connection.

# Request module

The request module is required for this module to work. If you have your own request implementation that does not rely on SQL, for example HTTP requests, you can make your own request module, but it must implement the same interface as the one provided by the request module.