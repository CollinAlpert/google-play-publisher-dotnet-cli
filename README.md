# Google Play Publisher

This application interacts with the Google Developer Publishing API and uses the transactional "edits" functionality to upload APKs or App bundles to the Google Play Store.
If you are missing a feature, something is not working as expected, or you are in need of clarification, feel free to [create an issue](https://github.com/CollinAlpert/google-play-publisher-dotnet-cli/issues/new) or a PR. Just make sure to check out the [Roadmap](#roadmap) beforehand.

## Requirements
There are a few things you need for this:
#### Not .NET Core 3.1 
Even though this tool is written in .NET Core 3.1, you do not need to install it. In a release, choose the binary for your operating system and run it. The runtime is packaged inside the binary.

#### A service account
A service account is an account which can talk to the Publishing API. You can create one in the [Google Console](https://console.cloud.google.com/apis/credentials). If you already have one (there is usually at least one already created), even better. Make sure you create a key and download the JSON file. You'll need this file when using the application. Note that this application **does not** support the use of a P12 key file. 

#### An APK or App bundle
Lastly, you'll need the file which you want to upload.

#### Patience
Since this application executes multiple API calls and uploads a multi-megabyte file, it takes a while. Wait at least 60 seconds after running the tool until you go on your `htop` killing spree.
I will try to improve the interactivity going forward.

# Installation
You can either choose to clone this repo and run it yourself using .NET Core 3.1, or download the most recent [Binary](https://github.com/CollinAlpert/google-play-publisher-dotnet-cli/releases/latest) and run that. A .NET Core installation is not necessary.

# Usage
You can set all the configuration options in a JSON file which you pass to the tool when you run it.

#### App name
The name of the app. Simple as that.

#### Package name
The full package name of the app, as chosen by you. It is displayed under the app name in the Google Play Console app overview.

#### The JSON service account file
This is the key file for the service account which is needed to talk to the Google Publishing API. You need to choose it before uploading an APK or App bundle, or else it doesn't work.

#### The application file
The APK or app bundle file you want to upload.

The basic structure for the JSON configuration file looks like this:

```json
{
  "name": "AppName",
  "package": "org.example.app",
  "serviceAccountFile": "/absolute/path/to/json/service/account/file",
  "applicationFile": "/absolute/path/to/application/file"
}
``` 

When running the tool, simply pass the file name of the configuration file as a command line argument.

# Roadmap
Things which I will try to implement down the road:
- Improved error handling/display. The current error display, is, let's just say, suboptimal.
- Support for multi-language release notes. Currently, only one language is supported.
- More interactivity while waiting for the publishing process to complete.