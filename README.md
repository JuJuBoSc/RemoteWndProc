# Remote WndProc Executor


This class allow you to run code in a remote process using SendMessage and WndProc override.

It use [MyMemory](https://github.com/JuJuBoSc/MyMemory) libary and only support x86 (tho x64 is barely the same).

Feel free to copy paste.

### Here is how it work step by step :

* It generate a custom message number to handle future request.
* A codecave is written in the remote process as a WndProc callback.
* When we want to execute code, we call SendMessage from our application with our custom message.
* Then the remote process *should* call our callback.
* The callback detect our custom message.
* The callback then call the function passed in wParam.
* It then store result of the call (EAX) into lParam pointer.
* The program read the value stored in lParam pointer.
* Done, profit !
