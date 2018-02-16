# legend-of-iris
In this fork of the [initial project](https://github.com/audio-game-crew/legend-of-iris) the focus is on removing the dependency of a VR-hreadset from the game. This in order to make the game more accessible to the target audience. As the headset was used solely for tracking the movement of the head, we needed a (cheap and widely available) device that would support this. The device we decided on for this is the smartphone, for obvious reasons. This also has the advantage that we can use the headphone jack (at that time a given port) for the audio output, making the experience truly wireless.

First we started on making the smartphone act as an input-device by measuring the movement and sending this to the main game. Later we took this a step further and created a working Android port of the game. For a release of the port, please see the release section for a quick download or compile it yourself for a (little more) recent version.

## AVR-Link
When making a smartphone act as an input device, three steps are needed. You need to capture the inputs form the device, send them to the machine running the game and apply the received values to actual movement in-game. Gathering the input data is relatively simple. Two buttons are placed on the screen to walk forward and backward and a standard API is used to measure the device rotation. On the receiving side a class `AudioVrControls` was added for converting the received data to movement. This class was accompanied by the `AudioVrLink` class to handle the data connection. This connection was made over sockets and required the user to type in a so-called 'connect-code' to connect to the game from the phone. For the intended audience, this is not ideal as they cannot easily type in the code and read the code displayed by the game. We therefore opted to change the communication from WiFi to Bluetooth as the pairing would make the setup a one time operation. The work on this improvement was abandoned in favor of the full port.

More information on the workings of the AVR-Link can be found on the `AudioVRLink` branch and on the repository of the [AudioVRLink app](https://github.com/ChielBruin/AudioVRLink).

## Android Port
As the game is written in Unity, porting a game to a different platform is possible. For the port to Android, two changes needed to be made. First we needed to change the controls of the game to allow for use on a touchscreen. Secondly, we needed to reduce the installation size as the original game resulted in an .apk-file of over 400MB.

This second step was the most straightforward. The major factor in the size of the game were the audio files used. These files were all stored in an uncompressed format resulting in large files. As the audio is mostly based on spoken language, compressing the audio and the resulting quality loss could be justified. In Unity you can simply change the compression methods of all audio files. This has the advantage that the original sources are not changed, and the quality can be raised when desired (for example a hifi-audio version). This change reduced the size of the game to 140MB, but can be lowered further as this still uses relatively high quality MP3.

Changing the controls required some more work. The controls are handled by instances of the `BaseControls`-class, which are managed by a `ControlsManager`. A new class `MobileControls` was added to handle the new inputs. The manager was then altered to take this new input method as a default when running Android. With only these changes, you are still not able to play the game, as they do not allow you to progress through the introduction. For this it was needed to make changes to the `PressAnyKeyQuest`, `KeyOrderQuest`, `QuestManager` and `ConversationManager`. This makes it that the user can complete the first input dialogs by tapping the screen and swiping left/right/up/down, skip conversations by a two finger tap and enter some cheat codes with 4 or more fingers. A possible improvement for this code would be to refactor the code in such a way that all the handling of inputs goes via instances of `BaseControls`, making it easier to change or add new controls. 

For the controls of the main game two input types are required: the rotation of the player and the movement vector. For the rotation either the gyroscope or the compass of the device is used. The first allows for more precision but the second is present on more (lower-end) devices. The code checks which is available and uses this sensor during the game. Usage of the gyroscope also allows the game to distinguish all three axis of rotation, but due to the way the game is coded only one of them is currently in use. Refactoring to allow this more fine grained rotation, might improve the experience of the users. For the movement vector there are also two methods of input. A player can press the screen to walk or use an external input device like a presentation-pen. When pressing the screen, the location of the finger is converted to a forward/backward movement. Here the upper half corresponds to forward and the lower half to backward. For the external input methods, frequently used keys by these devices are bound to forward and backward.

## Documentation
The original game did not include much documentation, which made the development harder than it should have been. To improve the code base on this subject, comments were added on all the changed and added files. Much work could however be done on this subject. 
