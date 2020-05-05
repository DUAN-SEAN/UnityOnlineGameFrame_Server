using Crazy.Common;
namespace Crazy.Common
{
	[Message(ServerInnerMessageOpcode.G2S_TestServerMessage)]
	public partial class G2S_TestServerMessage : IMessage {}

//ץס�ڲ�һ��Server
	[Message(ServerInnerMessageOpcode.G2S_CatchServerMessage)]
	public partial class G2S_CatchServerMessage : IRequest {}

	[Message(ServerInnerMessageOpcode.S2G_CatchServerMessage)]
	public partial class S2G_CatchServerMessage : IResponse {}

	[Message(ServerInnerMessageOpcode.S2G_WarppedServerMessage)]
	public partial class S2G_WarppedServerMessage : IMessage {}

//ץס�ڲ�һ��Server
	[Message(ServerInnerMessageOpcode.G2L_GetRoomPlayersServerMessageReq)]
	public partial class G2L_GetRoomPlayersServerMessageReq : IRequest {}

	[Message(ServerInnerMessageOpcode.L2G_GetRoomPlayersServerMessageACK)]
	public partial class L2G_GetRoomPlayersServerMessageACK : IResponse {}

}
namespace Crazy.Common
{
	public static partial class ServerInnerMessageOpcode
	{
		 public const ushort G2S_TestServerMessage = 1054;
		 public const ushort G2S_CatchServerMessage = 1055;
		 public const ushort S2G_CatchServerMessage = 1056;
		 public const ushort S2G_WarppedServerMessage = 1057;
		 public const ushort G2L_GetRoomPlayersServerMessageReq = 1058;
		 public const ushort L2G_GetRoomPlayersServerMessageACK = 1059;
	}
}
