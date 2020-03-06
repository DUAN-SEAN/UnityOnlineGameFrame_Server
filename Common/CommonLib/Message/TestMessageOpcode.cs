using Crazy.Common;
namespace Crazy.Common
{
////��֤��½��Ϣ
	[Message(TestMessageOpcode.C2S_TestMessage)]
	public partial class C2S_TestMessage : IMessage {}

	[Message(TestMessageOpcode.S2C_AllocPlayerIdMessage)]
	public partial class S2C_AllocPlayerIdMessage : IMessage {}

////��֤��½��Ϣ
	[Message(TestMessageOpcode.C2S_PlayerSyncEntityMessage)]
	public partial class C2S_PlayerSyncEntityMessage : IMessage {}

	[Message(TestMessageOpcode.Vector3)]
	public partial class Vector3 {}

}
namespace Crazy.Common
{
	public static partial class TestMessageOpcode
	{
		 public const ushort C2S_TestMessage = 1054;
		 public const ushort S2C_AllocPlayerIdMessage = 1055;
		 public const ushort C2S_PlayerSyncEntityMessage = 1056;
		 public const ushort Vector3 = 1057;
	}
}
