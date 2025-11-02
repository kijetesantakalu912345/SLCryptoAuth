## Kijetesantakalu's SLCryptoAuth fork
MelonLoader client and labAPI server mod for SCP: Secret Laboratory to implement functional and secure authentication that does not require a connection to central servers. This also fixes remote admin.

My current goal with this fork is to port SLCryptoAuth to scopophobia and megapatch 2. Probably also some 13.x version(s), but only because somebody requested it and it's only one major version below 14 so it'll probably be a relatively easy place to start. I might port it to other stuff too but if I don't feel like running it on my server I likely won't bother.

### Downloading a specific game version
Use [DepotDownloader](https://github.com/SteamRE/DepotDownloader) to download specific manifests.  
You can use the wiki's updates page to find the date a specific version released, and then ctrl+f the steamdb page for that date to get the manifest you need.  
Server steamdb depot link: https://steamdb.info/depot/996561/manifests/  
Client steamdb depot link: https://steamdb.info/depot/700331/manifests/  
Official wiki's updates page: https://en.scpslgame.com/index.php?title=Updates

### Currently supported game versions
SCPSL v14.0.3-labapi-beta.
* Client manifest: 7041928528288606435  
* Server manifest: 1429960657914648293

To compile SLCryptoAuth yourself:
 1. Switch to the git branch for the version you're interested in.
 2. If the server plugin uses Exiled in your selected game version, you'll need to download the files for that version of Exiled and put the DLLs in `/libs/server<game version>/EXILED_v<Exiled version you need>`.  
  2a. This is to avoid direct licensing conflicts between Exiled's CC BY-SA 3.0 license and this project's license. Make sure that if you put Exiled stuff in any other directory that it's gitignored.

To set up SLCrypoAuth for a game versison there isn't a build for here:
 1. You will need to replace dependencies and recompile the code at a bare minimum.
 2. You may need to fix any incompatabilities that may exist between the mod and the new game version. You'll probably just have to test if and see if anything breaks.
## Acknowledgements

1. Some patches and/or parts of the code in this project were borrowed or based on work from the AxonSL project. The AxonSL project is licensed under the MIT license. The original copyright notice and AxonSL license text can be found here: https://github.com/AxonSL/Axon/blob/master/LICENSE.txt

2. While apparently not used for the original project, I (kijetesantakalu) am using [SCPSL-ModPatch](https://github.com/hopperlopip/SCPSL-ModPatch) to remove the client's anti-cheat, which allows melonloader to run. https://github.com/hopperlopip/SCPSL-ModPatch

3. MelonLoader v1.0.0-ci.2176 ALPHA Pre-Release is used to build the client. For client installation, use [the latest melonloader loader](https://melonloader.co/download) version (0.7.0 or 0.7.1 openbeta at the time of writing).

4. While Exiled is not directly included in this repo for licensing reasons, here are the links to the repos for old and new Exiled. You'll need one of these to compile the server plugin if SLCryptoAuth uses Exiled on your target game version.  
 4a. Old Exiled: https://github.com/Exiled-Team/EXILED  
 4b. New Exiled: https://github.com/ExMod-Team/EXILED  

(the english version of this readme has parts from the original russian readme that were machine translated using deepl.com)
<hr>

## Original russian readme (which kijetesantakalu can't read)
MelonLoader модификация на SCP: Secret Laboratory для реализации рабочей и безопасной аутентификации, не требующей подключения к центральным серверам.

Было написано и протестировано на версии SCPSL v14.0.3-labapi-beta + MelonLoader v1.0.0-ci.2176 ALPHA Pre-Release.

SteamDepotsDownloader:
Манифест клиента: 7041928528288606435
Манифест сервера: 1429960657914648293

Для каждой версии игры нужно заменять зависимости и пересобирать код.

В будущем планирую провести рефакторинг кода, написать документацию, оформить репозиторий, настроить GitHub Actions и предоставить готовые к использованию релизу.

## Заимствования и Благодарности (Acknowledgements)

Некоторые патчи и/или части кода в этом проекте были позаимствованы или основаны на работе из проекта [AxonSL](https://github.com/AxonSL/Axon).
Проект AxonSL лицензирован на условиях лицензии MIT. Оригинальное уведомление об авторских правах и текст лицензии AxonSL можно найти здесь:
[https://github.com/AxonSL/Axon/blob/master/LICENSE.txt](https://github.com/AxonSL/Axon/blob/master/LICENSE.txt)
