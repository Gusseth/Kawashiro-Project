<img src="https://raw.githubusercontent.com/Gusseth/Kawashiro-Project/master/Kawashiro%20Project/Resources/logo.png">
<br>

# The Kawashiro Project - A Discord Bot for /r/weather

## An Overview
*Nitori* - The default namesake of the bot agent, will be filling in the gap brought on by MEE6.\
After surviving the meme hell that is CPSC 121 and 210, I am returning to C# to make a bot for the server.


## Goals and User Stories
The following list will be the main scope for Nitori:
- ✔️ ~~Clearing the 🎶djs channel once everyone leaves all voice channels~~
- ✔️ ~~Be able to delete ANY messages without the 2-week limit by MEE6, if possible~~
- ✔️ ~~Be able to delete any messages between a given message ID so deletes are more precise~~
- ✔️ ~~A mutiple choice poll system where you can vote by emoting to the message~~
- ✔️ ~~Post some dank memes as the successor to *Anarkali* like !flexmoney~~
- ✔️ ~~Limited local file transfer to host.~~

## Current Features:
- Fully Customizable reponses and partially-customizable embeds.
- Old message bulk deletion i.e. messages more than 2 weeks old.
- Deletion by messageID - no more spamming !clear n only to overshoot and delete important messages.
- Upload any local files less than 8MB from a folder.
- Polls of multiple choice.

## Commands:
- 🗑️delete/del `<int> or <messageID>`. Needs Manage Message permission. Deletion by messageID is **EXCLUSIVE** meaning the message where the ID belongs to __will not be deleted__. Works even with messages older than 2 weeks old! Uses the API-given BulkDelete to delete messages newer than 2 weeks old, then individually deletes older messages. Blame the Discord API for this.
- 🎚️autoclear/ac `Administrator:<channel mention> or Administrator:<channelID> or <none>`. Toggles the channel for auto-clearing once everyone disconnects from every voice channel. If no args are given, displays all channels that are in the list.
- 📊poll `<title> <prompt> <"options">`. Creates a 20 second poll. Emotes are customizable and number of options are limited to the amount of emotes defined. Surround quotation marks to "multi-word statements" to pass it as one argument.
- 📣echo `<any length string>`. Bot parrots whatever you say. Mentions work and standard Discord markdown works.
- 🌿weed `<filename> or <none>/<"dir">`. Uploads a file from data/media that is less than 8 MB. If no args or "dir" was passed as an argument, then displays the files in the media folder.
- 🏓ping `<none>`. Bot replies with "Pong!" and the latency between the bot and the API service.
- 🤑flexmoney `<number>`. Bot will always respond with a value higher than yours by 1 to 2^31. RegEx checked, will ignore non-digits and non-negative characters. If a negative value is inputted, the bot will berate you for being broke. If an input with no numbers is inputted, the bot will call out your weird flex.
- ↩️reload `Bot Owner:<none>`. Bot will re-read config.json, lines.json, embeds.json, and guilds.json.
- 📄embed `<key> or <"parse"> <json>`. Bot will load the given key defined in embeds.json. If "parse" and a [properly formatted](https://leovoel.github.io/embed-visualizer/) json object is inputted, then it loads the given json object into an embed.
