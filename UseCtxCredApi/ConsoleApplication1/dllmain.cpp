// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

#ifdef APPRECEIVERSHIM_EXPORTS
#define APPRECEIVERSHIM_API __declspec(dllexport)
#else
#define APPRECEIVERSHIM_API __declspec(dllimport)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

