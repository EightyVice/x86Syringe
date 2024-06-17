#include <Windows.h>


#using <mscorlib.dll>
#using <System.dll>
#using <WindowsBase.dll>

using namespace System;
using namespace System::Threading;
using namespace System::Windows;
using namespace x86Syringe;

#pragma managed
void Bootstrap() {
    // Create and run the WPF application
    Application^ app = gcnew Application();
    MainWindow^ mainWindow = gcnew MainWindow();
    app->Run(mainWindow);
}

void LoadApp() {
    MessageBox::Show("hey");
    // Start the WPF application on a new thread
    Thread^ wpfThread = gcnew Thread(gcnew ThreadStart(Bootstrap));

    wpfThread->SetApartmentState(ApartmentState::STA);
    wpfThread->Start();
    wpfThread->Join();
}


#pragma unmanaged
DWORD GoManaged() {
    LoadApp();
    return 0;
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved) 
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        exit(-1);
        CreateThread(nullptr, 0, (LPTHREAD_START_ROUTINE)GoManaged, nullptr, 0, NULL);
        break;

    case DLL_THREAD_ATTACH:
        break;

    case DLL_THREAD_DETACH:
        break;

    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}