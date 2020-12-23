ThermoCam160B
=========
library and sample to access to thermal camera board on Android device

This application contains copyrighted library under Apache License version 2.0.

Copyright (c) 2014-2017 saki t_saki@serenegiant.com

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

All files in the folder are under this Apache License, Version 2.0.
Files in the jni/libjpeg, jni/libusb and jin/libuvc folders may have a different license,
see the respective files.

How to compile library  
=========
The Gradle build system will build the entire project, including the NDK parts. If you want to build with Gradle build system,

1. make directory on your favorite place (this directory is parent directory of `UVCCamera` project).
2. change directory into the directory.
3. clone this repository with `git  clone https://github.com/saki4510t/UVCCamera.git`
4. change directory into `UVCCamera` directory with `cd UVCCamera`
5. build library with all sample projects using `gradle build`

It will takes several minutes to build. Now you can see apks in each `{sample project}/build/outputs/apks` directory.  
Or if you want to install and try all sample projects on your device, run `gradle installDebug`.  

Note: Just make sure that `local.properties` contains the paths for `sdk.dir` and `ndk.dir`. Or you can set them as enviroment variables of you shell. On some system, you may need add `JAVA_HOME` envairoment valiable that points to JDK directory.  

If you want to use Android Studio(unfortunately NDK supporting on Android Studio is very poor though),
1. make directory on your favorite place (this directory is parent directory of `UVCCamera` project).
2. change directory into the directory.
3. clone this repository with `git  clone https://github.com/saki4510t/UVCCamera.git`
4. start Android Studio and open the cloned repository using `Open an existing Android Studio project`
5. Android Studio raise some errors but just ignore now. Android Studio generate `local.properties` file. Please open `local.properties` and add `ndk.dir` key to the end of the file. The contents of the file looks like this.
```
sdk.dir={path to Android SDK on your storage}
ndk.dir={path to Android SDK on your storage}
```
Please replace actual path to SDK and NDK on your storage.  
Of course you can make `local.properties` by manually instead of using automatically generated ones by Android Studio.
6. Synchronize project
7. execute `Make project` from `Build` menu.

If you want to use build-in VCS on Android Studio, use `Check out project from Version Control` from `https://github.com/saki4510t/UVCCamera.git`. After cloning, Android Studio ask you open the project but don't open now. Instead open the project using `Open an existing Android Studio project`. Other procedures are same as above.

If you still need to use Eclipse or if you don't want to use Gradle with some reason, you can build suing `ndk-build` command.

1. make directory on your favorite place.
2. change directory into the directory.
3. clone this repository with `git  clone https://github.com/saki4510t/UVCCamera.git`
4. change directory into `{UVCCamera}/libuvccamera/build/src/main/jni` directory.
5. run `ndk-build`
6. resulted shared libraries are available under `{UVCCamera}/libuvccamera/build/src/main/libs` directory and copy them into your project with directories by manually.
7. copy files under `{UVCCamera}/libuvccamera/build/src/main/java` into your project source directory by manually.

How to use
=========
Please see sample project and/or our web site(but sorry web site is Japanese only).
These sample projects are IntelliJ projects, as is the library.
This library works on at least Android 3.1 or later(API >= 12), but Android 4.0(API >= 14)
or later is better. USB host function must be required.
If you want to try on Android 3.1, you will need some modification(need to remove
setPreviewTexture method in UVCCamera.java etc.), but we have not confirm whether the sample
project run on Android 3.1 yet.
Some sample projects need API>=18 though.

