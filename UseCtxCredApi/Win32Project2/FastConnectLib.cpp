// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "FastConnectLib.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

#ifdef APPRECEIVERSHIM_EXPORTS
#define APPRECEIVERSHIM_API __declspec(dllexport)
#else
#define APPRECEIVERSHIM_API __declspec(dllimport)
#endif


#define CCM_CALL_CONV __stdcall

typedef DWORD (CCM_CALL_CONV *PFNSSOLOGOFF)									 (DWORD timeout);
typedef void (CCM_CALL_CONV *PFNSSOLOGON)                  (MSV1_0_INTERACTIVE_LOGON *pNewCredentials, BOOL bDisconnectCurrentUser, BOOL bRestartPna, DWORD *pDwResult);

PFNSSOLOGOFF g_pfnSSOLogoff = 0;
PFNSSOLOGON g_pfnSSOLogon = 0;


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



DWORD LogoffSsoUser(DWORD timeout)
{
	HMODULE g_CCMDLL = 0; // Handle to the CCM dll
	g_CCMDLL = LoadLibrary(L"ctxcredapi.dll");
	g_pfnSSOLogoff = (PFNSSOLOGOFF)GetProcAddress(g_CCMDLL, "LogoffSsoUser");
	g_pfnSSOLogoff(0);
	return 0; // success
}

enum LOGONSSOUSER_ERROR_CODE
{
	LOGONSSOUSER_OK = 0,
	LOGONSSOUSER_UNABLE_TO_GET_PIPE_NAME = -1,
	LOGONSSOUSER_UNABLE_TO_CONNECT_TO_SSO = -2,
	LOGONSSOUSER_UNABLE_TO_SEND_REQUEST = -3,
	LOGONSSOUSER_INVALID_RESPONSE = -4
};

void LogonSsoUser(MSV1_0_INTERACTIVE_LOGON *pNewCredentials,
	BOOL bDisconnectCurrentUser,
	BOOL bRestartPna,
	DWORD *pDwResult)
{
	HMODULE g_CCMDLL = 0; // Handle to the CCM dll
	g_CCMDLL = LoadLibrary(L"ctxcredapi.dll");
	g_pfnSSOLogon = (PFNSSOLOGON)GetProcAddress(g_CCMDLL, "LogonSsoUser");
	return g_pfnSSOLogon(pNewCredentials, bDisconnectCurrentUser, bRestartPna, pDwResult);
}


