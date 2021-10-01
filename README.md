<p align="center">
  <img width="128" height="128" src="https://user-images.githubusercontent.com/14122279/135566373-b598c0c5-2a87-41e9-b7e8-2717ce378e52.png">
</p>

# Greenery Discord BOT

Greenery is a very small discord mini-game farming simulator. You can plant seeds that will grow based on your time active on the voice channels. Your seeds will require water, fertilizer and some care. Upon maturing you can harvest them for gold and experience. The shop allows your to get new items and seeds. Random events might occur which will give you random items or buffs in case you're arround when they happen.

## Usage

To use greenery just type ***!g-menu***. That should give you the access to all the functionality you need to play the mini game. **This is intended for very smalls servers since it is heavy on Discord's API usage for all the flowy interaction**.

![greenery-menu](https://user-images.githubusercontent.com/14122279/135566454-eb3a5743-aa18-4285-905a-aeacbb243c05.PNG)

## Installation

This assumes you have basic understanding of .net, containers and discord apps development.

- Clone the git repository
- Configure your data
- Configure your app.config, especially the *DiscordKey* value
- Copy data folder from Resources to build output folder
- Build the docker image (will need config if not using default folder)
- Deploy wherever you want

Alternatively you can use the included *docker-compose* file. 
**Don't forget to mount a volume for *storage* folders, since everything is stored on files and will be lost upon container shutdown otherwise**

## Configuration

**Configure the app**
- DiscordKey: Your discord bot key. Get it from the discord development portal
- CommandPrefix: Prefixes used for the commands. Default is !g-
- SaveTimeInterval: How many seconds between each data save
- TimeTickInterval: How many seconds between each tick for plant grow
- EventTickInterval: How many seconds between each tick for events

**Add items, plants, and others**

Edit files on data folder. I wish fields are self-descriptive enough
- Item.txt to edit items
- PlantVariety.tx to add more plants
- Shop.txt to add items to the shop (Less than 50 in total)

## Ideas (AKA things that will never be done)

- Make shop have random items each day 
- Seasons for harvesting mechanics
- Talents based on level mechanics
- Vegetables quality mechanics
- Daily gift to other channel memebers
- Contests event (based on vegetable quality)
- Personal and server achievements
- Pagination for seed selection, store and inventory
- And yeah... use a better storage method

## Contributing
Please feel free to fork or make any pull requests to expand the features available. Also, you can join our [Discord](https://discord.gg/nddUMHjCFC) in case you have any questions.
