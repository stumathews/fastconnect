using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using WFICALib;


namespace UseCtxCredApi
{
  class Program
  {
    static void Main( string[] args )
    {

      MSV1_0_INTERACTIVE_LOGON logon = new MSV1_0_INTERACTIVE_LOGON();
      logon.LogonDomainName = FastConnectWrapper.InitLsaString("xa.local");
      logon.MessageType = MSV1_0_LOGON_SUBMIT_TYPE.MsV1_0InteractiveLogon;
      logon.Password = FastConnectWrapper.InitLsaString( "citrix" );
      logon.UserName = FastConnectWrapper.InitLsaString( "user1" );

     
      FastConnectWrapper m = new FastConnectWrapper();
      
      

      int result = 100;
      LOGONSSOUSER_ERROR_CODE intermediate = m.MyLogonSsoUser( ref logon, 1, 1,ref result );

      System.Console.WriteLine("Press any key to Logoff");
      System.Console.ReadKey();
      
      m.MyLogoffSsoUser(1);

    }

    
  }
  
  class FastConnectWrapper
  {
    [DllImport( "CtxCredApi.dll", EntryPoint = "LogonSsoUser", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
    private static extern LOGONSSOUSER_ERROR_CODE LogonSsoUser(ref MSV1_0_INTERACTIVE_LOGON pNewCredentials, 
      int bDisconnectCurrentUser, 
      int bRestartPna, 
      ref int pDwResult );

    [DllImport( "CtxCredApi.dll", EntryPoint = "LogoffSsoUser",CallingConvention = CallingConvention.Winapi)]
    private static extern UInt32 LogoffSsoUser( 
      UInt32 timeout );

    public LOGONSSOUSER_ERROR_CODE MyLogonSsoUser( ref MSV1_0_INTERACTIVE_LOGON newCredentials, 
      int disconnectCurrentUser, 
      int restartPna,
      ref int result )
    {
      return LogonSsoUser( ref newCredentials, disconnectCurrentUser, restartPna, ref result );
    }

    public UInt32 MyLogoffSsoUser( UInt32 timeout )
    {
      return LogoffSsoUser( timeout );
    }

    public static LSA_UNICODE_STRING InitLsaString( string s )
    {
      // Unicode strings max. 32KB
      if(s.Length > 0x7ffe)
        throw new ArgumentException( "String too long" );
      LSA_UNICODE_STRING lus = new LSA_UNICODE_STRING();
      lus.Buffer = s;
      lus.Length = (ushort) (s.Length * sizeof( char ));
      lus.MaximumLength = (ushort) (lus.Length + sizeof( char ));
      return lus;
    }
  }
  /// <summary>
  /// Type of SSO logon error code (Citrix)
  /// </summary>
  public enum LOGONSSOUSER_ERROR_CODE
  {
    LOGONSSOUSER_OK = 0,
    LOGONSSOUSER_UNABLE_TO_GET_PIPE_NAME = -1,
    LOGONSSOUSER_UNABLE_TO_CONNECT_TO_SSO = -2,
    LOGONSSOUSER_UNABLE_TO_SEND_REQUEST = -3,
    LOGONSSOUSER_INVALID_RESPONSE = -4
  };

  public enum MSV1_0_LOGON_SUBMIT_TYPE
  {
    MsV1_0InteractiveLogon = 2,
    MsV1_0Lm20Logon,
    MsV1_0NetworkLogon,
    MsV1_0SubAuthLogon,
    MsV1_0WorkstationUnlockLogon = 7,
    MsV1_0S4ULogon = 12,
    MsV1_0VirtualLogon = 82,
    MsV1_0NoElevationLogon = 82
  };

  /// <summary>
  /// Interactive logon structure for use with Citrix API (msdn)
  /// </summary>
  [StructLayout( LayoutKind.Sequential, Pack = 1 )]
  public struct MSV1_0_INTERACTIVE_LOGON
  {
    public MSV1_0_LOGON_SUBMIT_TYPE MessageType;
    public LSA_UNICODE_STRING LogonDomainName;
    public LSA_UNICODE_STRING UserName;
    public LSA_UNICODE_STRING Password;
  }



  [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
  public struct LSA_UNICODE_STRING
  {
     public ushort Length;
     public ushort MaximumLength;
    [MarshalAs( UnmanagedType.LPWStr )]
     public string Buffer;
  }

  internal class FastConnectLibraryWrapper
  {
    [DllImport("FastConnectLib.dll")]
    internal static extern UInt32 LogoffSsoUser( UInt32 timeout );

    [DllImport( "FastConnectLib.dll" )]
    internal static extern LOGONSSOUSER_ERROR_CODE LogonSsoUser( ref MSV1_0_INTERACTIVE_LOGON pNewCredentials,
                                              int bDisconnectCurrentUser,
                                              int bRestartPna,
                                              ref UInt32 pDwResult );

    public UInt32 callLogoffSsoUser( UInt32 timeout )
    {
      return LogoffSsoUser( timeout );
    }

    public LOGONSSOUSER_ERROR_CODE callLogonSsoUser( ref MSV1_0_INTERACTIVE_LOGON pNewCredentials,
                                              int bDisconnectCurrentUser,
                                              int bRestartPna,
                                              ref UInt32 pDwResult )
    {
      return LogonSsoUser( ref pNewCredentials, bDisconnectCurrentUser, bRestartPna, ref pDwResult );
    }

  }

}
