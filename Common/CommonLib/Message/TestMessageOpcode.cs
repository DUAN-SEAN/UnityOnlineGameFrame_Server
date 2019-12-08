using Crazy.Common;
namespace Crazy.Common
{
////��֤��½��Ϣ
	[Message(TestMessageOpcode.C2S_TestMessage)]
	public partial class C2S_TestMessage : IMessage {}

}
namespace Crazy.Common
{
	public static partial class TestMessageOpcode
	{
		 public const ushort C2S_TestMessage = 1054;
	}
}
