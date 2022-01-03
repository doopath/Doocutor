# Doocutor

## What's doocutor
**Doocutor** is a **C#** code editor. It helps you to write some minor code. For example when
you want to show or test learned **C#** features but you're not going to start IDE because it's
pretty slowly.

You can write **C#** code with doocutor, compile and running it without IDE or terminal commands.
****

## Doocutor features
**Doocutor** can compile and run your written **C#** code. **Doocutor** also has useful commands like
:using or :removeBlock to help you navigate and edit your code. The user guide you can see below.
****

## User guide
At first you can type :help for getting a list of supported commands.

**Doocutor** set the default **C#** application layout. You can see your code with :view command.

To write something, type a line, for example: Console.WriteLine("Hello world");

If you want to use an external library, type :using <Lib name>. For example: :using System.

To run your code type :run <arg1, arg2, arg3>. Also you can compile your code without running,
just type :compile. If your code is incorrect then errors of compilation will be shown on your screen.
In our case the Main method does not require arguments, so just type :run and you will get something like:

**Output:**
<br/>
Hello world

![no image](https://raw.githubusercontent.com/doopath/Doocutor/develop/resources/img/Doocutor_DynamicEditor.png)
![no image](https://raw.githubusercontent.com/doopath/Doocutor/develop/resources/img/Doocutor_StaticEditor.png)

(On the pictures shown dynamic and static editors preview. If you want to use dynamic editor you should use *"--editor-mode dynamic"* launch option)
<br/>
When you write lines of code you don't must set tabulation because **Doocutor** will set it
automatically.
****

## Installation
**Doocutor** binaries contained as portable version.

Visit the [release page](https://github.com/doopath/doocutor/releases) and download an archive with Doocutor portable (osx, linux, win)-x64
then unpack it and move this directory (for example in C:\Program Files\ or /usr/bin/) and create a shortcut.
****
