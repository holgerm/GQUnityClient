Media in Quests                		{#MediaInQuests}
===============


# Loading Strategies (also for Updating)

There are three loading (or updating) strategies for media in quests:

1. Upfront - All media files are downloaded (or updated) before the quest can be started.
2. Lazy - The media files are only loaded (or updated) when they need to be shown or played. No further media files are downloaded in the background.
3. Background - Only those media files needed for the first page are loaded (or updated) and then the quest can be started. The rest in downloaded (or updated) in the background.

The strategy can be determined as well by the app in general as by the quest in particular. The app-wide setting works as default that can be overridden by the quest-specific setting. The total default is startegy 1.

At the first step only the first and the second strategy are realized. The third strategy will be realized later, because it needs loading in the background.


# Loading Algorithm

- Download und Update von Quests:
    - Falls Update: Questverzeichnis in <questid>-Old umbenennen und neues lokales Verzeichnis anlegen für Quest.
    - Download der game.xml
    - Deserialisieren der Game.Xml Datei und Erstellen des Runtime Models (Quest als Root)
    - Durchsuchen der game.xml nach Mediadateien => Liste aller URLs (ToDownload-Liste)
    - Falls Update: 
        - Einlesen des persistierten Dictionaries aller Mediendateien (URL => Timestamp, Größe) aus der game-media.json Datei
        - Kopie der Keys (URLs) in ein Todo Dictionary (URL => Delete) 
        - Alle Einträge aus der ToDownload Liste durchgehen und 
            - wenn in Todo Dictionary älter, ändern auf (URL => Delete and Load) und aus ToDownload Liste löschen
            - wenn in Todo Dictionary nicht älter  ändern auf (URL => Keep) und aus ToDownload Liste löschen
            - wenn nicht in Todo Dictionary, belassen bei URL => Delete) und in ToDownload Liste belassen.
    - ToDownload Liste durchgehen und für jede Datei einen Eintrag in neues ToDownload Dictionary machen: (URL => Timestamp, Größe)
    - Falls Update: ToDownload Dictionary durchgehen und für alle Einträge mit Delete and Load als Key, neue MadiaInfos in das ToDownload Dictionary eintragen.
    - Multidownloader starten mit Summe der Größen aus dem ToDownload Dictionary und allen dort genannten URLs. Zieldateien werden in das neue Verzeichnis gespeichert und unifiziert (Dateinamen plus ggf. Nummern). Dieser Name wird im ToDownload Dictionary eingetragen als Teil der jeweiligen Values. Diese sind vom Typ MediaInfo.
    - Nach erfolgreichem Abschluss der Downloads, falls Update: Todo Dictionary durchgehen und:
        - Wenn Keep dann alte lokale Datei in das neue Verzeichnis kopieren / verschieben und dazu MediaInfo der alten Serialisierung in das ToDownload Dictionary eintragen
        - Altes Verzeichnis komplett löschen 
    - game-media.json als Serialisierung der ToDownload erstellen
