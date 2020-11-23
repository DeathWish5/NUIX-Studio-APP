# IoThingsLab

Sorry everyone, your stars have disappeared as well as releases of the platform. But to make the repository standalone and super-lightweight I had to recreate it. The previous version can be found at https://github.com/FedorIvachev/IoThingsBeautifulLab

## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Required software and hardware](#required-software-and-hardware)
* [IoT VR Platform package](#iot-vr-platform-package)
* [Setup](#setup)
* [Known issues](#known-issues)
* [Contributing to the platform](#contributing-to-the-platform)

## General info

<img align="left" width="200" src="https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/20201030_173803.jpg">
<img align="left" width="200" src="https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/20201030_175023.jpg">

By using VRSimulator platform researchers can connect real and virtual IoT devices, test new IoT devices inside VR environment (and don't even need to buy them!)

[:VRSimulator Webpage:](https://vrsimulator.github.io/)
	
## Technologies
IoThingsLab uses the features of [Microsoft's Mixed Reality Toolkit](https://github.com/microsoft/MixedRealityToolkit-Unity#feature-areas), such as hand tracking and interaction techniques. 

Connection to real-world IoT devices is performed by REST API calls to [openHAB](https://www.openhab.org/download/) server, which runs either locally on user machine, or remotely on [myopenhab server](http://myopenhab.org/).
## Required software and hardware:
1. Unity Version 2019.4.13+
### Additional software & hardware:
1. Oculus Quest 1 or 2
2. OpenHAB (running on local or remote server)
3. IoT devices

## IoT VR Platform package
Available items (the list is frequently updated):

| [![Lamp](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/Lamp.png)]() [Light Item](Documentation/Things/Lamp.md) | [![Door](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/Door.png)]() [Door](Documentation/Things/Door.md) | [![WeightScaler](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/WeightScaler.png)]() [Weight Scaler](Documentation/Things/WeightScaler.md) | 
|:--- | :--- | :--- |
| A lamp thing with Location and Light items attached | A door with a door close/open sensor item attached | Weight Scaler item triggers according to the weight scaled on it |
| [![Camera](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/Camera.png)]() [Camera](Documentation/Things/Camera.md) | [![TV](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/TV.png)]() [TV](Documentation/Things/TV.md) | [![Vacuum Cleaner](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/VacuumCleaner.png)]() [Vacuum Cleaner](Documentation/Things/VacuumCleaner.md) |
| A camera with a motion sensor connected | A TV translating an image from the camera | A vacuum cleaner thing, which can be docked/undocked and move around the scene |

### Gesture recognizer

GestureRecognizerItem.cs item class enables usage of user-defined gestures. In the example, the Lamp toggles when Index finger tips pushed together.

![](https://github.com/FedorIvachev/IoThingsLab-ReadmeFiles/blob/master/Readme/Files/Gesture.gif)


### Thing constructor 

In the Client scene you can find a GameObject called ThingContrustructor. Add up 6 item prefabs into it, and then create your own thing using the Control panel GameObject. You can instantiate new items, move them around the scene and even edit them (in the next update). Save the newly created thing as a prefab using Unity Editor (in Play Mode).  

### IoT VR Package Structure 
The main part of the package is Server, where classes for the items and things are defined.

Things are Gameobjects added into Unity project which represent either a digital twin of a real IoT device or a purely virtual IoT device. Things potentially provide many functionalities in one.

Items are the parts things consist of: for example, smart light in the room can consist of several lamps and a receiver – each of them as an item.
	
Other parts of IoT VR Platform are: resources, prefabs, Thirdparty and scenes.	
	
	
## Setup

### VR (Client) part
1. [VIDEO TUTORIAL](https://www.bilibili.com/video/BV1vr4y1F7Jg) Intall MRTK [here](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Installation.html) and configure it for Oculus [here](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/OculusQuestMRTK.html)
OR just use [this project](https://github.com/provencher/MRTK-Quest-Sample) to save time.
2. [VIDEO TUTORIAL](https://www.bilibili.com/video/BV17z4y1y7Bb) Import IoThingsLab.unitypackage from the [latest release](https://github.com/VRSimulator/IoThingsLab/releases).

### Input simulation
[Input simulation service Documentation](https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html)

### Tutorial on the IoT part and how to run the server on the local machine
1. Install [openHAB](https://www.openhab.org/download/) and run the server.;
2. Add [REST API](https://www.openhab.org/docs/configuration/restdocs.html) binding;
3. Follow the instructions for binding your IoT device;

## Known Issues
1. If when running the platform on PC (Unity Editor) you get the following error:
![](/Readme/Files/ErrorXRSDK.png)
Then you need to do step 5 of the setup process: On the top of Unity Editor, select Mixed Reality Toolkit -> Utilities -> Oculus -> Integrate Oculus Integration Unity Modules;
![](/Readme/Files/Screenshot(33).png)

## Contributing to the platform
请分享新想法, 谢谢！
You can copy the Client Scene and test your ideas before merging the changes:) Thank you!
