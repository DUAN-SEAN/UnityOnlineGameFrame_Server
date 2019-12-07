using Crazy.Common;
namespace Crazy.Common
{
	[Message(MessageOpcode.ChatOneMessage)]
	public partial class ChatOneMessage : IMessage {}

// ��ƥ��ϵͳ��ص���Ϣд����
	[Message(MessageOpcode.C2S_CreateMatchTeam)]
	public partial class C2S_CreateMatchTeam : IMessage {}

	[Message(MessageOpcode.S2C_CreateMatchTeamComplete)]
	public partial class S2C_CreateMatchTeamComplete : IMessage {}

	[Message(MessageOpcode.C2S_InvitePlayerMatchTeam)]
	public partial class C2S_InvitePlayerMatchTeam : IMessage {}

	[Message(MessageOpcode.S2C_InvitePlayerMatchTeam)]
	public partial class S2C_InvitePlayerMatchTeam : IMessage {}

	[Message(MessageOpcode.C2S_JoinMatchTeam)]
	public partial class C2S_JoinMatchTeam : IMessage {}

	[Message(MessageOpcode.S2CM_JoinMatchTeamComplete)]
	public partial class S2CM_JoinMatchTeamComplete : IMessage {}

	[Message(MessageOpcode.C2S_GetMatchTeamInfo)]
	public partial class C2S_GetMatchTeamInfo : IMessage {}

	[Message(MessageOpcode.S2C_UpdateMatchTeamInfo)]
	public partial class S2C_UpdateMatchTeamInfo : IMessage {}

	[Message(MessageOpcode.MatchTeamInfo)]
	public partial class MatchTeamInfo {}

	[Message(MessageOpcode.C2S_ExitMatchTeam)]
	public partial class C2S_ExitMatchTeam : IMessage {}

	[Message(MessageOpcode.S2CM_ExitMatchTeamComplete)]
	public partial class S2CM_ExitMatchTeamComplete : IMessage {}

	[Message(MessageOpcode.C2S_JoinMatchQueue)]
	public partial class C2S_JoinMatchQueue : IMessage {}

	[Message(MessageOpcode.S2CM_JoinMatchQueueComplete)]
	public partial class S2CM_JoinMatchQueueComplete : IMessage {}

	[Message(MessageOpcode.C2S_ExitMatchQueue)]
	public partial class C2S_ExitMatchQueue : IMessage {}

	[Message(MessageOpcode.S2CM_ExitMatchQueue)]
	public partial class S2CM_ExitMatchQueue : IMessage {}

	[Message(MessageOpcode.S2CM_MatchingFinish)]
	public partial class S2CM_MatchingFinish : IMessage {}

// ��ս��ϵͳ��ص���Ϣд������
	[Message(MessageOpcode.S2CM_CreateBattleBarrier)]
	public partial class S2CM_CreateBattleBarrier : IMessage {}

	[Message(MessageOpcode.CreateBattleBarrierInfo)]
	public partial class CreateBattleBarrierInfo {}

//����Ϊ����ͨ����Ϣ
	[Message(MessageOpcode.C2S_ReConnectByLogin)]
	public partial class C2S_ReConnectByLogin : IMessage {}

//����Ϊ����ͨ����Ϣ
	[Message(MessageOpcode.C2S_SpeakToTeamReq)]
	public partial class C2S_SpeakToTeamReq : IMessage {}

//����Ϊ����ͨ����Ϣ
	[Message(MessageOpcode.S2C_SpeakToTeamAck)]
	public partial class S2C_SpeakToTeamAck : IMessage {}

//����Ϊ����ͨ����Ϣ
	[Message(MessageOpcode.C2S_SpeakToBattleReq)]
	public partial class C2S_SpeakToBattleReq : IMessage {}

//����Ϊ����ͨ����Ϣ
	[Message(MessageOpcode.S2C_SpeakToBattleAck)]
	public partial class S2C_SpeakToBattleAck : IMessage {}

}
namespace Crazy.Common
{
	public static partial class MessageOpcode
	{
		 public const ushort ChatOneMessage = 1018;
		 public const ushort C2S_CreateMatchTeam = 1019;
		 public const ushort S2C_CreateMatchTeamComplete = 1020;
		 public const ushort C2S_InvitePlayerMatchTeam = 1021;
		 public const ushort S2C_InvitePlayerMatchTeam = 1022;
		 public const ushort C2S_JoinMatchTeam = 1023;
		 public const ushort S2CM_JoinMatchTeamComplete = 1024;
		 public const ushort C2S_GetMatchTeamInfo = 1025;
		 public const ushort S2C_UpdateMatchTeamInfo = 1026;
		 public const ushort MatchTeamInfo = 1027;
		 public const ushort C2S_ExitMatchTeam = 1028;
		 public const ushort S2CM_ExitMatchTeamComplete = 1029;
		 public const ushort C2S_JoinMatchQueue = 1030;
		 public const ushort S2CM_JoinMatchQueueComplete = 1031;
		 public const ushort C2S_ExitMatchQueue = 1032;
		 public const ushort S2CM_ExitMatchQueue = 1033;
		 public const ushort S2CM_MatchingFinish = 1034;
		 public const ushort S2CM_CreateBattleBarrier = 1035;
		 public const ushort CreateBattleBarrierInfo = 1036;
		 public const ushort C2S_ReConnectByLogin = 1037;
		 public const ushort C2S_SpeakToTeamReq = 1038;
		 public const ushort S2C_SpeakToTeamAck = 1039;
		 public const ushort C2S_SpeakToBattleReq = 1040;
		 public const ushort S2C_SpeakToBattleAck = 1041;
	}
}
