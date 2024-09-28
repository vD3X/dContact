## Description:
**dContact** is a plugin that enables players to easily contact server admins. It features a dynamic contact list, allowing admins to provide multiple communication methods such as Steam, Discord, and others directly from a configurable JSON file. Players can access this information through menu.

## Preview Images:
![Main Menu](https://github.com/vD3X/dContact/blob/main/mainmenu.png)
![Chat Message](https://github.com/vD3X/dContact/blob/main/chatmessage.png)

## Configuration:
```
{
  "Settings": {
    "Contact_Command": "contact, kontakt", // Commands to open the menu (!contact, !kontakt)
    "Title": "[ ★ CS-Zjarani | Contacts ★ ]", // Title of the menu
    "TitleColor": "#29cc94" // Color of the title
  },
  "AdminContacts": [
    {
      "name": "D3X", // Name of the admin displayed in the menu (Required)
      "rank": "Server Owner", // Rank of the admin displayed in the menu (Required)
      "Contacts": {
        "Steam": "https://steamcommunity.com/id/dd3xx", // Steam profile displayed in chat (can be removed if not needed)
        "Forum": "https://cs-zjarani.pl", // Forum profile displayed in chat (can be removed if not needed)
        "Discord": "dd3xx", // Discord account displayed in chat (can be removed if not needed)
        "Other Contact": "anotherContact123" // Other contact method (can be added as needed)
      }
    },
    {
      "name": "av", // Name of the second admin displayed in the menu
      "rank": "Server Guardian", // Rank of the second admin displayed in the menu
      "Contacts": {
        "Steam": "https://steamcommunity.com/id/76535499555", // Steam profile of the second admin
        "Forum": "https://cs-zjarani.pl", // Forum profile of the second admin
        "Discord": "av45" // Discord account of the second admin
      }
    }
  ]
}

```
